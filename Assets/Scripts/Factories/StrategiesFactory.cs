using Data;
using Strategies;

namespace Factories
{
    public class StrategiesFactory
    {
        private readonly GameManagerData gameManagerData;

        public StrategiesFactory(GameManagerData gameManagerData)
        {
            this.gameManagerData = gameManagerData;
        }

        public IPlayerStrategy CreateRandomStrategy(PlayerData playerData)
        {
            var newStrategy = new PlayerRandomStrategy();
            newStrategy.SetStrategyInputData(playerData);
            return newStrategy;
        }

    }
}