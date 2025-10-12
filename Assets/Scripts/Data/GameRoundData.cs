using System.Collections.Generic;

public class GameRoundData : IGameRoundPrototype
{
    public int RoundId => roundId;
    public int RoundWinnerId => roundWinnerId;

    private readonly int roundId;

    private List<int> playerOrder;

    private int roundWinnerId;
    private int currentPlayerInOrderIndex;

    public GameRoundData(int roundId)
    {
        this.roundId = roundId;
        playerOrder = new List<int>();
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
}
