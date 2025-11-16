using System;
using Data;
using R3;
using Services;
using VContainer.Unity;
using View;

namespace Presenters
{
    public class GeneralGamePresenter : IInitializable, IDisposable
    {
        private readonly GeneralGameView gameView;

        public Action<CardSuit> OnGameStarted;
        public Action OnGameRoundStarted;
        public Action OnGameRoundFinished;
        
        private readonly GameManagerData gameManagerData;
        private readonly PlayersService playersService;

        private IDisposable currentRoundIndexDisposable;

        private bool hasGameStarted = false;

        public GeneralGamePresenter(
            GameManagerData gameManagerData, 
            PlayersService playersService,
            GeneralGameView gameView)
        {
            this.gameManagerData = gameManagerData;
            this.playersService = playersService;
            this.gameView = gameView;
            
            playersService.OnPlayersInitialized += OnPlayersInitialized;
        }

        // MODEL ONLY TEST CONSTRUCTOR
        public GeneralGamePresenter(
            GameManagerData gameManagerData,
            PlayersService playersService)
        {
            this.gameManagerData = gameManagerData;
            this.playersService = playersService;

            playersService.OnPlayersInitialized += OnPlayersInitialized;
        }

        public void StartGameButtonPressed()
        {
            hasGameStarted = true;
            playersService.ResetDataOnPlayers();
            gameManagerData.ResetAll();
            gameManagerData.SetupDeckForNewGame();
            StartGame();
            OnGameStarted?.Invoke(gameManagerData.DeckInitialCardSuit);
        }

        public void Initialize()
        {
            gameView.NewGameButtonClicked += StartGameButtonPressed;
            gameView.StartNextRoundButtonClicked += StartNextRoundButtonPressed;
            SubscribeOnGameManagerDataStats();

            gameManagerData.InitializeGameData();
        }

        public void StartGame()
        {
            DrawInitialHandForPlayers();
        }

        private void StartNextRoundButtonPressed()
        {
            if (hasGameStarted == false)
            {
                return;
            }
            if (gameManagerData.StartPlayRound())
            {
                OnGameRoundStarted?.Invoke();
            }
        }

        private void DrawInitialHandForPlayers()
        {
            var playersData = playersService.GetAllPlayers();
            var gameDeck = gameManagerData.GetDeckData();
            for (var i = 0; i < playersData.Count; i++)
            {
                var player = playersData[i];
                player.DrawCardsUntilMaxAllowed(gameDeck);
            }
        }

        private void OnPlayersInitialized()
        {
            gameManagerData.ReceivePlayersData(playersService.GetAllPlayers());
        }

        public void Dispose()
        {
            gameView.NewGameButtonClicked -= StartGameButtonPressed;
            gameView.StartNextRoundButtonClicked -= StartNextRoundButtonPressed;
            playersService.OnPlayersInitialized -= OnPlayersInitialized;
            gameManagerData.CurrentRoundPlayPhaseFinished -= OnCurrentRoundPlayPhaseFinished;
            gameManagerData.OnGameFinished -= GameFinished;
            currentRoundIndexDisposable?.Dispose();
        }

        private void SubscribeOnGameManagerDataStats()
        {
            currentRoundIndexDisposable = gameManagerData.CurrentRoundIndex.Subscribe( onNext: roundIndex => gameView.SetRoundNumber(roundIndex.ToString()));

            gameManagerData.CurrentRoundPlayPhaseFinished += OnCurrentRoundPlayPhaseFinished;
            gameManagerData.OnGameFinished += GameFinished;
        }

        private void OnCurrentRoundPlayPhaseFinished()
        {
            var currentRound = gameManagerData.GetCurrentRound();
            if (currentRound != null)
            {
                var winnerId = currentRound.ResolveRound(gameManagerData.DeckInitialCardSuit);
                currentRound.FinishRound(winnerId);
                OnGameRoundFinished?.Invoke();
            }
        }

        private void GameFinished()
        {
            hasGameStarted = false;
            gameView.SetGameOverScreen(true);
            var winnerId = gameManagerData.GameWinnerPlayerId;
            var winnerName = winnerId == -1 ? "You!" : "Player: " + winnerId.ToString();
            gameView.SetGameWinner(winnerName);
        }

    }
}
