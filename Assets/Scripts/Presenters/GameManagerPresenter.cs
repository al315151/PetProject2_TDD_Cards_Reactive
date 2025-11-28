using System;
using Data;
using R3;
using Services;
using VContainer;
using VContainer.Unity;
using View;

namespace Presenters
{
    public class GameManagerPresenter : IInitializable, IDisposable
    {
        private readonly GeneralGameView gameView;

        public Action<CardSuit> OnGameStarted;
        public Action OnGameRoundStarted;
        public Action OnGameRoundFinished;

        private readonly GameManagerData gameManagerData;
        private readonly PlayersService playersService;

        private GameRoundPresenter currentGameRoundPresenter;

        private IDisposable currentRoundIndexDisposable;
        private IGameRoundPrototype gameRoundConcretePrototype;

        private bool hasGameStarted = false;

        [Inject]
        public GameManagerPresenter(
            GameManagerData gameManagerData,
            PlayersService playersService,
            GeneralGameView gameView)
        {
            this.gameManagerData = gameManagerData;
            this.playersService = playersService;
            this.gameView = gameView;

            gameRoundConcretePrototype = new GameRoundPresenter(gameManagerData.CurrentRoundIndex.Value);

            playersService.OnPlayersInitialized += OnPlayersInitialized;
        }

        // MODEL ONLY TEST CONSTRUCTOR
        public GameManagerPresenter(
            GameManagerData gameManagerData,
            PlayersService playersService)
        {
            this.gameManagerData = gameManagerData;
            this.playersService = playersService;

            gameRoundConcretePrototype = new GameRoundPresenter(gameManagerData.CurrentRoundIndex.Value);

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
            if (hasGameStarted == false) {
                return;
            }
            if (StartPlayRound()) {
                OnGameRoundStarted?.Invoke();
            }
        }

        private void DrawInitialHandForPlayers()
        {
            var playersData = playersService.GetAllPlayers();
            var gameDeck = gameManagerData.GetDeckData();
            for (var i = 0; i < playersData.Count; i++) {
                var player = playersData[i];
                player.DrawCardsUntilMaxAllowed(gameDeck);
            }
        }

        public bool CreateAndStartRound()
        {
            var playersData = playersService.GetAllPlayersData();
            var players = playersService.GetAllPlayers();

            gameManagerData.IncrementCurrentRoundIndex();
            if (gameManagerData.SavePreviousRoundToRoundHistory()) {
                currentGameRoundPresenter.PlayPhaseFinished -= OnCurrentRoundPlayPhaseFinished;
            }

            var gameRoundPrototype = GetNewGameRoundPrototype(gameManagerData.CurrentRoundIndex.Value);

            var gameRound = gameRoundPrototype as GameRoundPresenter;

            gameManagerData.SetupCurrentGameRoundData(gameRound.GameRoundData);

            gameRound.ReceivePlayerPresenters(players);
            gameRound.ReceivePlayers(playersData);
            gameRound.StartPlayerDrawPhase(gameManagerData.GetDeckData());

            gameRound.PlayPhaseFinished += OnCurrentRoundPlayPhaseFinished;

            if (CanRoundBePlayed() == false) {
                currentGameRoundPresenter = null;
                if (IsGameFinished()) {
                    FinishGame();
                }
                return false;
            }

            currentGameRoundPresenter = gameRound;
            return true;
        }

        public void FinishGame()
        {
            var playersData = playersService.GetAllPlayersData();
            var playerMaxScore = -1;
            foreach (var player in playersData) {
                var currentPlayerScore = player.GetScore();
                if (currentPlayerScore > playerMaxScore) {
                    playerMaxScore = currentPlayerScore;
                    gameManagerData.SetupGameWinner(player.PlayerId, playerMaxScore);
                }
            }
            GameFinished();
        }

        private bool CanRoundBePlayed()
        {
            var playersData = playersService.GetAllPlayersData();
            for (var i = 0; i < playersData.Count; i++) {
                if (playersData[i].PlayerHandSize < 1) {
                    return false;
                }
            }
            return true;
        }

        public bool StartPlayRound()
        {
            if (currentGameRoundPresenter != null && currentGameRoundPresenter.IsRoundFinished == false) {
                return false;
            }

            if (IsGameFinished()) {
                FinishGame();
                return false;
            }

            //First setup the Round object.
            if (CreateAndStartRound() == false) {
                return false;
            }
            // Then set round order.
            gameManagerData.EstablishRoundOrder();
            // Then, start the Play phase. we will receive event / wait for the cards to be played 
            currentGameRoundPresenter.StartPlayPhase();
            // On current gameplay, we will have to wait for player input to actually know when to resolve the situation.
            return true;
        }

        public GameRoundPresenter GetCurrentRound()
        {
            return currentGameRoundPresenter;
        }

        private bool IsGameFinished()
        {
            return CanRoundBePlayed() == false && gameManagerData.GetDeckData().DeckCardCount == 0;
        }

        private void OnPlayersInitialized()
        {
            gameManagerData.ReceivePlayersData(playersService.GetAllPlayersData());
        }

        public void Dispose()
        {
            gameView.NewGameButtonClicked -= StartGameButtonPressed;
            gameView.StartNextRoundButtonClicked -= StartNextRoundButtonPressed;
            playersService.OnPlayersInitialized -= OnPlayersInitialized;
            gameManagerData.CurrentRoundPlayPhaseFinished -= OnCurrentRoundPlayPhaseFinished;
            currentRoundIndexDisposable?.Dispose();
        }

        private void SubscribeOnGameManagerDataStats()
        {
            currentRoundIndexDisposable =
                gameManagerData.CurrentRoundIndex.Subscribe(onNext: roundIndex =>
                    gameView.SetRoundNumber(roundIndex.ToString()));

            gameManagerData.CurrentRoundPlayPhaseFinished += OnCurrentRoundPlayPhaseFinished;
        }

        private IGameRoundPrototype GetNewGameRoundPrototype(int newIndex)
        {
            return gameRoundConcretePrototype.Clone(newIndex);
        }

        private void OnCurrentRoundPlayPhaseFinished()
        {
            if (currentGameRoundPresenter != null) {
                var winnerId = currentGameRoundPresenter.ResolveRound(gameManagerData.DeckInitialCardSuit);
                currentGameRoundPresenter.FinishRound(winnerId);
                OnGameRoundFinished?.Invoke();
            }
        }

        private void GameFinished()
        {
            hasGameStarted = false;
            if (gameView == null) {
                return;
            }
            gameView.SetGameOverScreen(true);
            var winnerId = gameManagerData.GameWinnerPlayerId;
            var winnerName = winnerId == -1 ? "You!" : "Player: " + winnerId.ToString();
            gameView.SetGameWinner(winnerName);
        }
    }
}