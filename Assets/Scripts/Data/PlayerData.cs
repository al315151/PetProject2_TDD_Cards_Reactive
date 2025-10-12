using System;
using System.Collections.Generic;

public class PlayerData : IPlayerPrototype
{
    public static int MaxHandSize = 3;

    public int PlayerHandSize => playerHand.Count;
    public int PlayerId => id;

    private int id;
    private List<CardData> playerHand;

    public PlayerData(int id = 0)
    {
        this.id = id;
        playerHand = new List<CardData>();
    }

    public PlayerData(PlayerData copy)
    {
        id = copy.id;
        playerHand = new List<CardData>();
    }

    public void SetupId(int id)
    {
        this.id = id;
    }

    public void AddCardToHandFromDeck(DeckData deck)
    {
        if (playerHand.Count >= MaxHandSize)
        {
            return;
        }
        playerHand.Add(deck.GetTopCardFromDeck());
    }

    public IPlayerPrototype Clone()
    {
        return new PlayerData(this);
    }
}
