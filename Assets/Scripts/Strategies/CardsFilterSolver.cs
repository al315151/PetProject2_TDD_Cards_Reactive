using System.Collections.Generic;
using UnityEngine;

namespace Strategies
{
    public static class CardsFilterSolver
    {
        public static List<CardData> FilterCardsFromSpecifiedSuit(CardSuit cardSuit, List<CardData> cardsToFilter)
        {
            var result = new List<CardData>();

            foreach (var card in cardsToFilter) {
                if (card.CardSuit == cardSuit) {
                    result.Add(card);
                }
            }
            return result;
        }

        public static CardData GetWinnerCardFromSameSuitCards(List<CardData> cardsToFilter)
        {
            //Filtered cards should be more than 1 by this point.
            var maxScore = -1;
            var maxNumber = 0;

            var maxScoreCard = cardsToFilter[0];
            var maxNumberCard = cardsToFilter[0];

            foreach (var card in cardsToFilter) {
                var cardNumber = card.CardNumber;
                var cardScore =
                    CardNumberToScoreConversionHelper.CardNumberToScoreConversion.GetValueOrDefault(cardNumber);

                maxScore = Mathf.Max(maxScore, cardScore);

                if (maxScore == cardScore) {
                    maxScoreCard = card;
                }

                maxNumber = Mathf.Max(maxNumber, cardNumber);
                if (maxNumber == cardNumber) {
                    maxNumberCard = card;
                }
            }
            
            return maxScore > 0 ? maxScoreCard : maxNumberCard;
        }

        public static CardData GetBestCardFromProvidedCards(List<CardData> cardsToFilter, CardSuit predominantCardSuit)
        {
            var cardsOfPredominantSuit = FilterCardsFromSpecifiedSuit(predominantCardSuit, cardsToFilter);

            if (cardsOfPredominantSuit.Count == 0) {
                return GetWinnerCardFromSameSuitCards(cardsToFilter);
            }

            return GetWinnerCardFromSameSuitCards(cardsOfPredominantSuit);
        }
    }
}
