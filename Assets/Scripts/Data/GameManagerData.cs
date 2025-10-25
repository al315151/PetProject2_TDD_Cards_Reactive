using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Data
{
    public class GameManagerData
    {
        public int NumberOfPlayers => playersData.Count;
        public int GameWinnerPlayerId => gameWinnerId;
        public int GameWinnerPlayerScore => gameWinnerScore;

        public CardSuit DeckInitialCardSuit => deckData.ChosenCardSuit;
        public ReactiveProperty<int> CurrentRoundIndex { get; private set; }

        private List<PlayerData> playersData;
        private DeckData deckData;        

        private IGameRoundPrototype gameRoundConcretePrototype;

        private GameRoundData currentGameRound;
        private List<GameRoundData> roundDataHistory;

        private int gameWinnerId;
        private int gameWinnerScore;

        public GameManagerData()
        {
            deckData = new DeckData();

            CurrentRoundIndex = new ReactiveProperty<int>(0);

            gameRoundConcretePrototype = new GameRoundData(CurrentRoundIndex.CurrentValue);
            roundDataHistory = new List<GameRoundData>();
        }
        
        public void CreateGame()
        {
            //Players will be created through PlayersService.
            deckData.CreateDeck();
        }

        public void ReceivePlayersData(List<PlayerData> playersData)
        {
            this.playersData = playersData;
        }
        
        public void StartGame()
        {
            //Shuffle the cards, set the deck chosen Suit.
            deckData.Shuffle();
            deckData.ChooseInitialSuit();

            DrawInitialHandForPlayers();
        }

        public int CurrentDeckSize()
        {
            return deckData.DeckCardCount;
        }

        private void DrawInitialHandForPlayers()
        {
            var initialCardCountForPlayer = PlayerData.MaxHandSize;
            for (var i = 0; i < playersData.Count; i++) { 
                var player = playersData[i]; 
                for (int j = 0; j < initialCardCountForPlayer; j++) {
                    player.AddCardToHandFromDeck(deckData);
                }
            }
        }

        public int GetCurrentRoundId()
        {
            return currentGameRound.RoundId;
        }

        public int GetCurrentPlayerInOrder()
        {
            return currentGameRound.GetCurrentPlayerIdInOrder();
        }

        public bool CreateAndStartRound()
        {
            CurrentRoundIndex.Value++;
            if (currentGameRound != null) {
                roundDataHistory.Add(currentGameRound);
            }
            
            var gameRoundPrototype = gameRoundConcretePrototype.Clone(CurrentRoundIndex.Value);
            
            var gameRound = gameRoundPrototype as GameRoundData;
            gameRound.ReceivePlayers(playersData);
            gameRound.StartPlayerDrawPhase(deckData);

            if (CanRoundBePlayed() == false) {
                return false;
            }
            currentGameRound = gameRound;
            return true;
        }
        
        public void EstablishRoundOrder()
        {
            // Winner of last round will start.
            // Otherwise, player will go last. 
            var playerOrder = new List<int>();
            var startingIndex = 1;
            if (currentGameRound.RoundId <= 1) {
                startingIndex = 1;
            }
            else {
                var previousRoundWinner = roundDataHistory[^1].RoundWinnerId;
                startingIndex = 0;
                for (int i = 0; i < playersData.Count; i++) {
                    if (playersData[i].PlayerId == previousRoundWinner) {
                        startingIndex = i;
                        break;
                    }
                }
            }

            for (int i = 0; i < playersData.Count; i++) {
                if (startingIndex == playersData.Count) {
                    startingIndex = 0;
                }

                playerOrder.Add(playersData[startingIndex].PlayerId);
                startingIndex++;
            }

            currentGameRound.SetPlayerOrder(playerOrder);
        }

        public GameRoundData GetCurrentRound()
        {
            return currentGameRound;
        }

        public bool StartPlayRound()
        {
            //First setup the Round object.
            if (CreateAndStartRound() == false) {
                return false;
            }
            // Then set round order.
            EstablishRoundOrder();
            // Then, start the Play phase. we will receive event / wait for the cards to be played 
            currentGameRound.StartPlayPhase();       
            // On current gameplay, we will have to wait for player input to actually know when to resolve the situation.
            return true;
        }

        public void FinishRound()
        {
            var winnerId = currentGameRound.ResolveRound(deckData.ChosenCardSuit);

            currentGameRound.FinishRound(winnerId);
        }
        
        private bool CanRoundBePlayed()
        {
            for (int i = 0; i < playersData.Count; i++) {
                if (playersData[i].PlayerHandSize < 1) {
                    return false;
                }
            }
            return true;
        }

        public void FinishGame()
        {
            var playerMaxScore = -1;
            foreach (var player in playersData) {
                var currentPlayerScore = player.GetScore();
                if (currentPlayerScore > playerMaxScore) {
                    playerMaxScore = currentPlayerScore;
                    gameWinnerScore = playerMaxScore;
                    gameWinnerId = player.PlayerId;
                }
            }
        }
    }
}
