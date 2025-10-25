using System;
using Data;
using UnityEngine;
using VContainer.Unity;

namespace Presenters
{
    public class TableUIPresenter : IInitializable, IDisposable
    {
        private readonly GeneralGamePresenter gameManagerPresenter;

        public CardSuit SelectedCardSuit => selectedSuit;
        
        private CardSuit selectedSuit;
        
        public TableUIPresenter(GeneralGamePresenter gameManagerPresenter)
        {
            this.gameManagerPresenter = gameManagerPresenter;
        }

        public void Initialize()
        {
            gameManagerPresenter.OnGameStarted += OnGameStarted;
        }

        private void OnGameStarted(CardSuit initialCardSuit)
        {
            // Store (bc other data should be retrieved from gamePresenter) and setup cardSuit on view.
            selectedSuit = initialCardSuit;
        }

        public void Dispose()
        {
            gameManagerPresenter.OnGameStarted -= OnGameStarted;
        }
    }
}
