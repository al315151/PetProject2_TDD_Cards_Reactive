using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
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

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator DeckValidationTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
