
using System.Collections.Generic;

namespace Strategies
{
    public class PlayerRandomStrategy : IPlayerStrategy
    {
        private List<CardData> playerHand;

        public void SetStrategyInputData(List<CardData> playerHand)
        {
            this.playerHand = playerHand;
        }

        public CardData ExecuteStrategy()
        {
            var randomIndex = new System.Random().Next(playerHand.Count);
            return playerHand[randomIndex];
        }
    }
}