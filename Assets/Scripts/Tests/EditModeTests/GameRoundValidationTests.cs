using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Factories;
using NUnit.Framework;
using PlayerPresenters;
using Presenters;
using Services;

namespace Tests
{
    public class GameRoundValidationTests
    {
        private void SetupGameManagerAndStartGame(out GameManagerData gameManagerData, out GameManagerPresenter gameManagerPresenter, out PlayersService playersService)
        {
            var numberOfCPUPlayers = 2;

            gameManagerData = new GameManagerData();
            var strategiesFactory = new StrategiesFactory(gameManagerData);
            playersService = new PlayersService(strategiesFactory);
            gameManagerPresenter = new GameManagerPresenter(gameManagerData, playersService);
            
            playersService.InitializePlayers(numberOfCPUPlayers);            
            var players = playersService.GetAllPlayersData();

            gameManagerData.ReceivePlayersData(players);
            gameManagerData.InitializeGameData();

            //Create the game and start it.
            //Only then Game Rounds can be created.
            gameManagerData.SetupDeckForNewGame();
        }

        [Test]
        public void GameRoundValidationCreateGameRound()
        {
            SetupGameManagerAndStartGame(out GameManagerData gameManagerData, out GameManagerPresenter gameManagerPresenter, out PlayersService playersService);

            gameManagerPresenter.StartPlayRound();

            Assert.IsTrue(gameManagerData.GetCurrentRoundId() == 1);
        }

        [Test]
        public void GameRoundValidationEstablishRoundOrder()
        {
            SetupGameManagerAndStartGame(out GameManagerData gameManagerData, out GameManagerPresenter gameManagerPresenter, out PlayersService playersService);

            gameManagerPresenter.CreateAndStartRound();

            //Each player needs to play their cards.
            //For now resolving the round is not needed.
            gameManagerData.EstablishRoundOrder();

            var firstPlayerInPlayOrder = gameManagerData.GetCurrentPlayerInOrder();
            var firstCPUId = 1;

            Assert.IsTrue(firstPlayerInPlayOrder == firstCPUId);
        }

        [Test]
        public async Task GameRoundValidationPlayPhaseMakesPlayersSelectCardForRound()
        {
            SetupGameManagerAndStartGame(out GameManagerData gameManagerData, out GameManagerPresenter gameManagerPresenter, out PlayersService playersService);

            gameManagerPresenter.CreateAndStartRound();
            gameManagerData.EstablishRoundOrder();

            //Number of players: 2 + player.
            List<PlayerPresenter> players = playersService.GetAllPlayers();

            GameRoundPresenter round = gameManagerPresenter.GetCurrentRound();

            //Following order, wait until all players do play their cards. 
            await GameRoundPlayPhase(round);

            Assert.IsTrue(round.IsRoundPlayPhaseFinished);
        }

        [Test]
        public async Task GameRoundValidationResolvePhase_PlayerWins_ChosenSuit()
        {
            CreateCustomPlayersAndRound(out List<PlayerPresenter> players, out GameRoundPresenter gameRoundData);

            var chosenCardSuit = CardSuit.Swords;
            var otherCardSuit = CardSuit.Clubs;
        
            //Add expected cards to players so that we can rig who is going to win.
            players[0].TestAddCard(new CardData(chosenCardSuit, 5));
            players[1].TestAddCard(new CardData(otherCardSuit, 5));
            players[2].TestAddCard(new CardData(otherCardSuit, 1));

            await GameRoundPlayPhase(gameRoundData);

            Assert.IsTrue(gameRoundData.IsRoundPlayPhaseFinished);

            int winnerId = gameRoundData.ResolveRound(chosenCardSuit);

            Assert.IsTrue(winnerId == players[0].PlayerId);
        }

        [Test]
        public async Task GameRoundValidationResolvePhase_PlayerWinsWithHighestScore_ChosenSuit()
        {
            CreateCustomPlayersAndRound(out var players, out var gameRoundData);

            var chosenCardSuit = CardSuit.Swords;
            var otherCardSuit = CardSuit.Clubs;

            //Add expected cards to players so that we can rig who is going to win.
            players[0].TestAddCard(new CardData(chosenCardSuit, 5));
            players[1].TestAddCard(new CardData(chosenCardSuit, 12));
            players[2].TestAddCard(new CardData(otherCardSuit, 1));

            await GameRoundPlayPhase(gameRoundData);

            Assert.IsTrue(gameRoundData.IsRoundPlayPhaseFinished);

            int winnerId = gameRoundData.ResolveRound(chosenCardSuit);

            Assert.IsTrue(winnerId == players[1].PlayerId);
        }

        [Test]
        public async Task GameRoundValidationResolvePhase_FirstPlayerWins_NoChosenSuit()
        {
            CreateCustomPlayersAndRound(out var players, out var gameRoundData);

            var chosenCardSuit = CardSuit.Swords;
            var otherCardSuit = CardSuit.Clubs;
            var anotherCardSuit = CardSuit.Coins;

            //Add expected cards to players so that we can rig who is going to win.
            players[0].TestAddCard(new CardData(otherCardSuit, 5));
            players[1].TestAddCard(new CardData(anotherCardSuit, 12));
            players[2].TestAddCard(new CardData(anotherCardSuit, 1));

            await GameRoundPlayPhase(gameRoundData);

            Assert.IsTrue(gameRoundData.IsRoundPlayPhaseFinished);

            int winnerId = gameRoundData.ResolveRound(chosenCardSuit);

            Assert.IsTrue(winnerId == players[0].PlayerId);
        }

        [Test]
        public async Task GameRoundValidationResolvePhase_PlayerWinsWithHighestScore_NoChosenSuit()
        {
            CreateCustomPlayersAndRound(out var players, out var gameRoundData);

            var chosenCardSuit = CardSuit.Swords;
            var otherCardSuit = CardSuit.Clubs;
            var anotherCardSuit = CardSuit.Coins;

            //Add expected cards to players so that we can rig who is going to win.
            players[0].TestAddCard(new CardData(otherCardSuit, 5));
            players[1].TestAddCard(new CardData(anotherCardSuit, 12));
            players[2].TestAddCard(new CardData(otherCardSuit, 1));

            await GameRoundPlayPhase(gameRoundData);

            Assert.IsTrue(gameRoundData.IsRoundPlayPhaseFinished);

            int winnerId = gameRoundData.ResolveRound(chosenCardSuit);

            Assert.IsTrue(winnerId == players[2].PlayerId);
        }

        [Test]
        public async Task GameRoundValidationResolvePhase_PlayerWinsWithHighestNumber_NoScore_NoChosenSuit()
        {
            CreateCustomPlayersAndRound(out var players, out var gameRoundData);

            var chosenCardSuit = CardSuit.Swords;
            var otherCardSuit = CardSuit.Clubs;
            var anotherCardSuit = CardSuit.Coins;

            //Add expected cards to players so that we can rig who is going to win.
            players[0].TestAddCard(new CardData(otherCardSuit, 5));
            players[1].TestAddCard(new CardData(anotherCardSuit, 12));
            players[2].TestAddCard(new CardData(otherCardSuit, 2));

            await GameRoundPlayPhase(gameRoundData);

            Assert.IsTrue(gameRoundData.IsRoundPlayPhaseFinished);

            var winnerId = gameRoundData.ResolveRound(chosenCardSuit);

            Assert.IsTrue(winnerId == players[0].PlayerId);
        }

        [Test]
        public async Task GameRoundValidationResolvePhase_RoundScoreResult_OneAce_EqualsToItsPoints()
        {
            CreateCustomPlayersAndRound(out var players, out var gameRoundData);

            var chosenCardSuit = CardSuit.Swords;
            var otherCardSuit = CardSuit.Clubs;
            var anotherCardSuit = CardSuit.Coins;

            //Add expected cards to players so that we can rig who is going to win.
            players[0].TestAddCard(new CardData(otherCardSuit, 5));
            players[1].TestAddCard(new CardData(anotherCardSuit, 1));
            players[2].TestAddCard(new CardData(otherCardSuit, 2));

            await GameRoundPlayPhase(gameRoundData);

            Assert.IsTrue(gameRoundData.IsRoundPlayPhaseFinished);
        
            var winnerId = gameRoundData.ResolveRound(chosenCardSuit);

            var scoreFromRound = gameRoundData.GetTotalRoundScore();
            var scoreForAce = CardNumberToScoreConversionHelper.CardNumberToScoreConversion[1];

            Assert.IsTrue(scoreFromRound == scoreForAce);
        }

        [Test]
        public async Task GameRoundValidationEndPhase_FinishRound_WinnerObtainsPointsFromRound()
        {
            CreateCustomPlayersAndRound(out var players, out var gameRoundData);

            var chosenCardSuit = CardSuit.Swords;
            var otherCardSuit = CardSuit.Clubs;
            var anotherCardSuit = CardSuit.Coins;

            //Add expected cards to players so that we can rig who is going to win.
            players[0].TestAddCard(new CardData(otherCardSuit, 5));
            players[1].TestAddCard(new CardData(anotherCardSuit, 12));
            players[2].TestAddCard(new CardData(otherCardSuit, 2));

            await GameRoundPlayPhase(gameRoundData);

            Assert.IsTrue(gameRoundData.IsRoundPlayPhaseFinished);

            int winnerId = gameRoundData.ResolveRound(chosenCardSuit);
            int scoreFromRound = gameRoundData.GetTotalRoundScore();
            gameRoundData.FinishRound(winnerId);

            var firstPlayerData = players[0].GetPlayerData();

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

        private async Task GameRoundPlayPhase(GameRoundPresenter gameRoundData)
        {
            gameRoundData.StartPlayPhase();

            var timeoutInSeconds = 3;

            var timeoutInSecondsTimeStamp = DateTime.Now;
            while (gameRoundData.IsRoundPlayPhaseFinished == false) {
                await Task.Delay(100);
                if ((DateTime.Now - timeoutInSecondsTimeStamp).TotalSeconds > timeoutInSeconds) {
                    break;
                }
            }
        }

    }
}
