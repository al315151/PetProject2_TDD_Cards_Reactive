using System;
using System.Collections.Generic;
using System.Threading;

public class GameManagerData
{
    public int NumberOfPlayers => playersData.Count;

    private List<PlayerData> playersData;
    private DeckData deckData;

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
}
