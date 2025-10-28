using System;
using Data;
using R3;
using Services;
using UnityEngine;
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
        public Action OnGameFinished;
        
        private readonly GameManagerData gameManagerData;
        private readonly PlayersService playersService;

        private IDisposable currentRoundIndexDisposable;

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
        
        public void StartGameButtonPressed()
        {
            playersService.ResetDataOnPlayers();
            gameManagerData.ResetAll();
            gameManagerData.StartGame();            
            OnGameStarted?.Invoke(gameManagerData.DeckInitialCardSuit);
        }

        public void Initialize()
        {
            gameView.NewGameButtonClicked += StartGameButtonPressed;
            gameView.StartNextRoundButtonClicked += StartNextRoundButtonPressed;
            SubscribeOnGameManagerDataStats();

            gameManagerData.CreateGame();
        }

        private void StartNextRoundButtonPressed()
        {
            if (gameManagerData.StartPlayRound())
            {
                OnGameRoundStarted?.Invoke();
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
            currentRoundIndexDisposable?.Dispose();
        }

        private void SubscribeOnGameManagerDataStats()
        {
            currentRoundIndexDisposable = gameManagerData.CurrentRoundIndex.Subscribe( onNext: roundIndex => gameView.SetRoundNumber(roundIndex.ToString()));

            gameManagerData.CurrentRoundPlayPhaseFinished += OnCurrentRoundPlayPhaseFinished;
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
    }
}
