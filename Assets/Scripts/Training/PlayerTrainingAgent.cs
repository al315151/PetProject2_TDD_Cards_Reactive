using Data;
using Strategies;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Training
{
    public class PlayerTrainingAgent : Agent
    {
        public int PlayerId => playerId;

        [SerializeField]
        private PlayerStrategyType playerStrategyType = PlayerStrategyType.RoundReading_MaxRoundWins_UsePredominantSuit;

        [SerializeField]
        private int playerId = 1;

        [SerializeField]
        private CardGameAcademy academy;

        public override void CollectObservations(VectorSensor sensor)
        {
            var playerCount = academy.GetCurrentNumberOfPlayers();
            var playerMaxHandCount = PlayerData.MaxHandSize;
            var playerHand = academy.GetPlayerData(playerId).PlayerHand.Value;
            var currentRound = academy.GetCurrentRoundPresenter().GameRoundData;
            var playedCards = currentRound.PlayedCardsInRound.Value;

            Debug.Log($"[Framecount: {Time.frameCount}] ==========ADDED OBSERVATIONS FOR PLAYER: {playerId} ==========");

            for (int i = 0; i < playerCount; i++)
            {
                if (i >= playedCards.Count)
                {
                    Debug.Log($"[Framecount: {Time.frameCount}] Adding empty card on index: {i}");
                    sensor.AddObservation(new Vector2(0, 0));
                    continue;
                }
                Debug.Log($"[Framecount: {Time.frameCount}] Adding Card from Round with info: {playedCards[i].CardNumber} , {playedCards[i].CardSuit} on index: {i}");
                sensor.AddObservation(new Vector2(playedCards[i].CardNumber, (int)playedCards[i].CardSuit));
            }

            for (int i = 0; i < playerMaxHandCount; i++)
            {
                if (i >= playerHand.Count)
                {
                    Debug.Log($"[Framecount: {Time.frameCount}] Adding empty card on index: {i}");
                    sensor.AddObservation(new Vector2(0 , 0));
                    continue;
                }
                Debug.Log($"[Framecount: {Time.frameCount}] Adding Card from Hand with info: {playerHand[i].CardNumber} , {playerHand[i].CardSuit} on index: {i}");
                sensor.AddObservation(new Vector2(playerHand[i].CardNumber, (int)playerHand[i].CardSuit));
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            Debug.Log($"[Framecount: {Time.frameCount}] OnActionReceived called, actions continuous size: {actions.ContinuousActions.Length} , actions discrete actions: {actions.DiscreteActions.Length}");

            var actionsBuffer = actions.DiscreteActions;
            var cardIndexInHand = actionsBuffer[0];

            var playerData = academy.GetPlayerData(playerId);
            var playerHand = playerData.PlayerHand.Value;

            //If input received is not valid, then return a valid card.
            if (playerHand.Count >= cardIndexInHand)
            {
                cardIndexInHand = Random.Range(0, playerHand.Count);
            }

            var cardDataFromIndex = playerHand[cardIndexInHand];

            Debug.Log($"[Framecount: {Time.frameCount}]Card found by action: {cardDataFromIndex.CardNumber} , {cardDataFromIndex.CardSuit}");

            academy.PlayCardFromTrainingAgent(playerId, cardDataFromIndex);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            Debug.Log($"[Framecount: {Time.frameCount}] Heuristic called!");

            //Reconstruct the data received from the Observations.
            var playerCount = academy.GetCurrentNumberOfPlayers();
            var playerMaxHandCount = PlayerData.MaxHandSize;
            var playerData = academy.GetPlayerData(playerId);
            var currentRoundPresenter = academy.GetCurrentRoundPresenter();
            var roundData = currentRoundPresenter.GameRoundData;

            var strategySolver = new PlayerTableReadingStrategiesSolver();
            strategySolver.SetupAdditionalData(playerStrategyType, roundData, academy.GetPredominantCardSuit());
            strategySolver.SetupPlayerData(playerData);
            var cardPoppedUpFromStrategy = strategySolver.ExecuteStrategy();
            
            var indexOfPlayerCard = playerData.PlayerHand.Value.IndexOf(cardPoppedUpFromStrategy);

            var discreteActions = actionsOut.DiscreteActions;

            discreteActions[0] = Mathf.Max(0, indexOfPlayerCard);
        }

        public override void OnEpisodeBegin()
        {
            Debug.Log($"[Framecount: {Time.frameCount}] OnEpisodeBegin called! for player with ID: {playerId}");
        }
    }
}