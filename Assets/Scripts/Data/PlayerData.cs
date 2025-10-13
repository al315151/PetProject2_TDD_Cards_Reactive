using System;
using System.Collections.Generic;

public class PlayerData : IPlayerPrototype
{
    public static int MaxHandSize = 3;

    public Action<int, CardData> OnCardPlayed;

    public int PlayerHandSize => playerHand.Count;
    public int PlayerId => id;

    public bool PlayedCardForTheRound { get; set; }

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

    public IPlayerPrototype Clone(int id)
    {
        return new PlayerData(id);
    }

    public void RequestCardFromPlayer()
    {
        //For now, choose card at random.
        var randomIndex = new Random().Next(playerHand.Count);
        var randomCard = playerHand[randomIndex];
        OnCardPlayed?.Invoke(id, randomCard);
    }
}
