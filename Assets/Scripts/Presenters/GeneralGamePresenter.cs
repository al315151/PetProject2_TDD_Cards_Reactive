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
            gameManagerData.StartGame();
            OnGameStarted?.Invoke(gameManagerData.DeckInitialCardSuit);
        }

        public void StartGameRound()
        {
            gameManagerData.StartPlayRound();
        }

        public void Initialize()
        {
            gameView.NewGameButtonClicked += StartGameButtonPressed;
            SubscribeOnGameManagerDataStats();

            gameManagerData.CreateGame();
        }

        private void OnPlayersInitialized()
        {
            gameManagerData.ReceivePlayersData(playersService.GetAllPlayers());
        }

        public new void Dispose()
        {
            gameView.NewGameButtonClicked -= StartGameButtonPressed;
            playersService.OnPlayersInitialized -= OnPlayersInitialized;
            currentRoundIndexDisposable?.Dispose();
        }

        private void SubscribeOnGameManagerDataStats()
        {
            currentRoundIndexDisposable = gameManagerData.CurrentRoundIndex.Subscribe( onNext: roundIndex => gameView.SetRoundNumber(roundIndex.ToString()));
        }
    }
}
