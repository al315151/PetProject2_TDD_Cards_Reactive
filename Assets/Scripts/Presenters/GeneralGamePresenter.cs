using System;
using Data;
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

        public void Initialize()
        {
            gameManagerData.CreateGame();

            gameView.NewGameButtonClicked += StartGameButtonPressed;
        }

        private void OnPlayersInitialized()
        {
            gameManagerData.ReceivePlayersData(playersService.GetAllPlayers());
        }

        public void Dispose()
        {
            gameView.NewGameButtonClicked -= StartGameButtonPressed;
            playersService.OnPlayersInitialized -= OnPlayersInitialized;
        }
    }
}
