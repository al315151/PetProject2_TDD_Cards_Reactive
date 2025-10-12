using NUnit.Framework;
using UnityEngine;

public class GameRoundValidationTests
{
    [Test]
    public void GameRoundValidationCreateGameRound()
    {
        var numberOfCPUPlayers = 2;

        var gameManager = new GameManagerData();

        //Create the game and start it.
        //Only then Game Rounds can be created.
        gameManager.CreateGame(numberOfCPUPlayers);
        gameManager.StartGame();

        gameManager.CreateAndStartRound();

        Assert.IsTrue(gameManager.GetCurrentRoundId() == 1);
    }

    [Test]
    public void GameRoundValidationEstablishRoundOrder()
    {

        var numberOfCPUPlayers = 2;
        var gameManager = new GameManagerData();

        gameManager.CreateGame(numberOfCPUPlayers);
        gameManager.StartGame();

        gameManager.CreateAndStartRound();

        //Each player needs to play their cards.
        //For now resolving the round is not needed.
        gameManager.EstablishRoundOrder();

        //For first round, first player should be the first CPU, and the player should be the last.
        var gameRound = gameManager.GetCurrentRoundId();

        var firstPlayerInPlayOrder = gameManager.GetCurrentPlayerInOrder();
        var firstCPUId = 1;

        Assert.IsTrue(firstPlayerInPlayOrder == firstCPUId);
    }

}
