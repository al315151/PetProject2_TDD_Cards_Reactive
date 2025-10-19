using System.Collections.Generic;

namespace Data
{
    public class GameManagerData
    {
        public int NumberOfPlayers => playersData.Count;

        private List<PlayerData> playersData;
        private DeckData deckData;
        private int currentRound;

        private IGameRoundPrototype gameRoundConcretePrototype;

        private GameRoundData currentGameRound;
        private List<GameRoundData> roundDataHistory;

        public void CreateGame(int numberOfCPUPlayers)
        {
            playersData = new List<PlayerData>();

            //Player will be -1. No player should be allowed to be 0.
            var basePlayer = new PlayerData(-1);
            playersData.Add(basePlayer);

            for (var i = 0; i < numberOfCPUPlayers; i++) {
                var newCPU = basePlayer.Clone(playersData.Count) as PlayerData;
                playersData.Add(newCPU);
            }

            deckData = new DeckData();
            deckData.CreateDeck();

            gameRoundConcretePrototype = new GameRoundData(currentRound);
            roundDataHistory = new List<GameRoundData>();
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

        public void CreateAndStartRound()
        {
            currentRound++;
            if (currentGameRound != null) {
                currentGameRound.RemovePlayerReferences();
                roundDataHistory.Add(currentGameRound);
            }
            
            var gameRoundPrototype = gameRoundConcretePrototype.Clone(currentRound);
            
            currentGameRound = gameRoundPrototype as GameRoundData;
            currentGameRound.ReceivePlayers(playersData);
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

        public List<PlayerData> GetPlayers()
        {
            return playersData;
        }

        public GameRoundData GetCurrentRound()
        {
            return currentGameRound;
        }

        public void StartPlayRound()
        {        
            //First setup the Round object.
            CreateAndStartRound();
            // Then set round order.
            EstablishRoundOrder();
            // Then, start the Play phase. we will receive event / wait for the cards to be played 
            currentGameRound.StartPlayPhase();       
            // On current gameplay, we will have to wait for player input to actually know when to resolve the situation.
        }

        public void FinishRound()
        {
            var winnerId = currentGameRound.ResolveRound(deckData.ChosenCardSuit);

            currentGameRound.FinishRound(winnerId);
        }
    }
}
