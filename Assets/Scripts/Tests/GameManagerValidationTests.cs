using NUnit.Framework;
using UnityEngine;

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
}
