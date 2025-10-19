using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using NUnit.Framework;
using UnityEngine.TestTools;

public class DeckValidationTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void DeckValidationTestCreateExpectedNumberOfCards()
    {
        // Number of expected cards: number of cards per each suit, from each suit.
        var numberOfExpectedCards = DeckData.NumberOfCardsPerSuit * Enum.GetValues(typeof(CardSuit)).Length;

        // Use the Assert class to test conditions
        var newDeck = new DeckData();
        newDeck.CreateDeck();

        Assert.IsTrue(newDeck.DeckCardCount == numberOfExpectedCards);
    }

    [Test]
    public void DeckValidationTestAllCardsMustBeDifferent()
    {
        var newDeck = new DeckData();
        newDeck.CreateDeck();

        var removedCards = new List<CardData>();

        var areAllCardsDifferent = true;

        while (areAllCardsDifferent && newDeck.DeckCardCount > 0) {
            var topCard = newDeck.GetTopCardFromDeck();

            for (var i = 0; i < removedCards.Count; i++) {
                var removedCard = removedCards[i];
                if (removedCard.CardNumber == topCard.CardNumber 
                    && removedCard.CardSuit == topCard.CardSuit) {
                    areAllCardsDifferent = false;
                    break;
                }
            }

            removedCards.Add(topCard);
        }
        
        Assert.IsTrue(areAllCardsDifferent);
    }

    [Test]
    public void DeckValidationTestShuffleShouldChangePositionOfCards()
    {
        var deck = new DeckData();
        deck.CreateDeck();

        //Get initial order of all cards.
        var initialCardOrder = new List<CardData>();
        while (deck.DeckCardCount > 0) {
            var topCard = deck.GetTopCardFromDeck();
            initialCardOrder.Add(topCard);
        }

        //Create deck again, and shuffle.
        deck.CreateDeck();
        deck.Shuffle();

        var isDeckShuffled = false;
        var deckDepth = 0;
        //Then check if the cards have changed place.
        while (deck.DeckCardCount > 0) {
            var topCardAfterShuffle = deck.GetTopCardFromDeck();
            if (topCardAfterShuffle.CardNumber != initialCardOrder[deckDepth].CardNumber ||
                topCardAfterShuffle.CardSuit != initialCardOrder[deckDepth].CardSuit) {
                isDeckShuffled = true;
                break;
            }
            deckDepth++;
        }

        Assert.IsTrue(isDeckShuffled);
    }

    [Test]
    public void DeckValidationTestChooseGameSuit()
    {
        var deck = new DeckData();
        deck.CreateDeck();
        deck.ChooseInitialSuit();

        CardSuit cardSuit = deck.ChosenCardSuit;

        Assert.IsNotNull(cardSuit);
    }
}
