using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Presenters;
using Services;
using Unity.MLAgents;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Training
{
    public class CardGameAcademy : MonoBehaviour, IInitializable, IDisposable
    {
        private GameManagerPresenter gameManagerPresenter;
        private PlayersService playersService;

        [SerializeField]
        private PlayerTrainingAgent[] playerTrainingAgents;

        [Inject]
        public void Inject(
            GameManagerPresenter gameManagerPresenter,
            PlayersService playersService)
        {
            this.gameManagerPresenter = gameManagerPresenter;
            this.playersService = playersService;
        }

        private void Awake()
        {
            Academy.Instance.OnEnvironmentReset += OnEnvironmentReset;
            Academy.Instance.AutomaticSteppingEnabled = false;
        }

        private void OnEnable()
        {

        }

        private void OnDestroy()
        {
            Academy.Instance.OnEnvironmentReset -= OnEnvironmentReset;
        }

        public void Initialize()
        {
            gameManagerPresenter.OnGameFinished += OnGameFinished;
            gameManagerPresenter.OnGameRoundStarted += OnGameRoundStarted;
            gameManagerPresenter.OnGameRoundFinished += OnGameRoundFinished;
        }

        private async Task WaitAndStartSimulation()
        {
            // Wait for all stuff to be initialized.
            await Task.Delay(3000);
            Academy.Instance.EnvironmentStep();

        }

        private void OnGameRoundFinished()
        {
            var currentRound = gameManagerPresenter.GetCurrentRound();
            var roundWinnerId= currentRound.GameRoundData.RoundWinnerId;

            foreach (var player in playerTrainingAgents)
            {
                var rewardPerRoundWon = player.PlayerId == roundWinnerId ? 0.3f : - 0.1f;
                player.SetReward(rewardPerRoundWon);
            }

        }

        private void OnGameRoundStarted()
        {
            Academy.Instance.EnvironmentStep();
        }

        public void Dispose()
        {
            gameManagerPresenter.OnGameFinished -= OnGameFinished;
            gameManagerPresenter.OnGameRoundStarted -= OnGameRoundStarted;
            gameManagerPresenter.OnGameRoundFinished -= OnGameRoundFinished;
        }

        private void OnEnvironmentReset()
        {
            gameManagerPresenter.FinishGame();
            gameManagerPresenter.StartGameButtonPressed();
            DisablePlayersInput();
            gameManagerPresenter.StartNextRoundButtonPressed();
        }

        private void OnGameFinished()
        {
            //Get player scores, order them from highest to lowest, and set their rewards depending on that state.
            var playersData = playersService.GetAllPlayersData();

            var playersScores = new SortedList<int, int>(playersData.Count);

            foreach (var playerData in playersData)
            {
                playersScores.Add(playerData.GetScore(), playerData.PlayerId);
            }

            for (int i = 0; i < playersScores.Count; i++)
            {
                foreach(var playerAgent in playerTrainingAgents)
                {
                    if (playersScores[i] == playerAgent.PlayerId)
                    {
                        var normalizedReward = i + 1 / playersScores.Count;
                        playerAgent.SetReward(normalizedReward);
                    }
                }
            }

            foreach (var agent in playerTrainingAgents)
            {
                agent.EndEpisode();
            }
        }

        private void DisablePlayersInput()
        {
            var allPlayers = playersService.GetAllPlayers();
            foreach (var player in allPlayers)
            {
                player.DisablePlayerInput();
            }
        }
    }
}