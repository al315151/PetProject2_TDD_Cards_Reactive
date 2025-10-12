using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

public class GameManagerValidationTests
{
    [Test]
    [TestCase(3)]
    [TestCase(7)]
    [TestCase(0)]
    public void GameManagerTestCreateGameWithSpecifiedPlayers(int numberOfCPUPlayers)
    {        
        var gameManager = new GameManagerData();
        gameManager.CreateGame(numberOfCPUPlayers);

        Assert.IsTrue(gameManager.NumberOfPlayers == numberOfCPUPlayers + 1);
    }

    [Test]
    public void GameManagerTestStartGame()
    {
        var numberOfCPUPlayers = 2;
        var gameManager = new GameManagerData();
        gameManager.CreateGame(numberOfCPUPlayers);

        var playerMaxHandCount = PlayerData.MaxHandSize;
        var numberOfExpectedCards = DeckData.NumberOfCardsPerSuit * Enum.GetValues(typeof(CardSuit)).Length;

        // deck created, initial cards dealt, check remaining cards on deck to know if initial card deal is correct.
        gameManager.StartGame();

        var currentDeckSize = gameManager.CurrentDeckSize();

        Assert.IsTrue(currentDeckSize == numberOfExpectedCards - (playerMaxHandCount * gameManager.NumberOfPlayers));
    }

}
