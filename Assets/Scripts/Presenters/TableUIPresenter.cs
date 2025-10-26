using System;
using Data;
using UnityEngine;
using VContainer.Unity;
using View;

namespace Presenters
{
    public class TableUIPresenter : IInitializable, IDisposable
    {
        private readonly GeneralGamePresenter gameManagerPresenter;
        private readonly GameManagerData gameManagerData;
        private readonly TableUIView tableUIView;

        public CardSuit SelectedCardSuit => selectedSuit;
        
        private CardSuit selectedSuit;
        
        public TableUIPresenter(
            GeneralGamePresenter gameManagerPresenter,
            GameManagerData gameManagerData,
            TableUIView tableUIView)
        {
            this.gameManagerPresenter = gameManagerPresenter;
            this.gameManagerData = gameManagerData;
            this.tableUIView = tableUIView;
        }

        public void Initialize()
        {
            gameManagerPresenter.OnGameStarted += OnGameStarted;
            tableUIView.RequestDeckCardCountUpdate += OnRequestDeckCardCountUpdate;
        }

        private void OnRequestDeckCardCountUpdate()
        {
            tableUIView.SetCardsLeftInDeck(gameManagerData.CurrentDeckSize());
        }

        private void OnGameStarted(CardSuit initialCardSuit)
        {
            tableUIView.SetupSelectedCardSuitVisuals(initialCardSuit);
        }

        public void Dispose()
        {
            gameManagerPresenter.OnGameStarted -= OnGameStarted;
            tableUIView.RequestDeckCardCountUpdate -= OnRequestDeckCardCountUpdate;
        }

        private void SubscribeToRoundRelatedData()
        {

        }
    }
}
