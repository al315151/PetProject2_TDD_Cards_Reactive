using System;
using System.Collections.Generic;
using Data;
using R3;
using UnityEngine;

namespace Presenters
{
    public class GameRoundPresenter : IGameRoundPrototype, IObserver<KeyValuePair<int, CardData>>
    {
        public Action PlayPhaseFinished;
        
        public GameRoundData GameRoundData => gameRoundData;

        public bool IsRoundPlayPhaseFinished => gameRoundData.IsRoundPlayPhaseFinished();

        public bool IsRoundFinished { get; private set; }

        private readonly GameRoundData gameRoundData;
        
        private IDisposable currentPlayerInOrderDisposable;

        private int roundId;
        
        public GameRoundPresenter(int roundId)
        {
            this.roundId = roundId;
            gameRoundData = new GameRoundData(roundId);

            IsRoundFinished = false;
        }

        public IGameRoundPrototype Clone(int roundId)
        {
            return new GameRoundPresenter(roundId);
        }

        public void SetPlayerOrder(List<int> playerOrder)
        {
            gameRoundData.SetPlayerOrder(playerOrder);
        }

        public int GetCurrentPlayerIdInOrder()
        {
            return gameRoundData.GetCurrentPlayerIdInOrder();
        }

        public void ReceivePlayers(List<PlayerData> playersData)
        {
            gameRoundData.SetupPlayers(playersData);
        }

        public int GetCurrentRoundId()
        {
            return roundId;
        }

        public void StartPlayPhase()
        {
            var PlayedCardsByPlayers = gameRoundData.PlayedCardsInRound;
            PlayedCardsByPlayers.Value = new List<CardData>();
            PlayedCardsByPlayers.OnNext(PlayedCardsByPlayers.Value);
            // For each player, ask them to play their cards. 
            RequestCardFromPlayer(GetCurrentPlayerIdInOrder());
        }

        private void OnCardPlayedFromPlayer(int playerId, CardData cardData)
        {
            Debug.Log($"[Round: {gameRoundData.RoundId}] Player: {playerId} has played card: Number: {cardData.CardNumber} , Suit: {cardData.CardSuit}");

            var PlayedCardsByPlayers = gameRoundData.PlayedCardsInRound;

            gameRoundData.AddPlayerCardPlayed(playerId, cardData);

            PlayedCardsByPlayers.Value.Add(cardData);
            PlayedCardsByPlayers.OnNext(PlayedCardsByPlayers.Value);

            gameRoundData.IncreaseCurrentPlayerOrder();
            //Stop going through users if they have all played.
            if (gameRoundData.IsRoundPlayPhaseFinished()) {
                PlayPhaseFinished?.Invoke();
                return;
            }
            RequestCardFromPlayer(GetCurrentPlayerIdInOrder());
        }

        private void UnsubscribeToPlayerEvents() 
        {
            if (gameRoundData.ArePlayersEmpty()) {
                return;
            }
            currentPlayerInOrderDisposable?.Dispose();
        }

        private void RequestCardFromPlayer(int playerId)
        {
            var playersData = gameRoundData.PlayersData;
            for (var i = 0; i < playersData.Count; i++) {
                if (playersData[i].PlayerId != playerId) {
                    continue;
                }
                currentPlayerInOrderDisposable = playersData[i].Subscribe(this);
                break;
            }
        }

        public int ResolveRound(CardSuit predominantCardSuit)
        {
            //Rounds can be resolved with the following rules.
            //First get the cards on the predominant suit.
            //If there is more than one, those will be resolved by their scores.
            //if there is none, the first player in order will count as the predominant suit.

            List<int> playersWithPredominantSuit = new List<int>();

            var playedCardsByPlayers = gameRoundData.PlayedCardsByPlayers;
            var dictionaryEnumerator = playedCardsByPlayers.GetEnumerator();
            
            while (dictionaryEnumerator.MoveNext()) {
                if (dictionaryEnumerator.Current.Value.CardSuit == predominantCardSuit) {
                    playersWithPredominantSuit.Add(dictionaryEnumerator.Current.Key);
                }
            }

            if (playersWithPredominantSuit.Count == 1) {
                // Only one player with it, it wins the round!
                gameRoundData.SetRoundWinnerId(playersWithPredominantSuit[0]);
                return gameRoundData.RoundWinnerId;
            }
            if (playersWithPredominantSuit.Count == 0) {
                //No players with predominant score, we will use the first player suit as the chosen suit.
                return ResolveRound(playedCardsByPlayers[gameRoundData.PlayerOrder[0]].CardSuit);
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
            gameRoundData.SetRoundWinnerId(winnerId);
            
            return winnerId;
        }

        public int GetTotalRoundScore()
        {
            var totalRoundScore = 0;
            var dictionaryEnumerator = gameRoundData.PlayedCardsByPlayers.GetEnumerator();

            while (dictionaryEnumerator.MoveNext()) {
                totalRoundScore += CardNumberToScoreConversionHelper.CardNumberToScoreConversion.GetValueOrDefault(dictionaryEnumerator.Current.Value.CardNumber);
            }
            return totalRoundScore;
        }

        public void FinishRound(int winnerId)
        {
            var roundScore = GetTotalRoundScore();
            var playersData = gameRoundData.PlayersData;

            for (var i = 0; i < playersData.Count; i++) {
                if (playersData[i].PlayerId == winnerId) {
                    playersData[i].AddScoreToPlayer(roundScore);
                }
            }
            IsRoundFinished = true;
        }

        public void StartPlayerDrawPhase(DeckData deckData)
        {
            var playersData = gameRoundData.PlayersData;
            for (var i = 0; i < playersData.Count; i++) {
                if (playersData[i].PlayerHandSize >= PlayerData.MaxHandSize) {
                    continue;
                }
                playersData[i].AddCardToHandFromDeck(deckData);
            }
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<int, CardData> playerAndCard)
        {
            UnsubscribeToPlayerEvents();
            OnCardPlayedFromPlayer(playerAndCard.Key, playerAndCard.Value);
        }
    }
}
