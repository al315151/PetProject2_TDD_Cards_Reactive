using System.Collections.Generic;
using R3;

namespace Data
{
    public class PlayerData
    {
        public static int MaxHandSize = 3;


        public CardData LatestPlayedCard;
        public PlayerStrategyType PlayerStrategy => playerStrategy;
        public bool ExternalInputEnabled => inputEnabled;

        public int PlayerHandSize => PlayerHand.Value.Count;
        public int PlayerId => id;

        public ReactiveProperty<List<CardData>> PlayerHand { get; private set; }
        public ReactiveProperty<int> PlayerScore { get; private set; }

        private PlayerStrategyType playerStrategy;

        private bool inputEnabled = false;
        private readonly int id;


        public PlayerData(int playerId)
        {
            this.id = playerId;
            PlayerHand = new ReactiveProperty<List<CardData>>(new List<CardData>());
            PlayerScore = new ReactiveProperty<int>(0);
            inputEnabled = true;
        }

        public void DisablePlayerInput()
        {
            inputEnabled = false;
        }

        public void EnablePlayerInput()
        {
            inputEnabled = true;
        }

        public int GetScore()
        {
            return PlayerScore.Value;
        }

        public void SetPlayerStrategy(PlayerStrategyType playerStrategy)
        {
            this.playerStrategy = playerStrategy;
        }

        public void AddScoreToPlayer(int roundScore)
        {
            PlayerScore.Value += roundScore;
            PlayerScore.OnNext(PlayerScore.CurrentValue);
        }

        public void AddCard(CardData card)
        {
            PlayerHand.CurrentValue.Add(card);
            PlayerHand.OnNext(PlayerHand.CurrentValue);
        }

        public void Reset()
        {
            PlayerScore.Value = 0;
            PlayerScore.OnNext(PlayerScore.CurrentValue);

            PlayerHand.Value.Clear();
            PlayerHand.OnNext(PlayerHand.CurrentValue);
        }

        public void UpdateDataAfterPlayPhase(CardData playedCard)
        {
            LatestPlayedCard = playedCard;
            PlayerHand.Value.Remove(playedCard);
            PlayerHand.OnNext(PlayerHand.CurrentValue);
        }
    }
}