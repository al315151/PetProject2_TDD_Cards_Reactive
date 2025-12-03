using System;
using System.Collections.Generic;
using Data;
using NUnit.Framework;
using Presenters;
using Services;
using Strategies;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Training
{
    public class PlayerTrainingAgent : Agent, IInitializable, IDisposable
    {
        public int PlayerId => playerId;

        [SerializeField]
        private PlayerStrategyType playerStrategyType = PlayerStrategyType.RoundReading_MaxRoundWins_UsePredominantSuit;

        [SerializeField]
        private int playerId = 1;

        [SerializeField]
        private CardGameAcademy academy;

        private PlayersService playersService;
        private GameManagerPresenter gameManagerPresenter;

        private bool isAgentOnTrainingEnvironment = true;

        [Inject]
        public void Inject( PlayersService playersService, GameManagerPresenter gameManagerPresenter)
        {
            this.gameManagerPresenter = gameManagerPresenter;
            this.playersService = playersService;
            isAgentOnTrainingEnvironment = false;

            playersService.OnPlayersInitialized += OnPlayersInitialized;
        }

        public override void Initialize()
        {
            base.Initialize();
            if (isAgentOnTrainingEnvironment == false)
            {
                Subscribe();
            }            
        }

        private void Subscribe()
        {
            gameManagerPresenter.OnGameRoundStarted += OnGameRoundStarted;
            gameManagerPresenter.OnGameRoundFinished += OnGameRoundFinished;
        }
        private void OnGameRoundStarted()
        {
            var currentRound = gameManagerPresenter.GetCurrentRound();
            currentRound.OnCardRequestedFromPlayer += OnRequestedCardFromPlayer;
        }

        private void OnGameRoundFinished()
        {
            var currentRound = gameManagerPresenter.GetCurrentRound();
            if (currentRound != null)
            {
                currentRound.OnCardRequestedFromPlayer -= OnRequestedCardFromPlayer;
            }
        }

        private void OnRequestedCardFromPlayer(int playerIndexInTurn)
        {
            if (playerIndexInTurn != playerId)
            {
                return;
            }
            RequestDecision();
        }

        private void OnPlayersInitialized()
        {
            var playerPresenter = playersService.GetPlayerPresenterById(playerId);
            playerPresenter.EnableExternalPlayerInput();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            Debug.Log($"[Framecount: {Time.frameCount}] ======ADDED OBSERVATIONS FOR PLAYER: {playerId} ======");
            if (isAgentOnTrainingEnvironment)
            {
                Debug.Log($"[Framecount: {Time.frameCount}] Observations called from Training environment!");
                GetObservationsInputFromAcademy(
                    out int playerCount,
                    out List<CardData> cardsPlayedInRound,
                    out List<CardData> cardsInHand);
                AddObservationsFromProvidedData(sensor, playerCount, cardsPlayedInRound, cardsInHand);
            }
            else
            {
                Debug.Log($"[Framecount: {Time.frameCount}] Observations called from Injection environment!");
                GetObservationsInputFromInjection(
                    out int playerCount,
                    out List<CardData> cardsPlayedInRound,
                    out List<CardData> cardsInHand);
                AddObservationsFromProvidedData(sensor, playerCount, cardsPlayedInRound, cardsInHand);
            }
        }

        private void GetObservationsInputFromInjection(
            out int playerCount,
            out List<CardData> cardsPlayedInRound,
            out List<CardData> cardsInHand)
        {
            playerCount = playersService.GetAllPlayers().Count;
            var currentRound = gameManagerPresenter.GetCurrentRound().GameRoundData;
            cardsPlayedInRound = currentRound.PlayedCardsInRound.Value;
            var playerPresenter = playersService.GetPlayerPresenterById(playerId);
            cardsInHand = playerPresenter.GetPlayerData().PlayerHand.Value;
        }

        private void GetObservationsInputFromAcademy(
            out int playerCount, 
            out List<CardData> cardsPlayedInRound,
            out List<CardData> cardsInHand)
        {
            playerCount = academy.GetCurrentNumberOfPlayers();
            cardsInHand = academy.GetPlayerData(playerId).PlayerHand.Value;
            var currentRound = academy.GetCurrentRoundPresenter().GameRoundData;
            cardsPlayedInRound = currentRound.PlayedCardsInRound.Value;
        }

        private void AddObservationsFromProvidedData(
            VectorSensor sensor,
            int playerCount,
            List<CardData> cardsPlayedInRound,
            List<CardData> cardsInHand)
        {
            for (int i = 0; i < playerCount; i++)
            {
                if (i >= cardsPlayedInRound.Count)
                {
                    //Debug.Log($"[Framecount: {Time.frameCount}] Adding empty card on index: {i}");
                    sensor.AddObservation(new Vector2(0, 0));
                    continue;
                }
                //Debug.Log($"[Framecount: {Time.frameCount}] Adding Card from Round with info: {cardsPlayedInRound[i].CardNumber} , {cardsPlayedInRound[i].CardSuit} on index: {i}");
                sensor.AddObservation(new Vector2(cardsPlayedInRound[i].CardNumber, (int)cardsPlayedInRound[i].CardSuit));
            }

            var handCardLimit = PlayerData.MaxHandSize;
            for (int i = 0; i < handCardLimit; i++)
            {
                if (i >= cardsInHand.Count)
                {
                    //Debug.Log($"[Framecount: {Time.frameCount}] Adding empty card on index: {i}");
                    sensor.AddObservation(new Vector2(0, 0));
                    continue;
                }
                Debug.Log($"[Framecount: {Time.frameCount}] Adding Card from Hand with info: {cardsInHand[i].CardNumber} , {cardsInHand[i].CardSuit} on index: {i}");
                sensor.AddObservation(new Vector2(cardsInHand[i].CardNumber, (int)cardsInHand[i].CardSuit));
            }


        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            //Debug.Log($"[Framecount: {Time.frameCount}] OnActionReceived called, actions continuous size: {actions.ContinuousActions.Length} , actions discrete actions: {actions.DiscreteActions.Length}");

            List<CardData> playerHand;

            if (isAgentOnTrainingEnvironment)
            {
                GetObservationsInputFromAcademy(out int _, out var _, out playerHand);
            }
            else
            {
                GetObservationsInputFromInjection(out int _, out var _, out playerHand);
            }
            var actionsBuffer = actions.DiscreteActions;
            var cardIndexInHand = actionsBuffer[0];

            //Nothing to play, nothing to do.
            if (playerHand.Count == 0)
            {
                return;
            }

            //If input received is not valid, then return a valid card.
            if (cardIndexInHand >= playerHand.Count)
            {
                cardIndexInHand = UnityEngine.Random.Range(0, playerHand.Count);
            }

            var cardDataFromIndex = playerHand[cardIndexInHand];

            Debug.Log($"[Framecount: {Time.frameCount}] [OnActionReceived] Card found by action: {cardDataFromIndex.CardNumber} , {cardDataFromIndex.CardSuit}");

            if (isAgentOnTrainingEnvironment)
            {
                academy.PlayCardFromTrainingAgent(playerId, cardDataFromIndex);
            }
            else
            {
                PlayCardFromNonTrainingEnvironment(playerId, cardDataFromIndex);
            }
            
        }

        private void PlayCardFromNonTrainingEnvironment(int playerId, CardData cardData)
        {
            var playerPresenter = playersService.GetPlayerPresenterById(playerId);
            if (playerPresenter == null)
            {
                return;
            }
            playerPresenter.PlayCardFromUserHand(cardData.CardSuit, cardData.CardNumber);

        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            //Debug.Log($"[Framecount: {Time.frameCount}] Heuristic called!");

            var discreteActions = actionsOut.DiscreteActions;
            int indexOfPlayerCard;
            
            if (isAgentOnTrainingEnvironment)
            {
                var playerData = academy.GetPlayerData(playerId);
                var roundData = academy.GetCurrentRoundPresenter().GameRoundData;
                indexOfPlayerCard = GetHeuristicFromInput(playerData, roundData);
            }
            else
            {
                var playerData = playersService.GetPlayerPresenterById(playerId).GetPlayerData();
                var roundData = gameManagerPresenter.GetCurrentRound().GameRoundData;
                indexOfPlayerCard = GetHeuristicFromInput(playerData, roundData);
            }

            discreteActions[0] = Mathf.Max(0, indexOfPlayerCard);
        }

        private int GetHeuristicFromInput(PlayerData playerData, GameRoundData roundData)
        {
            var strategySolver = new PlayerTableReadingStrategiesSolver();
            strategySolver.SetupAdditionalData(playerStrategyType, roundData, academy.GetPredominantCardSuit());
            strategySolver.SetupPlayerData(playerData);
            var cardPoppedUpFromStrategy = strategySolver.ExecuteStrategy();

            return playerData.PlayerHand.Value.IndexOf(cardPoppedUpFromStrategy);
        }

        public override void OnEpisodeBegin()
        {
            Debug.Log($"[Framecount: {Time.frameCount}] OnEpisodeBegin called! StepCount from academy: {Academy.Instance.StepCount} for player with ID: {playerId}");
        }

        public void Dispose()
        {
            if (isAgentOnTrainingEnvironment)
            {
                return;
            }
            playersService.OnPlayersInitialized -= OnPlayersInitialized;
            gameManagerPresenter.OnGameRoundStarted -= OnGameRoundStarted;
            gameManagerPresenter.OnGameRoundFinished -= OnGameRoundFinished;
        }
    }
}