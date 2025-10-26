using System.Collections;
using System.Collections.Generic;
using Data;
using LifetimeScope;
using NUnit.Framework;
using Presenters;
using Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VContainer;
using View;

namespace Tests
{
    [TestFixture]
    public class PlayModeValidationTests
    {
        private const string sceneName = "SampleScene";

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator PlayModeValidation_LoadSampleScene()
        {
            yield return LoadSampleScene();
        }

        [UnityTest]
        public IEnumerator PlayModeValidation_GetLifetimeScope()
        {
            yield return LoadSampleScene();

            var applicationLifetimeScope = GameObject.Find("ApplicationLifetimeScope");
            
            Assert.IsNotNull(applicationLifetimeScope);

            var scopeContainer = applicationLifetimeScope.GetComponent<ApplicationLifetimeScope>();

            Assert.IsNotNull(scopeContainer);
        }

        [UnityTest]
        public IEnumerator PlayModeValidation_StartGame_ViewIsInitialized()
        {
            yield return LoadSampleScene();

            var scopeContainer = GetPlayModeLifetimeScope();
            
            //Wait some seconds so that everything has been initialized.
            yield return new WaitForSeconds(3.0f);

            var gameManagerView = scopeContainer.Container.Resolve<GeneralGameView>();
            Assert.IsNotNull(gameManagerView);

            // trigger newGame as in button interaction.
            gameManagerView.NewGameButtonClicked?.Invoke();
            
            // Check that deck has the proper suit.
            var tableUIPresenter = scopeContainer.Container.Resolve<TableUIPresenter>();
            Assert.IsNotNull(tableUIPresenter);
            
            var gameManagerData = scopeContainer.Container.Resolve<GameManagerData>();
            Assert.IsNotNull(gameManagerData);

            Assert.IsNotNull(tableUIPresenter.SelectedCardSuit == gameManagerData.DeckInitialCardSuit);
        }

        [UnityTest]
        public IEnumerator PlayModeValidation_ValidateGameRoundSteps()
        {
            yield return LoadSampleScene();

            var scopeContainer = GetPlayModeLifetimeScope();

            //Wait some seconds so that everything has been initialized.
            yield return new WaitForSeconds(3.0f);
            
            var gameManagerView = scopeContainer.Container.Resolve<GeneralGameView>();
            Assert.IsNotNull(gameManagerView);
            
            DisablePlayerInput(scopeContainer);

            var gameManagerData = scopeContainer.Container.Resolve<GameManagerData>();
            Assert.IsNotNull(gameManagerData);

            // trigger newGame as in button interaction.
            gameManagerView.NewGameButtonClicked?.Invoke();

            // Start game round
            // trigger newGame as in button interaction.
            gameManagerView.StartNextRoundButtonClicked?.Invoke();

            //Round is divided on different phases.
            // Draw phase -- first round it does nothing, but it will make players draw cards on each following round.
            // Play phase -- Each player, in order, plays the cards they want on the established order.
            // Resolve phase -- A round winner is decided by checking the cards that were received from the players, and scores are given.
            var currentGameRound = gameManagerData.GetCurrentRound();
            yield return new WaitForSeconds(1.0f);

            Assert.IsTrue(currentGameRound.IsRoundFinished);
        }

        private void DisablePlayerInput(ApplicationLifetimeScope scope)
        {
            const int playerId = -1;
            //Get players, and disablePlayer input for user (on tests we make it select random cards).
            var playersService = scope.Container.Resolve<PlayersService>();
            Assert.IsNotNull(playersService);

            var players = playersService.GetAllPlayers();
            foreach (var player in players) {
                if (player.PlayerId == playerId) {
                    player.DisablePlayerInput();
                }
            }
        }
        
        private ApplicationLifetimeScope GetPlayModeLifetimeScope()
        {
            var applicationLifetimeScope = GameObject.Find("ApplicationLifetimeScope");
            Assert.IsNotNull(applicationLifetimeScope);
            var scopeContainer = applicationLifetimeScope.GetComponent<ApplicationLifetimeScope>();
            Assert.IsNotNull(scopeContainer);

            return scopeContainer;
        }

        private IEnumerator LoadSampleScene()
        {
            Assert.That(Application.CanStreamedLevelBeLoaded(sceneName),
                "Cannot load scene '{0}' for test '{1}'.",
                sceneName, GetType());
            
            var isSceneLoaded = SceneManager.LoadSceneAsync("SampleScene");

            while (isSceneLoaded.isDone == false) {
                yield return null;
            }
            
            var scene = SceneManager.GetSceneByName(sceneName);
            
            Assert.That(scene.isLoaded);
        }
    }
}
