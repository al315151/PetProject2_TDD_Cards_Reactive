using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Data
{
    public class DeckData
    {
        public static int NumberOfCardsPerSuit = 12;

        public int DeckCardCount => deckCardData.Count;
        public CardSuit ChosenCardSuit => chosenCardSuit;

        private List<CardData> deckCardData = new();
        private CardSuit chosenCardSuit;

        public void CreateDeck()
        {
            deckCardData = new List<CardData>();

            var enumeratorForSuits = Enum.GetValues(typeof(CardSuit)).Cast<CardSuit>().ToArray();

            for (var i = 0; i < NumberOfCardsPerSuit; i++) {
                foreach (var suit in enumeratorForSuits) {
                    //Cards should go from 1 to 12.
                    var newCard = new CardData(suit, i + 1);
                    deckCardData.Add(newCard);
                }
            }
        }

        public CardData GetTopCardFromDeck()
        {
            //Get card and remove it.
            var topCardFromDeck = deckCardData.FirstOrDefault();
            if (topCardFromDeck != null) {
                deckCardData.Remove(topCardFromDeck);
            }
            return topCardFromDeck;
        }

        public void Shuffle()
        {
            // One card at a time, let's find the next one to add.
            var shuffledDeck = new List<CardData>();

            var randomProcess = new Random();

            var initialDeckSize = deckCardData.Count;

            for (var i = 0; i < initialDeckSize; i++) {
                var cardIndexToMove = randomProcess.Next(deckCardData.Count);
                var cardToShuffle = deckCardData[cardIndexToMove];
                deckCardData.RemoveAt(cardIndexToMove);
                shuffledDeck.Add(cardToShuffle);
            }
            deckCardData = shuffledDeck;
        }

        public void ChooseInitialSuit()
        {
            //Suit is chosen by grabbing initial card and storing its suit.        
            var firstCard = GetTopCardFromDeck();
            chosenCardSuit = firstCard.CardSuit;

            Debug.Log($"[Deck] Chosen Suit is: {chosenCardSuit}");
            
            // After being chosen, it will be placed onto the bottom of the deck.
            deckCardData.Add(firstCard);
        }
    }
}
