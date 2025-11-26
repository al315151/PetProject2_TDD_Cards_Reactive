using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Factories;
using NUnit.Framework;
using PlayerPresenters;
using Presenters;
using Strategies;
using UnityEngine;

namespace Tests
{
    public class PlayerStrategiesValidationTests
    {
        [Test]
        public async Task PlayerStrategiesValidation_PlayerUsesRandomStrategy_WinsRoundWithRiggedBoard()
        {
            CreateCustomPlayersAndRound(out var players, out var gameRoundData);

            var gameManager = new GameManagerData();
            var strategiesFactory = new StrategiesFactory(gameManager);
            var randomStrategy = strategiesFactory.CreateRandomStrategy(players[0].GetPlayerData());

            players[0].SetPlayerStrategy(PlayerStrategyType.Random, randomStrategy);

            var chosenCardSuit = CardSuit.Swords;
            var otherCardSuit = CardSuit.Clubs;

            //Add expected cards to players so that we can rig who is going to win.
            players[0].TestAddCard(new CardData(chosenCardSuit, 5));
            players[1].TestAddCard(new CardData(otherCardSuit, 7));
            players[2].TestAddCard(new CardData(otherCardSuit, 1));


            await GameRoundPlayPhase(gameRoundData);

            Assert.IsTrue(gameRoundData.IsRoundPlayPhaseFinished);

            int winnerId = gameRoundData.ResolveRound(chosenCardSuit);
            int scoreFromRound = gameRoundData.GetTotalRoundScore();
            gameRoundData.FinishRound(winnerId);

            var firstPlayerData = players[0].GetPlayerData();

            Assert.IsTrue(firstPlayerData.GetScore() == scoreFromRound);

        }

        [Test]
        public async Task PlayerStrategiesValidation_PlayerUsesBoardReadingStrategy_GoesFirstWinsRoundWithChosenSuit()
        {
            CreateCustomPlayersAndRound(out var players, out var gameRoundData);

            var chosenCardSuit = CardSuit.Swords;
            var otherCardSuit = CardSuit.Clubs;

            var winnerPlayer = players[0];

            SetupBoardReadingStrategyOnPlayer(ref winnerPlayer, gameRoundData.GameRoundData, chosenCardSuit);

            //Add expected cards to players so that we can rig who is going to win.
            players[0].TestAddCard(new CardData(otherCardSuit, 5));
            players[0].TestAddCard(new CardData(otherCardSuit, 2));
            players[0].TestAddCard(new CardData(chosenCardSuit, 5));
            players[0].TestAddCard(new CardData(otherCardSuit, 3));
            
            players[1].TestAddCard(new CardData(otherCardSuit, 7));
            players[2].TestAddCard(new CardData(otherCardSuit, 1));


            await GameRoundPlayPhase(gameRoundData);

            Assert.IsTrue(gameRoundData.IsRoundPlayPhaseFinished);

            int winnerId = gameRoundData.ResolveRound(chosenCardSuit);
            int scoreFromRound = gameRoundData.GetTotalRoundScore();
            gameRoundData.FinishRound(winnerId);

            var firstPlayerData = players[0].GetPlayerData();

            Assert.IsTrue(firstPlayerData.GetScore() == scoreFromRound);

        }

        [Test]
        public async Task PlayerStrategiesValidation_PlayerUsesBoardReadingStrategy_GoesLastWinsRoundWithChosenSuitAndHigherScore()
        {
            CreateCustomPlayersAndRound(out var players, out var gameRoundData);

            var chosenCardSuit = CardSuit.Swords;
            var otherCardSuit = CardSuit.Clubs;

            var winnerPlayer = players[2];

            SetupBoardReadingStrategyOnPlayer(ref winnerPlayer, gameRoundData.GameRoundData, chosenCardSuit);

            //Add expected cards to players so that we can rig who is going to win.
            players[2].TestAddCard(new CardData(otherCardSuit, 5));
            players[2].TestAddCard(new CardData(otherCardSuit, 2));
            players[2].TestAddCard(new CardData(chosenCardSuit, 3));
            players[2].TestAddCard(new CardData(otherCardSuit, 3));

            players[1].TestAddCard(new CardData(chosenCardSuit, 7));
            players[0].TestAddCard(new CardData(otherCardSuit, 1));

            await GameRoundPlayPhase(gameRoundData);

            Assert.IsTrue(gameRoundData.IsRoundPlayPhaseFinished);

            int winnerId = gameRoundData.ResolveRound(chosenCardSuit);
            int scoreFromRound = gameRoundData.GetTotalRoundScore();
            gameRoundData.FinishRound(winnerId);

            var firstPlayerData = winnerPlayer.GetPlayerData();

            Assert.IsTrue(firstPlayerData.GetScore() == scoreFromRound);

        }

        private void CreateCustomPlayersAndRound(out List<PlayerPresenter> players, out GameRoundPresenter gameRoundData)
        {
            var playerData = new PlayerPresenter();

            players = new() {
                playerData.Clone(1) as PlayerPresenter,
                playerData.Clone(2) as PlayerPresenter,
                playerData.Clone(3) as PlayerPresenter,
            };

            gameRoundData = new GameRoundPresenter(1);
            gameRoundData.ReceivePlayerPresenters(players);

            var playersData = new List<PlayerData>();

            for (int i = 0; i < players.Count; i++)
            {
                playersData.Add(players[i].GetPlayerData());
            }

            gameRoundData.ReceivePlayers(playersData);
            gameRoundData.SetPlayerOrder(new() { 1, 2, 3 });
        }

        private void SetupBoardReadingStrategyOnPlayer(ref PlayerPresenter playerPresenter,
            GameRoundData gameRoundData,
            CardSuit chosenCardSuit)
        {
            var gameManager = new GameManagerData();
            var strategiesFactory = new StrategiesFactory(gameManager);
            var tableReadingStrategy = strategiesFactory.CreateRoundPlayedCardsStrategy(playerPresenter.GetPlayerData()) as PlayerTableReadingStrategy;

            tableReadingStrategy.SetupAdditionalData(gameRoundData, chosenCardSuit);

            playerPresenter.SetPlayerStrategy(PlayerStrategyType.RoundPlayedCardsProcessing, tableReadingStrategy);
        }

        private async Task GameRoundPlayPhase(GameRoundPresenter gameRoundData)
        {
            gameRoundData.StartPlayPhase();

            var timeoutInSeconds = 3;

            var timeoutInSecondsTimeStamp = DateTime.Now;
            while (gameRoundData.IsRoundPlayPhaseFinished == false)
            {
                await Task.Delay(100);
                if ((DateTime.Now - timeoutInSecondsTimeStamp).TotalSeconds > timeoutInSeconds)
                {
                    break;
                }
            }
        }
    }
}