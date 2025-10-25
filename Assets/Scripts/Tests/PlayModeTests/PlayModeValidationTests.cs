using System.Collections;
using System.Collections.Generic;
using LifetimeScope;
using NUnit.Framework;
using Presenters;
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
        public IEnumerator PlayModeValidation_StartGameOnPlayMode()
        {
            yield return LoadSampleScene();

            var applicationLifetimeScope = GameObject.Find("ApplicationLifetimeScope");
            Assert.IsNotNull(applicationLifetimeScope);
            var scopeContainer = applicationLifetimeScope.GetComponent<ApplicationLifetimeScope>();
            Assert.IsNotNull(scopeContainer);

            yield return new WaitForSeconds(3.0f);

            var gameManagerPresenter = scopeContainer.Container.Resolve<GeneralGamePresenter>();
            Assert.IsNotNull(gameManagerPresenter);
            
            var gameManagerView = scopeContainer.Container.Resolve<GeneralGameView>();
            Assert.IsNotNull(gameManagerView);
            
            // trigger newGame as in button interaction.
            gameManagerView.NewGameButtonClicked?.Invoke();
            
            // Check that deck has the proper suit.
            var tableUIPresenter = scopeContainer.Container.Resolve<TableUIPresenter>();
            Assert.IsNotNull(tableUIPresenter);
            
            Assert.IsNotNull(tableUIPresenter.SelectedCardSuit == gameManagerPresenter.TestOnlyGameManagerData.DeckInitialCardSuit);
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
