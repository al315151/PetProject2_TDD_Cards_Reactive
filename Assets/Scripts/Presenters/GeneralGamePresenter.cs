using System;
using Data;
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

        public GameManagerData TestOnlyGameManagerData => gameManagerData;
        
        public GeneralGamePresenter(
            GameManagerData gameManagerData,
            GeneralGameView gameView)
        {
            this.gameManagerData = gameManagerData;
            this.gameView = gameView;
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

        public void Dispose()
        {
            gameView.NewGameButtonClicked -= StartGameButtonPressed;
        }
    }
}
