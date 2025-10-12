using NUnit.Framework;
using UnityEngine;

public class GameRoundValidationTests
{
    private void SetupGameManagerAndStartGame(out GameManagerData gameManager)
    {
        var numberOfCPUPlayers = 2;

        gameManager = new GameManagerData();

        //Create the game and start it.
        //Only then Game Rounds can be created.
        gameManager.CreateGame(numberOfCPUPlayers);
        gameManager.StartGame();
    }

    [Test]
    public void GameRoundValidationCreateGameRound()
    {
        SetupGameManagerAndStartGame(out GameManagerData gameManager);

        gameManager.CreateAndStartRound();

        Assert.IsTrue(gameManager.GetCurrentRoundId() == 1);
    }

    [Test]
    public void GameRoundValidationEstablishRoundOrder()
    {
        SetupGameManagerAndStartGame(out GameManagerData gameManager);

        gameManager.CreateAndStartRound();

        //Each player needs to play their cards.
        //For now resolving the round is not needed.
        gameManager.EstablishRoundOrder();

        var firstPlayerInPlayOrder = gameManager.GetCurrentPlayerInOrder();
        var firstCPUId = 1;

        Assert.IsTrue(firstPlayerInPlayOrder == firstCPUId);
    }

    [Test]
    public void GameRoundValidationPlayRound()
    {
        SetupGameManagerAndStartGame(out GameManagerData gameManager);

        gameManager.CreateAndStartRound();
        gameManager.EstablishRoundOrder();

        //Number of players: 2 + player.


    }

}
