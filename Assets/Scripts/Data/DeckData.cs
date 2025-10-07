using System;
using System.Collections.Generic;
using System.Linq;

public class DeckData
{
    public static int NumberOfCardsPerSuit = 12;

    public int DeckCardCount => deckCardData.Count;

    private List<CardData> deckCardData;

    public DeckData() {
        deckCardData = new List<CardData>();
    }

    public void CreateDeck()
    {
        deckCardData = new List<CardData>();

        var enumeratorForSuits = Enum.GetValues(typeof(CardSuit)).Cast<CardSuit>();

        for (int i = 0; i < NumberOfCardsPerSuit; i++)
        {
            foreach (var suit in enumeratorForSuits)
            {
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
        deckCardData.Remove(topCardFromDeck);
        return topCardFromDeck;
    }

    public void Shuffle()
    {
        // One card at a time, let's find the next one to add.
        var shuffledDeck = new List<CardData>();

        var randomProcess = new Random();

        var initialDeckSize = deckCardData.Count;

        for (int i = 0; i < initialDeckSize; i++)
        {
            var cardIndexToMove = randomProcess.Next(deckCardData.Count);
            var cardToShuffle = deckCardData[cardIndexToMove];
            deckCardData.RemoveAt(cardIndexToMove);
            shuffledDeck.Add(cardToShuffle);
        }
        deckCardData = shuffledDeck;
    }
}
