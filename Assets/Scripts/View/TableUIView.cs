using System;
using System.Collections.Generic;
using Data;
using Providers;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace View
{
    public class TableUIView : MonoBehaviour
    {
        private CardSuitSpriteProvider cardSuitSpriteProvider;
        
        [SerializeField]
        private Image selectedCardSuitImage;

        [SerializeField]
        private TMP_Text cardsLeftInDeckText;
        
        [SerializeField]
        private List<TMP_Text> npcPlayersScoresText;

        [SerializeField]
        private List<TMP_Text> npcPlayersRoundOrderText;

        [SerializeField]
        private List<CardView> cardViews;

        public Action RequestDeckCardCountUpdate;

        [Inject]
        public void Inject(CardSuitSpriteProvider cardSuitProvider)
        {
            cardSuitSpriteProvider = cardSuitProvider;
            
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

        public void SetNPCPlayerScores(List<KeyValuePair<int, int>> playerScores)
        {
            var playerScoreFormat = "Player {0} score: {1}";

            for (int i = 0; i < playerScores.Count; i++)
            {
                var playerId = playerScores[i].Key.ToString();
                if (playerScores[i].Key == -1)
                {
                    playerId = "You";
                }
                npcPlayersScoresText[i].text = string.Format(playerScoreFormat, playerId, playerScores[i].Value);
            }
        }

        public void SetPlayerRoundOrderText(List<int> playerOrder)
        {
            var playerOrderFormat = "Player {0}";
            for (int i = 0;i < playerOrder.Count;i++)
            {
                var playerId = playerOrder[i].ToString();
                if (playerOrder[i] == -1)
                {
                    playerId = "You";
                }

                npcPlayersRoundOrderText[i].text = string.Format(playerOrderFormat, playerId);
            }
        }

        public void SetupRoundCardsView(List<CardData> playedCards)
        {
            for (int i = 0; i < playedCards.Count; i++)
            {
                cardViews[i].SetupCardGraphics(playedCards[i].CardSuit, playedCards[i].CardNumber);
            }
        }

        public void ResetCardViews()
        {
            for (int i = 0; i < cardViews.Count; i++)
            {
                cardViews[i].ResetCardGraphics();
            }
        }
    }
}
