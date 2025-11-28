using System.Collections.Generic;
using R3;

namespace Data
{
    public class GameRoundData
    {
        public int RoundId => roundId;
        public int RoundWinnerId => roundWinnerId;
        public List<int> PlayerOrder => playerOrder;
        public int CurrentPlayerOrderIndex => currentPlayerInOrderIndex;
        public ReactiveProperty<List<CardData>> PlayedCardsInRound;
        public Dictionary<int, CardData> PlayedCardsByPlayers => playedCardsByPlayers;

        public List<PlayerData> PlayersData => playersData;

        private readonly int roundId;

        private List<int> playerOrder;

        private List<PlayerData> playersData;

        private readonly Dictionary<int, CardData> playedCardsByPlayers;

        private int roundWinnerId;
        private int currentPlayerInOrderIndex;

        public GameRoundData(int roundId)
        {
            this.roundId = roundId;
            playerOrder = new List<int>();
            playedCardsByPlayers = new Dictionary<int, CardData>();
            PlayedCardsInRound = new ReactiveProperty<List<CardData>>(new List<CardData>());
        }

        public void SetPlayerOrder(List<int> playerOrder)
        {
            this.playerOrder = playerOrder;
        }

        public void SetupPlayers(List<PlayerData> playersData)
        {
            this.playersData = playersData;
        }

        public void SetRoundWinnerId(int roundWinnerId)
        {
            this.roundWinnerId = roundWinnerId;
        }

        public int GetCurrentPlayerIdInOrder()
        {
            return playerOrder[currentPlayerInOrderIndex];
        }

        public bool IsRoundPlayPhaseFinished()
        {
            return playedCardsByPlayers.Count >= playersData.Count;
        }

        public bool ArePlayersEmpty()
        {
            return playersData == null || playersData.Count == 0;
        }

        public void AddPlayerCardPlayed(int playerId, CardData cardData)
        {
            playedCardsByPlayers.Add(playerId, cardData);
        }

        public void IncreaseCurrentPlayerOrder()
        {
            currentPlayerInOrderIndex++;
        }
    }
}