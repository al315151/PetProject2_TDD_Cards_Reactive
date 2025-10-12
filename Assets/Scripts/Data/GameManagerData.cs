using System;
using System.Collections.Generic;
using System.Threading;

public class GameManagerData
{
    public int NumberOfPlayers => playersData.Count;

    private List<PlayerData> playersData;
    private DeckData deckData;
    private int currentRound;

    private IGameRoundPrototype gameRoundConcretePrototype;

    private GameRoundData currentGameRound;
    private List<GameRoundData> roundDataHistory;

    public void CreateGame(int numberOfCPUPlayers)
    {
        playersData = new List<PlayerData>();

        var basePlayer = new PlayerData();
        playersData.Add(basePlayer);

        for (int i = 0; i < numberOfCPUPlayers; i++)
        {
            var newCPU = basePlayer.Clone() as PlayerData;
            newCPU.SetupId(playersData.Count);
            playersData.Add(newCPU);
        }

        deckData = new DeckData();
        deckData.CreateDeck();

        gameRoundConcretePrototype = new GameRoundData(currentRound);
        roundDataHistory = new List<GameRoundData>();
    }

    public void StartGame()
    {
        //Shuffle the cards, set the deck chosen Suit.
        deckData.Shuffle();
        deckData.ChooseInitialSuit();

        DrawInitialHandForPlayers();

    }

    public int CurrentDeckSize()
    {
        return deckData.DeckCardCount;
    }

    private void DrawInitialHandForPlayers()
    {
        var initialCardCountForPlayer = PlayerData.MaxHandSize;
        for (int i = 0;i < playersData.Count;i++)
        {
           var player = playersData[i];
           for (int j = 0; j < initialCardCountForPlayer; j++)
           {
                player.AddCardToHandFromDeck(deckData);
           }
        }
    }

    public int GetCurrentRoundId()
    {
        return currentGameRound.RoundId;
    }

    public int GetCurrentPlayerInOrder()
    {
        return currentGameRound.GetCurrentPlayerIdInOrder();
    }

    public void CreateAndStartRound()
    {
        currentRound++;
        var gameRoundPrototype = gameRoundConcretePrototype.Clone(currentRound);
        if (currentGameRound != null)
        {
            roundDataHistory.Add(currentGameRound);
        }
        currentGameRound = gameRoundPrototype as GameRoundData;
    }

    public void EstablishRoundOrder()
    {
        // Winner of last round will start.
        // Otherwise, player will go last. 
        var playerOrder = new List<int>();
        var startingIndex = 1;
        if (currentGameRound.RoundId <= 1)
        {
            startingIndex = 1;
        }
        else
        {
            var previousRoundWinner = roundDataHistory[roundDataHistory.Count - 1].RoundWinnerId;
            startingIndex = 0;
            for (int i = 0; i < playersData.Count; i++)
            {
                if (playersData[i].PlayerId == previousRoundWinner)
                {
                    startingIndex = i;
                    break;
                }
            }
        }

        for (int i = 0; i < playersData.Count; i++)
        {
            if (startingIndex == playersData.Count)
            {
                startingIndex = 0;
            }

            playerOrder.Add(playersData[startingIndex].PlayerId);
            startingIndex++;
        }

        currentGameRound.SetPlayerOrder(playerOrder);
    }
}
