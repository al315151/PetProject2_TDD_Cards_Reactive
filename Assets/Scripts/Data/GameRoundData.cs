using System.Collections.Generic;

public class GameRoundData : IGameRoundPrototype
{
    public int RoundId => roundId;
    public int RoundWinnerId => roundWinnerId;

    public bool IsRoundPlayPhaseFinished => playedCardsByPlayers.Count == playersData.Count;

    private readonly int roundId;

    private List<int> playerOrder;

    private List<PlayerData> playersData;

    private List<KeyValuePair<int, CardData>> playedCardsByPlayers;

    private int roundWinnerId;
    private int currentPlayerInOrderIndex;

    public GameRoundData(int roundId)
    {
        this.roundId = roundId;
        playerOrder = new List<int>();
        playedCardsByPlayers = new List<KeyValuePair<int, CardData>>();
    }

    public IGameRoundPrototype Clone(int roundId)
    {
        return new GameRoundData(roundId);
    }

    public void SetPlayerOrder(List<int> playerOrder)
    {
        this.playerOrder = playerOrder;
    }

    public void SetupRoundWinner(int roundWinnerId)
    {
        this.roundWinnerId = roundWinnerId;
    }

    public int GetCurrentPlayerIdInOrder()
    {
        return playerOrder[currentPlayerInOrderIndex];
    }

    public void ReceivePlayersFromRound(List<PlayerData> playersData)
    {
        UnsubscribeToPlayerEvents();

        this.playersData = playersData;

        SubscribeToPlayerEvents();
    }

    public void StartPlayPhase()
    {
        // For each player, ask them to play their cards. 
        RequestCardFromPlayer(GetCurrentPlayerIdInOrder());
    }

    private void SubscribeToPlayerEvents()
    {
        for(int i = 0;i < playersData.Count;i++)
        {
            playersData[i].OnCardPlayed += OnCardPlayedFromPlayer;
        }
    }

    private void OnCardPlayedFromPlayer(int playerId, CardData cardData)
    {
        playedCardsByPlayers.Add(new KeyValuePair<int, CardData>(playerId, cardData));

        currentPlayerInOrderIndex++;
        //Stop going through users if they have all played.
        if (currentPlayerInOrderIndex >= playersData.Count)
        {
            return;
        }
        RequestCardFromPlayer(GetCurrentPlayerIdInOrder());
    }

    private void UnsubscribeToPlayerEvents() 
    {
        if (playersData == null || playersData.Count == 0)
        {
            return;
        }

        for (int i = 0; i < playersData.Count; i++)
        {
            playersData[i].OnCardPlayed -= OnCardPlayedFromPlayer;
        }
    }

    private void RequestCardFromPlayer(int playerId)
    {
        var currentPlayerId = GetCurrentPlayerIdInOrder();
        for (int i = 0; i < playersData.Count; i++)
        {
            if (playersData[i].PlayerId == playerId)
            {
                playersData[i].RequestCardFromPlayer();
                break;
            }
        }
    }
}
