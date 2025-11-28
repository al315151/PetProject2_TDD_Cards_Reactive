using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Factories;
using NUnit.Framework;
using Presenters;
using Services;

namespace Tests
{
    public class GameManagerValidationTests
    {
        [Test]
        [TestCase(3)]
        [TestCase(7)]
        [TestCase(0)]
        public void GameManagerTestCreateGameWithSpecifiedPlayers(int numberOfCPUPlayers)
        {
            var gameManager = new GameManagerData();
            var strategiesFactory = new StrategiesFactory(gameManager);
            var playerService = new PlayersService(strategiesFactory);
            playerService.InitializePlayers(numberOfCPUPlayers);
            gameManager.ReceivePlayersData(playerService.GetAllPlayersData());
            gameManager.InitializeGameData();

            Assert.IsTrue(gameManager.NumberOfPlayers == numberOfCPUPlayers + 1);
        }

        [Test]
        public void GameManagerValidation_StartGame_PlayersReceiveIntendedInitialCards()
        {
            var numberOfCPUPlayers = 2;
            var gameManager = new GameManagerData();
            var strategiesFactory = new StrategiesFactory(gameManager);
            var playerService = new PlayersService(strategiesFactory);
            var gameManagerPresenter = new GameManagerPresenter(gameManager, playerService);
            playerService.InitializePlayers(numberOfCPUPlayers);
            gameManager.ReceivePlayersData(playerService.GetAllPlayersData());

            // deck created, initial cards dealt, check remaining cards on deck to know if initial card deal is correct.
            gameManagerPresenter.StartGameButtonPressed();

            var playerMaxHandCount = PlayerData.MaxHandSize;
            var numberOfExpectedCards = DeckData.NumberOfCardsPerSuit * Enum.GetValues(typeof(CardSuit)).Length;

            var currentDeckSize = gameManager.CurrentDeckSize();

            Assert.IsTrue(currentDeckSize == numberOfExpectedCards - playerMaxHandCount * gameManager.NumberOfPlayers);
        }

        [Test]
        public async Task GameManagerValidation_PlayOneRound_RoundLifecycleIsCorrect()
        {
            var numberOfCPUPlayers = 2;
            var gameManager = new GameManagerData();
            var strategiesFactory = new StrategiesFactory(gameManager);
            var playerService = new PlayersService(strategiesFactory);
            var gameManagerPresenter = new GameManagerPresenter(gameManager, playerService);
            playerService.InitializePlayers(numberOfCPUPlayers);

            var players = playerService.GetAllPlayersData();

            gameManager.ReceivePlayersData(players);
            gameManager.InitializeGameData();

            gameManager.SetupDeckForNewGame();

            await PlayOneRound(gameManagerPresenter);

            var currentRound = gameManagerPresenter.GetCurrentRound();

            //Make sure the winner player has gotten the score.

            var roundWinner = GetRoundWinner(players, gameManager.GetCurrentRoundData());
            var roundScore = currentRound.GetTotalRoundScore();

            Assert.IsTrue(roundWinner.GetScore() == roundScore);
        }

        [Test]
        public async Task GameManagerValidation_PlayTwoRounds_RoundsAreDifferent()
        {
            var numberOfCPUPlayers = 2;
            var gameManager = new GameManagerData();
            var strategiesFactory = new StrategiesFactory(gameManager);
            var playerService = new PlayersService(strategiesFactory);
            var gameManagerPresenter = new GameManagerPresenter(gameManager, playerService);
            playerService.InitializePlayers(numberOfCPUPlayers);
            gameManager.ReceivePlayersData(playerService.GetAllPlayersData());
            gameManager.InitializeGameData();

            gameManager.SetupDeckForNewGame();

            var firstRoundFinished = await PlayOneRound(gameManagerPresenter);

            var firstRound = gameManagerPresenter.GetCurrentRound();
            var firstRoundId = firstRound.GetCurrentRoundId();

            var secondRoundFinished = await PlayOneRound(gameManagerPresenter);

            var secondRound = gameManagerPresenter.GetCurrentRound();
            var secondRoundId = secondRound.GetCurrentRoundId();

            Assert.IsTrue(firstRoundFinished && secondRoundFinished && firstRoundId != secondRoundId);
        }

        [Test]
        [TestCase(3)]
        [TestCase(7)]
        [TestCase(0)]
        public async Task GameManagerValidation_PlayFourRounds_PlayersHaveProperAmountOfCards(int numberOfCPUPlayers)
        {
            var gameManager = new GameManagerData();
            var strategiesFactory = new StrategiesFactory(gameManager);
            var playerService = new PlayersService(strategiesFactory);
            var gameManagerPresenter = new GameManagerPresenter(gameManager, playerService);

            playerService.InitializePlayers(numberOfCPUPlayers);

            var players = playerService.GetAllPlayersData();

            gameManager.ReceivePlayersData(players);

            gameManagerPresenter.StartGameButtonPressed();

            var firstRoundFinished = await PlayOneRound(gameManagerPresenter);
            var firstRound = gameManagerPresenter.GetCurrentRound();
            var firstRoundId = firstRound.GetCurrentRoundId();

            var secondRoundFinished = await PlayOneRound(gameManagerPresenter);
            var secondRound = gameManagerPresenter.GetCurrentRound();
            var secondRoundId = secondRound.GetCurrentRoundId();

            var thirdRoundFinished = await PlayOneRound(gameManagerPresenter);
            var thirdRound = gameManagerPresenter.GetCurrentRound();
            var thirdRoundId = thirdRound.GetCurrentRoundId();

            var fourthRoundFinished = await PlayOneRound(gameManagerPresenter);
            var fourthRound = gameManagerPresenter.GetCurrentRound();
            var fourthRoundId = fourthRound.GetCurrentRoundId();

            var allRoundsFinished =
                firstRoundFinished && secondRoundFinished && thirdRoundFinished && fourthRoundFinished;
            var allRoundsIdAreDifferent = firstRoundId != secondRoundId
                                          && firstRoundId != thirdRoundId
                                          && firstRoundId != fourthRoundId;

            var allPlayersHaveFullHand = true;

            foreach (var player in players) {
                // Hand is refilled at the start of the round.
                if (player.PlayerHandSize == PlayerData.MaxHandSize - 1) {
                    continue;
                }
                allPlayersHaveFullHand = false;
                break;
            }

            Assert.IsTrue(allRoundsFinished);
            Assert.IsTrue(allRoundsIdAreDifferent);
            Assert.IsTrue(allPlayersHaveFullHand);
        }

        [Test]
        [TestCase(3)]
        [TestCase(7)]
        [TestCase(0)]
        public async Task GameManagerValidation_PlayAllRounds_PlayUntilDeckIsEmpty(int numberOfCPUPlayers)
        {
            var gameManager = new GameManagerData();
            var strategiesFactory = new StrategiesFactory(gameManager);
            var playerService = new PlayersService(strategiesFactory);
            var gameManagerPresenter = new GameManagerPresenter(gameManager, playerService);
            playerService.InitializePlayers(numberOfCPUPlayers);
            gameManager.ReceivePlayersData(playerService.GetAllPlayersData());
            gameManager.InitializeGameData();

            gameManager.SetupDeckForNewGame();

            var previousRoundId = -3;
            // Triggering more rounds than the deck should be able to handle.
            for (var i = 0; i < 99; i++) {
                var nRoundFinished = await PlayOneRound(gameManagerPresenter);
                var nRound = gameManagerPresenter.GetCurrentRound();
                var nRoundId = nRound.GetCurrentRoundId();

                // if round cannot be played and deck is 0, game is over!
                if (nRoundFinished == false && gameManager.CurrentDeckSize() == 0) {
                    break;
                }

                Assert.IsTrue(nRoundFinished && nRoundId != previousRoundId);
                previousRoundId = nRoundId;
            }

            var deckSize = gameManager.CurrentDeckSize();
            Assert.IsTrue(deckSize == 0);
        }

        [Test]
        [TestCase(3)]
        [TestCase(7)]
        [TestCase(0)]
        public async Task GameManagerValidation_PlayAllRounds_GameWinnerHasHighestScore(int numberOfCPUPlayers)
        {
            var gameManager = new GameManagerData();
            var strategiesFactory = new StrategiesFactory(gameManager);
            var playerService = new PlayersService(strategiesFactory);
            var gameManagerPresenter = new GameManagerPresenter(gameManager, playerService);
            playerService.InitializePlayers(numberOfCPUPlayers);

            var players = playerService.GetAllPlayersData();

            gameManager.ReceivePlayersData(players);
            gameManager.InitializeGameData();

            gameManager.SetupDeckForNewGame();

            var previousRoundId = -3;

            // Triggering more rounds than the deck should be able to handle.
            while (gameManager.CurrentDeckSize() > 0) {
                var nRoundFinished = await PlayOneRound(gameManagerPresenter);
                var nRound = gameManagerPresenter.GetCurrentRound();
                var nRoundId = nRound.GetCurrentRoundId();

                // if round cannot be played and deck is 0, game is over!
                if (nRoundFinished == false && gameManager.CurrentDeckSize() == 0) {
                    break;
                }

                Assert.IsTrue(nRoundFinished && nRoundId != previousRoundId);
                previousRoundId = nRoundId;
            }

            var deckSize = gameManager.CurrentDeckSize();
            Assert.IsTrue(deckSize == 0);

            gameManagerPresenter.FinishGame();

            var gameWinnerScore = gameManager.GameWinnerPlayerScore;

            var foundHigherScoreThanWinner = false;
            for (var i = 0; i < players.Count; i++) {
                if (players[i].GetScore() > gameWinnerScore) {
                    foundHigherScoreThanWinner = true;
                    break;
                }
            }

            Assert.IsFalse(foundHigherScoreThanWinner);
        }

        private async Task<bool> PlayOneRound(GameManagerPresenter gameManager)
        {
            if (gameManager.StartPlayRound() == false) {
                return false;
            }

            var currentRound = gameManager.GetCurrentRound();
            var playPhaseFinished = await WaitForRoundPlayPhaseToBeFinished(currentRound);

            var roundFinished = await WaitForRoundToBeFinished(currentRound);

            return playPhaseFinished && roundFinished;
        }

        private async Task<bool> WaitForRoundToBeFinished(GameRoundPresenter round)
        {
            var timeoutLengthInSeconds = 3;
            var timeoutStartTime = DateTime.Now;

            while (round.IsRoundFinished == false) {
                if ((DateTime.Now - timeoutStartTime).TotalSeconds > timeoutLengthInSeconds) {
                    break;
                }
                await Task.Delay(100);
            }

            return round.IsRoundFinished;
        }

        private async Task<bool> WaitForRoundPlayPhaseToBeFinished(GameRoundPresenter round)
        {
            var timeoutLengthInSeconds = 3;
            var timeoutStartTime = DateTime.Now;

            while (round.IsRoundPlayPhaseFinished == false) {
                if ((DateTime.Now - timeoutStartTime).TotalSeconds > timeoutLengthInSeconds) {
                    break;
                }
                await Task.Delay(100);
            }

            return round.IsRoundPlayPhaseFinished;
        }

        private PlayerData GetRoundWinner(List<PlayerData> players, GameRoundData round)
        {
            var winnerId = round.RoundWinnerId;
            PlayerData roundWinner = null;

            for (var i = 0; i < players.Count; i++) {
                if (players[i].PlayerId == winnerId) {
                    roundWinner = players[i];
                    break;
                }
            }
            return roundWinner;
        }
    }
}