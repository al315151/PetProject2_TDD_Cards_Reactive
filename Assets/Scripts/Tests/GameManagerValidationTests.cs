using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
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
    public void GameManagerValidation_StartGame_PlayersReceiveIntendedInitialCards()
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

    [Test]
    public async Task GameManagerValidation_PlayOneRound_RoundLifecycleIsCorrect()
    {
        var numberOfCPUPlayers = 2;
        var gameManager = new GameManagerData();
        gameManager.CreateGame(numberOfCPUPlayers);

        var players = gameManager.GetPlayers();

        gameManager.StartGame();

        await PlayOneRound(gameManager);
        
        var currentRound = gameManager.GetCurrentRound();

        //Make sure the winner player has gotten the score.

        var roundWinner = GetRoundWinner(players, currentRound);
        var roundScore = currentRound.GetTotalRoundScore();
        
        Assert.IsTrue(roundWinner.GetScore() == roundScore);
    }

    [Test]
    public async Task GameManagerValidation_PlayTwoRounds_RoundsAreDifferent()
    {
        var numberOfCPUPlayers = 2;
        var gameManager = new GameManagerData();
        gameManager.CreateGame(numberOfCPUPlayers);

        gameManager.StartGame();

        var firstRoundFinished = await PlayOneRound(gameManager);
        
        var firstRound = gameManager.GetCurrentRound();

        var secondRoundFinished = await PlayOneRound(gameManager);

        var secondRound = gameManager.GetCurrentRound();

        Assert.IsTrue(firstRoundFinished && secondRoundFinished && firstRound.RoundId != secondRound.RoundId);
    }

    [Test]
    public async Task GameManagerValidation_PlayFourRounds_PlayersHaveProperAmountOfCards()
    {
        var numberOfCPUPlayers = 2;
        var gameManager = new GameManagerData();
        gameManager.CreateGame(numberOfCPUPlayers);

        gameManager.StartGame();

        var players = gameManager.GetPlayers();
        
        var firstRoundFinished = await PlayOneRound(gameManager);
        var firstRound = gameManager.GetCurrentRound();

        var secondRoundFinished = await PlayOneRound(gameManager);
        var secondRound = gameManager.GetCurrentRound();
        
        var thirdRoundFinished = await PlayOneRound(gameManager);
        var thirdRound = gameManager.GetCurrentRound();
        
        var fourthRoundFinished = await PlayOneRound(gameManager);
        var fourthRound = gameManager.GetCurrentRound();

        var allRoundsFinished = firstRoundFinished && secondRoundFinished && thirdRoundFinished && fourthRoundFinished;
        var allRoundsIdAreDifferent = firstRound.RoundId != secondRound.RoundId 
                                      && firstRound.RoundId != thirdRound.RoundId 
                                      && firstRound.RoundId != fourthRound.RoundId;

        var allPlayersHaveFullHand = true;

        foreach (var player in players) {
            // Hand is refilled at the start of the round.
            if (player.PlayerHandSize == PlayerData.MaxHandSize - 1) {
                continue;
            }
            allPlayersHaveFullHand = false;
            break;
        }
        
        Assert.IsTrue(allRoundsFinished && allRoundsIdAreDifferent && allPlayersHaveFullHand);
    }
    
    private async Task<bool> PlayOneRound(GameManagerData gameManager)
    {
        gameManager.StartPlayRound();
        
        var currentRound = gameManager.GetCurrentRound();
        var playPhaseFinished = await WaitForRoundPlayPhaseToBeFinished(currentRound);

        gameManager.FinishRound();

        var roundFinished = await WaitForRoundToBeFinished(currentRound);

        return playPhaseFinished && roundFinished;
    }
    
    private async Task<bool> WaitForRoundToBeFinished(GameRoundData round)
    {
        var timeoutLengthInSeconds = 3;
        var timeoutStartTime = DateTime.Now;

        while (round.IsRoundFinished == false) {
            if ((DateTime.Now - timeoutStartTime).TotalSeconds > timeoutLengthInSeconds) {
                break;
            }
            await Task.Delay(100);
        }

        return round.IsRoundFinished;
    }

    private async Task<bool> WaitForRoundPlayPhaseToBeFinished(GameRoundData round)
    {
        var timeoutLengthInSeconds = 3;
        var timeoutStartTime = DateTime.Now;

        while (round.IsRoundPlayPhaseFinished == false) {
            if ((DateTime.Now - timeoutStartTime).TotalSeconds > timeoutLengthInSeconds) {
                break;
            }
            await Task.Delay(100);
        }

        return round.IsRoundPlayPhaseFinished;
    }

    private PlayerData GetRoundWinner(List<PlayerData> players, GameRoundData round)
    {
        var winnerId = round.RoundWinnerId;
        PlayerData roundWinner = null;

        for (var i = 0; i < players.Count; i++) {
            if (players[i].PlayerId == winnerId) {
                roundWinner = players[i];
                break;
            }
        }
        return roundWinner;
    }

}
