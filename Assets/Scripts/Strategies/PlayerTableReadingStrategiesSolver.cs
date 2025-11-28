using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;

namespace Strategies
{
    public class PlayerTableReadingStrategiesSolver : IPlayerStrategy
    {
        private PlayerData playerData;
        private GameRoundData gameRoundData;
        private CardSuit predominantCardSuit;

        private PlayerStrategyType playerStrategyType;
        
        public void SetupAdditionalData(
            PlayerStrategyType playerStrategyType,
            GameRoundData gameRoundData,
            CardSuit predominantCardSuit)
        {
            this.playerStrategyType = playerStrategyType;
            this.gameRoundData = gameRoundData;
            this.predominantCardSuit = predominantCardSuit;
        }

        public CardData ExecuteStrategy()
        { 
            return ExecuteStrategyThroughType(playerStrategyType);
        }

        public void SetupPlayerData(PlayerData playerData)
        {
            this.playerData = playerData;
        }

        private CardData ExecuteStrategyThroughType(PlayerStrategyType playerStrategyType)
        {
            switch (playerStrategyType) {
                case PlayerStrategyType.RoundReading_MaxRoundWins_UsePredominantSuit:
                    return ExecuteStrategy_MaxRoundWins_UsePredominantSuit();
                default:
                    Debug.Log($"Strategy: {playerStrategyType} Not supported by: {this} , assign different strategy to player!");
                    return playerData.PlayerHand.Value.FirstOrDefault();
            }
        }

        private CardData ExecuteStrategy_MaxRoundWins_UsePredominantSuit()
        {
            var roundWinCandidates = FindRoundWinningCardCandidates();
            if (roundWinCandidates.Count == 0) {
                roundWinCandidates = playerData.PlayerHand.Value;
            }
            //If more than one card is found, choose the best one!
            var bestCandidate = CardsFilterSolver.GetBestCardFromProvidedCards(roundWinCandidates, predominantCardSuit);
            return bestCandidate;
        }
        
        private List<CardData> FindRoundWinningCardCandidates()
        {
            var potentialCandidates = new List<CardData>();

            // First, read data from Game Round Data to know what are the currently played cards on the board.
            var playerHand = playerData.PlayerHand.Value;

            foreach (var playerCard in playerHand) {
                if (DoesCardBeatCurrentPlayedCards(playerCard)) {
                    potentialCandidates.Add(playerCard);
                }
            }

            return potentialCandidates;
        }
        
        private bool DoesCardBeatCurrentPlayedCards(CardData cardData)
        {
            //Rounds can be resolved with the following rules.
            //First get the cards on the predominant suit.
            //If there is more than one, those will be resolved by their scores.
            //if there is none, the first player in order will count as the predominant suit.

            // First, read data from Game Round Data to know what are the currently played cards on the board.

            var tempPredominantSuit = predominantCardSuit;

            var playedCards = gameRoundData.PlayedCardsInRound.Value.ToList();
            playedCards.Add(cardData);

            var filteredCards = CardsFilterSolver.FilterCardsFromSpecifiedSuit(tempPredominantSuit, playedCards);

            if (filteredCards.Count == 0) {
                // Choose the predominant suit of the first card played this turn.
                tempPredominantSuit = playedCards[0].CardSuit;
                filteredCards = CardsFilterSolver.FilterCardsFromSpecifiedSuit(tempPredominantSuit, playedCards);
            }

            if (filteredCards.Count == 1) {
                var canCardWinRound = filteredCards[0] == cardData;
                Debug.Log(
                    $"[Round Reading Strategy] is Card: [{cardData.CardNumber},{cardData.CardSuit}] winner? {canCardWinRound}");
                return canCardWinRound;
            }

            //If filtered cards do not contain the card data by this point, no need to check further.
            if (filteredCards.Contains(cardData) == false) {
                Debug.Log(
                    $"[Round Reading Strategy] Card: [{cardData.CardNumber},{cardData.CardSuit}] not AMONG cards with predominant card suit!");
                return false;
            }

            //Filtered cards should be more than 1 by this point.
            var winnerCard = CardsFilterSolver.GetWinnerCardFromSameSuitCards(filteredCards);

            var isCardWinner = winnerCard.CardNumber == cardData.CardNumber;
            Debug.Log(
                $"[Round Reading Strategy] is Card: [{cardData.CardNumber},{cardData.CardSuit}] winner? {isCardWinner}");

            return isCardWinner;
        }
    }
}