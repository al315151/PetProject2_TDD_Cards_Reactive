using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using VContainer.Unity;

namespace Services
{
    public class PlayersService : IInitializable, IDisposable
    {
        private const int MaxCPUPlayers = 3;

        public Action OnPlayersInitialized;
        
        private List<PlayerData> npcPlayersData = new();
        
        private PlayerData userPlayer;
        
        public void Initialize()
        {
            CreatePlayers(MaxCPUPlayers);
            OnPlayersInitialized?.Invoke();
        }

        public void Dispose()
        {
        }

        public void CreatePlayers(int numberOfCPUPlayers)
        {
            //Player will be -1. No player should be allowed to be 0.
            userPlayer = new PlayerData(-1);

            for (var i = 0; i < numberOfCPUPlayers; i++) {
                var newCPU = userPlayer.Clone(npcPlayersData.Count) as PlayerData;
                npcPlayersData.Add(newCPU);
            }
        }

        public List<PlayerData> GetAllPlayers()
        {
            var allPlayers = npcPlayersData.ToList();
            allPlayers.Add(userPlayer);
            return allPlayers;
        }

        public PlayerData GetUserPlayer()
        {
            return userPlayer;
        }

        public List<PlayerData> GetCPUPlayers()
        {
            return npcPlayersData;
        }

        public void ResetDataOnPlayers()
        {
            //Reset score and hands on every player.
            userPlayer.Reset();
            foreach (var player in npcPlayersData)
            {
                player.Reset();
            }
        }
    }
}
