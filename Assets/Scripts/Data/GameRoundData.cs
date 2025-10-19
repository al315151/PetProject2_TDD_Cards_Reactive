using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class GameRoundData : IGameRoundPrototype
    {
        public int RoundId => roundId;
        public int RoundWinnerId => roundWinnerId;

        public bool IsRoundPlayPhaseFinished => playedCardsByPlayers.Count == playersData.Count;

        public bool IsRoundFinished { get; private set; }

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
            IsRoundFinished = false;
        }

        public IGameRoundPrototype Clone(int roundId)
        {
            return new GameRoundData(roundId);
        }

        public void SetPlayerOrder(List<int> playerOrder)
        {
            this.playerOrder = playerOrder;
        }

        public int GetCurrentPlayerIdInOrder()
        {
            return playerOrder[currentPlayerInOrderIndex];
        }

        public void RemovePlayerReferences()
        {
            UnsubscribeToPlayerEvents();
        }
        
        public void ReceivePlayers(List<PlayerData> playersData)
        {
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
            for (var i = 0; i < playersData.Count; i++) {
                playersData[i].OnCardPlayed += OnCardPlayedFromPlayer;
            }
        }

        private void OnCardPlayedFromPlayer(int playerId, CardData cardData)
        {
            Debug.Log($"[Round: {roundId}] Player: {playerId} has played card: Number: {cardData.CardNumber} , Suit: {cardData.CardSuit}");
            playedCardsByPlayers.Add(playerId, cardData);

            currentPlayerInOrderIndex++;
            //Stop going through users if they have all played.
            if (currentPlayerInOrderIndex >= playersData.Count) {
                return;
            }
            RequestCardFromPlayer(GetCurrentPlayerIdInOrder());
        }

        private void UnsubscribeToPlayerEvents() 
        {
            if (playersData == null || playersData.Count == 0) {
                return;
            }

            for (var i = 0; i < playersData.Count; i++) {
                playersData[i].OnCardPlayed -= OnCardPlayedFromPlayer;
            }
        }

        private void RequestCardFromPlayer(int playerId)
        {
            for (var i = 0; i < playersData.Count; i++) {
                if (playersData[i].PlayerId == playerId) {
                    playersData[i].RequestCardFromPlayer();
                    break;
                }
            }
        }

        public int ResolveRound(CardSuit predominantCardSuit)
        {
            //Rounds can be resolved with the following rules.
            //First get the cards on the predominant suit.
            //If there is more than one, those will be resolved by their scores.
            //if there is none, the first player in order will count as the predominant suit.

            List<int> playersWithPredominantSuit = new List<int>();

            var dictionaryEnumerator = playedCardsByPlayers.GetEnumerator();

            while (dictionaryEnumerator.MoveNext()) {
                if (dictionaryEnumerator.Current.Value.CardSuit == predominantCardSuit) {
                    playersWithPredominantSuit.Add(dictionaryEnumerator.Current.Key);
                }
            }

            if (playersWithPredominantSuit.Count == 1) {
                // Only one player with it, it wins the round!
                roundWinnerId = playersWithPredominantSuit[0];
                Debug.Log($"[Round: {roundId}] Player: {roundWinnerId} Wins the Round!");
                return roundWinnerId;
            }
            if (playersWithPredominantSuit.Count == 0) {
                //No players with predominant score, we will use the first player suit as the chosen suit.
                return ResolveRound(playedCardsByPlayers[playerOrder[0]].CardSuit);
            }

            //This situation requires a check to know which player wins the round.
            //First we go by max score, then if two players are equal at score, then we go at max number.
            // Given that only players that played specific suit arrive here, all cards should be unique and thus there will be no ties.

            var maxScore = -1;
            var maxNumber = 0;

            var maxScorePlayerId = 0;
            var maxNumberPlayerId = 0;

            for (var i = 0; i < playersWithPredominantSuit.Count; i++) {
                var cardNumber = playedCardsByPlayers[playersWithPredominantSuit[i]].CardNumber;
                var cardScore = CardNumberToScoreConversionHelper.CardNumberToScoreConversion.GetValueOrDefault(cardNumber);

                if (maxScore < cardScore) {
                    maxScorePlayerId = playersWithPredominantSuit[i];
                }
                if (maxNumber < cardNumber) {
                    maxNumberPlayerId = playersWithPredominantSuit[i];
                }

                maxScore = Mathf.Max(maxScore, cardScore);
                maxNumber = Mathf.Max(maxNumber, cardNumber);
            }

            var winnerId = maxScore > 0 ? maxScorePlayerId : maxNumberPlayerId;
            roundWinnerId = winnerId;

            Debug.Log($"[Round: {roundId}] Player: {roundWinnerId} Wins the Round!");
            
            return winnerId;
        }

        public int GetTotalRoundScore()
        {
            var totalRoundScore = 0;
            var dictionaryEnumerator = playedCardsByPlayers.GetEnumerator();

            while (dictionaryEnumerator.MoveNext()) {
                totalRoundScore += CardNumberToScoreConversionHelper.CardNumberToScoreConversion.GetValueOrDefault(dictionaryEnumerator.Current.Value.CardNumber);
            }
            return totalRoundScore;
        }

        public void FinishRound(int winnerId)
        {
            var roundScore = GetTotalRoundScore();
        
            for (var i = 0; i < playersData.Count; i++) {
                if (playersData[i].PlayerId == winnerId) {
                    playersData[i].AddScoreToPlayer(roundScore);
                }
            }
            IsRoundFinished = true;
        }
    }
}
