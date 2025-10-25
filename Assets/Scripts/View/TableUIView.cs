using System;
using System.Collections.Generic;
using Data;
using Providers;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class TableUIView : MonoBehaviour
    {
        private readonly CardSuitSpriteProvider cardSuitSpriteProvider;
        [SerializeField]
        private Image selectedCardSuitImage;

        [SerializeField]
        private TMP_Text cardsLeftInDeckText;
        
        [SerializeField]
        private List<TMP_Text> npcPlayersScoresText;
        
        [SerializeField]
        private List<CardView> cardViews;

        public Action RequestDeckCardCountUpdate;
        
        public TableUIView(CardSuitSpriteProvider cardSuitSpriteProvider)
        {
            this.cardSuitSpriteProvider = cardSuitSpriteProvider;
            var everySecondCheck = Observable.Interval(TimeSpan.FromSeconds(1f), destroyCancellationToken);
            everySecondCheck.Subscribe(x => RequestDeckCardCountUpdate?.Invoke());
        }
        
        public void SetupSelectedCardSuitVisuals(CardSuit cardSuit)
        {
            selectedCardSuitImage.sprite = cardSuitSpriteProvider.GetCardSuitSprite(cardSuit);
        }

        public void SetCardsLeftInDeck(int cardsLeftInDeck)
        {
            cardsLeftInDeckText.text = cardsLeftInDeck.ToString();
        }
    }
}
