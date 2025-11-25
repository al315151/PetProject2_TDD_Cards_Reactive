using Data;

namespace Strategies
{
    public class PlayerRandomStrategy : IPlayerStrategy
    {
        private PlayerData playerData;

        public void SetStrategyInputData(PlayerData playerData)
        {
            this.playerData = playerData;
        }

        public CardData ExecuteStrategy()
        {
            var playerHand = playerData.PlayerHand.Value;
            var randomIndex = new System.Random().Next(playerHand.Count);
            return playerHand[randomIndex];
        }
    }
}