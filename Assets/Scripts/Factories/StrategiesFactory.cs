using Data;
using Strategies;

namespace Factories
{
    public class StrategiesFactory
    {
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