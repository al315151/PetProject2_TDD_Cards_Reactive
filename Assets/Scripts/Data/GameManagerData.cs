using System;
using System.Collections.Generic;
using R3;

namespace Data
{
    public class GameManagerData
    {
        public int NumberOfPlayers => playersData.Count;
        public int GameWinnerPlayerId => gameWinnerId;
        public int GameWinnerPlayerScore => gameWinnerScore;

        public CardSuit DeckInitialCardSuit => deckData.ChosenCardSuit;
        public ReactiveProperty<int> CurrentRoundIndex { get; private set; }

        public Action CurrentRoundPlayPhaseFinished;

        private List<PlayerData> playersData;
        private DeckData deckData;

        private GameRoundData currentGameRoundData;
        private List<GameRoundData> roundDataHistory;

        private int gameWinnerId;
        private int gameWinnerScore;

        public GameManagerData()
        {
            deckData = new DeckData();

            CurrentRoundIndex = new ReactiveProperty<int>(0);
            roundDataHistory = new List<GameRoundData>();
        }

        public void InitializeGameData()
        {
            //Players will be created through PlayersService.
            deckData.CreateDeck();
        }

        public void ReceivePlayersData(List<PlayerData> playersData)
        {
            this.playersData = playersData;
        }

        public void SetupDeckForNewGame()
        {
            //Shuffle the cards, set the deck chosen Suit.
            deckData.Shuffle();
            deckData.ChooseInitialSuit();
        }

        public void SetupCurrentGameRoundData(GameRoundData gameRoundData)
        {
            currentGameRoundData = gameRoundData;
        }

        public int CurrentDeckSize()
        {
            return deckData.DeckCardCount;
        }

        public int GetCurrentRoundId()
        {
            return currentGameRoundData.RoundId;
        }

        public int GetCurrentPlayerInOrder()
        {
            return currentGameRoundData.GetCurrentPlayerIdInOrder();
        }

        public void SetupGameWinner(int playerId, int score)
        {
            gameWinnerId = playerId;
            gameWinnerScore = score;
        }

        public void IncrementCurrentRoundIndex()
        {
            CurrentRoundIndex.Value++;
        }

        public bool SavePreviousRoundToRoundHistory()
        {
            if (currentGameRoundData != null && !roundDataHistory.Contains(currentGameRoundData)) {
                roundDataHistory.Add(currentGameRoundData);
                return true;
            }
            return false;
        }

        public void EstablishRoundOrder()
        {
            // Winner of last round will start.
            // Otherwise, player will go last. 
            var playerOrder = new List<int>();
            var startingIndex = 1;
            if (currentGameRoundData.RoundId <= 1) {
                startingIndex = 0;
            }
            else {
                var previousRoundWinner = roundDataHistory[^1].RoundWinnerId;
                startingIndex = 0;
                for (var i = 0; i < playersData.Count; i++) {
                    if (playersData[i].PlayerId == previousRoundWinner) {
                        startingIndex = i;
                        break;
                    }
                }
            }

            for (var i = 0; i < playersData.Count; i++) {
                if (startingIndex == playersData.Count) {
                    startingIndex = 0;
                }

                playerOrder.Add(playersData[startingIndex].PlayerId);
                startingIndex++;
            }

            currentGameRoundData.SetPlayerOrder(playerOrder);
        }

        public GameRoundData GetCurrentRoundData()
        {
            return currentGameRoundData;
        }

        public void ResetAll()
        {
            deckData.CreateDeck();
            CurrentRoundIndex.Value = 0;
            gameWinnerId = 0;
            roundDataHistory.Clear();
            currentGameRoundData = null;
        }

        public DeckData GetDeckData()
        {
            return deckData;
        }
    }
}