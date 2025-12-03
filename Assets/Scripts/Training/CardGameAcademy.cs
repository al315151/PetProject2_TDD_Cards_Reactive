using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Factories;
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
            CreateDependencies();
            Subscribe();
            Debug.Log($"[Framecount:{Time.frameCount}] Initialize");
        }

        private void CreateDependencies()
        {
            // Everything has to be initialized by hand because training environment does not work well with VContainer.
            Debug.Log($"[Framecount:{Time.frameCount}] Creating dependencies");
            var gameManagerData = new GameManagerData();
            gameManagerData.InitializeGameData();
            strategiesFactory = new StrategiesFactory();
            playersService = new PlayersService(strategiesFactory);
            playersService.Initialize();
            gameManagerData.ReceivePlayersData(playersService.GetAllPlayersData());
            gameManagerPresenter = new GameManagerPresenter(gameManagerData, playersService);
            DisableNonPlayersInput();
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
            if (currentRound != null)
            {
                currentRound.OnCardRequestedFromPlayer -= OnRequestedCardFromPlayer;
            }

            var roundWinnerId= currentRound.GameRoundData.RoundWinnerId;

            foreach (var player in playerTrainingAgents)
            {
                var rewardPerRoundWon = player.PlayerId == roundWinnerId ? 0.3f : - 0.1f;
                player.SetReward(rewardPerRoundWon);
            }

            gameManagerPresenter.StartNextRoundButtonPressed();
        }

        private void OnGameRoundStarted()
        {
            Debug.Log($"[Framecount:{Time.frameCount}] OnGameRoundStarted");
            var currentRound = gameManagerPresenter.GetCurrentRound();
            currentRound.OnCardRequestedFromPlayer += OnRequestedCardFromPlayer;
        }

        private void OnRequestedCardFromPlayer(int playerIndexInTurn)
        {
            for (int i = 0; i < playerTrainingAgents.Length; i++)
            {
                if (playerTrainingAgents[i].PlayerId == playerIndexInTurn)
                {
                    Debug.Log($"[Framecount:{Time.frameCount}] OnRequestCardFromPlayer! requesting decision from player: {playerIndexInTurn}");
                    playerTrainingAgents[i].RequestDecision();
                }
            }
        }

        private void OnEnvironmentReset()
        {
            Debug.Log($"[Framecount:{Time.frameCount}] OnEnvironmentReset!");
            StartGame();
        }

        private void StartGame()
        {
            gameManagerPresenter.StartGameButtonPressed();
            DisableNonPlayersInput();
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

            WaitThenStartTheGameAgain();
        }

        private async Task WaitThenStartTheGameAgain()
        {
            await Task.Delay(10);
            //Wait for the game to be finished!
            StartGame();
        }

        private void DisableNonPlayersInput()
        {
            var allPlayers = playersService.GetAllPlayers();
            var trainingPlayers = new List<int>();

            foreach (var trainingAgent in playerTrainingAgents)
            {
                trainingPlayers.Add(trainingAgent.PlayerId);
            }

            foreach (var player in allPlayers)
            {              
                if (trainingPlayers.Contains(player.PlayerId))
                {
                    player.EnableExternalPlayerInput();
                }
                else
                {
                    player.DisableExternalPlayerInput();
                }                
            }            
        }

        public PlayerData GetPlayerData(int playerId)
        {
            var playerPresenter = playersService.GetPlayerPresenterById(playerId);

            return playerPresenter != null ? playerPresenter.GetPlayerData() : null;
        }

        public int GetCurrentNumberOfPlayers()
        {
            return playersService.GetAllPlayers().Count;
        }

        public GameRoundPresenter GetCurrentRoundPresenter()
        {
            return gameManagerPresenter.GetCurrentRound();
        }

        public CardSuit GetPredominantCardSuit()
        {
            return gameManagerPresenter.PredominantCardSuit;
        }

        public void PlayCardFromTrainingAgent(int playerId, CardData cardData)
        {
            var playerPresenter = playersService.GetPlayerPresenterById(playerId);
            if (playerPresenter != null)
            {
                playerPresenter.PlayCardFromUserHand(cardData.CardSuit, cardData.CardNumber);
            }
        }
    }
}