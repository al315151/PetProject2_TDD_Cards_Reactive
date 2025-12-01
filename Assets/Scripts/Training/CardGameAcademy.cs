using System.Collections.Generic;
using System.Linq;
using Data;
using Factories;
using NUnit.Framework;
using Presenters;
using Services;
using Unity.MLAgents;
using UnityEngine;

namespace Training
{
    public class CardGameAcademy : MonoBehaviour
    {
        private GameManagerPresenter gameManagerPresenter;
        private PlayersService playersService;
        private StrategiesFactory strategiesFactory;

        [SerializeField]
        private PlayerTrainingAgent[] playerTrainingAgents;

        private void Awake()
        {
            InitializeCardGameAcademy();
        }

        private void OnDestroy()
        {
            UnSubscribe();
        }

        private void InitializeCardGameAcademy()
        {
            Academy.Instance.AutomaticSteppingEnabled = false;
            CreateDependencies();
            Subscribe();
            Debug.Log($"[Framecount:{Time.frameCount}] Initialize -- EnvironmentStep");
            Academy.Instance.EnvironmentStep();
        }

        private void CreateDependencies()
        {
            // Everything has to be initialized by hand because training environment does not work well with VContainer.
            Debug.Log($"[Framecount:{Time.frameCount}] Creating dependencies");
            var gameManagerData = new GameManagerData();
            gameManagerData.InitializeGameData();
            strategiesFactory = new StrategiesFactory(gameManagerData);
            playersService = new PlayersService(strategiesFactory);
            playersService.Initialize();
            gameManagerData.ReceivePlayersData(playersService.GetAllPlayersData());
            gameManagerPresenter = new GameManagerPresenter(gameManagerData, playersService);
        }

        private void Subscribe()
        {
            Academy.Instance.OnEnvironmentReset += OnEnvironmentReset;
            gameManagerPresenter.OnGameFinished += OnGameFinished;
            gameManagerPresenter.OnGameRoundStarted += OnGameRoundStarted;
            gameManagerPresenter.OnGameRoundFinished += OnGameRoundFinished;
        }

        private void UnSubscribe()
        {
            Academy.Instance.OnEnvironmentReset -= OnEnvironmentReset;
            gameManagerPresenter.OnGameFinished -= OnGameFinished;
            gameManagerPresenter.OnGameRoundStarted -= OnGameRoundStarted;
            gameManagerPresenter.OnGameRoundFinished -= OnGameRoundFinished;
        }

        private void OnGameRoundFinished()
        {
            Debug.Log($"[Framecount:{Time.frameCount}] OnGameRoundFinished");
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
            Debug.Log($"[Framecount:{Time.frameCount}] OnGameRoundStarted -- Calling EnvironmentStep");
            Academy.Instance.EnvironmentStep();
        }

        private void OnEnvironmentReset()
        {
            Debug.Log($"[Framecount:{Time.frameCount}] OnEnvironmentReset!");
            gameManagerPresenter.FinishGame();
            gameManagerPresenter.StartGameButtonPressed();
            DisablePlayersInput();
            gameManagerPresenter.StartNextRoundButtonPressed();
        }

        private void OnGameFinished()
        {
            if (gameManagerPresenter.HasGameStarted == false)
            {
                return;
            }

            Debug.Log($"[Framecount:{Time.frameCount}] OnGameFinished -- End Episodes?");
            //Get player scores, order them from highest to lowest, and set their rewards depending on that state.
            var playersData = playersService.GetAllPlayersData();

            var playersScores = new List<(int, int)>(playersData.Count);

            foreach (var playerData in playersData)
            {
                playersScores.Add(new (playerData.PlayerId, playerData.GetScore()));
            }

            playersScores = playersScores.OrderBy(x => x.Item2).ToList();

            for (int i = 0; i < playersScores.Count; i++)
            {
                foreach(var playerAgent in playerTrainingAgents)
                {
                    if (playersScores[i].Item1 == playerAgent.PlayerId)
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