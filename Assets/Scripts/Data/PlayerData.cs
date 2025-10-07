using System;
using System.Collections.Generic;

public class PlayerData
{
    public static int MaxHandSize = 3;

    public int PlayerHandSize => playerHand.Count;

    private readonly int id;
    private List<CardData> playerHand;

    public PlayerData(int id = 0)
    {
        this.id = id;
        playerHand = new List<CardData>();
    }

    public void AddCardToHandFromDeck(DeckData deck)
    {
        if (playerHand.Count >= MaxHandSize)
        {
            return;
        }
        playerHand.Add(deck.GetTopCardFromDeck());
    }
}
