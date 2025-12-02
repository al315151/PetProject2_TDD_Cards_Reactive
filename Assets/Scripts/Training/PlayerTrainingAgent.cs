using System.Collections.Generic;
using Data;
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
        private PlayerStrategyType playerStrategyType;

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

            //Reconstruct the data received from the Observations.
            var playerCount = academy.GetCurrentNumberOfPlayers();
            var playerMaxHandCount = PlayerData.MaxHandSize;
            var currentRoundPresenter = academy.GetCurrentRoundPresenter();
            var currentIndex = 0;

            var cardsPlayedInRound = new List<CardData>();

            for (int i = 0; i < playerCount; i++)
            {
                //var obtainedCard = new CardData(actions.DiscreteActions)
            }


            Debug.Log($"[Framecount: {Time.frameCount}] ONActionReceived called!");
            base.OnActionReceived(actions);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            Debug.Log($"[Framecount: {Time.frameCount}] Heuristic called!");
            base.Heuristic(actionsOut);
        }

        public override void OnEpisodeBegin()
        {
            Debug.Log($"[Framecount: {Time.frameCount}] OnEpisodeBegin called! for player with ID: {playerId}");
            base.OnEpisodeBegin();
        }
    }
}