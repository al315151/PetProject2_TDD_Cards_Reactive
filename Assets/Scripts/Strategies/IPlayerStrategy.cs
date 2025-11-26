using Data;

namespace Strategies
{
    public interface IPlayerStrategy
    {
        public void SetupPlayerData(PlayerData playerData);
        public CardData ExecuteStrategy();
    }
}