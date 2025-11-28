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
            newStrategy.SetupPlayerData(playerData);
            return newStrategy;
        }

        public IPlayerStrategy CreateRoundPlayedCardsStrategiesSolver(PlayerData playerData)
        {
            var newStrategy = new PlayerTableReadingStrategiesSolver();
            newStrategy.SetupPlayerData(playerData);
            return newStrategy;
        }
    }
}