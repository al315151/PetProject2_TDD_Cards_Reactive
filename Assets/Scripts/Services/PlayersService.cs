using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using PlayerPresenters;
using VContainer.Unity;

namespace Services
{
    public class PlayersService : IInitializable, IDisposable
    {
        private const int MaxCPUPlayers = 3;

        public Action OnPlayersInitialized;
        
        private List<PlayerPresenter> npcPlayersPresenters = new();
        
        private PlayerPresenter userPlayer;
        
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
            userPlayer = new PlayerPresenter(-1);

            for (var i = 0; i < numberOfCPUPlayers; i++) {
                var newCPU = userPlayer.Clone(npcPlayersPresenters.Count + 1) as PlayerPresenter;
                npcPlayersPresenters.Add(newCPU);
            }
        }

        public List<PlayerPresenter> GetAllPlayers()
        {
            var allPlayers = npcPlayersPresenters.ToList();
            allPlayers.Add(userPlayer);
            return allPlayers;
        }

        public List<PlayerData> GetAllPlayersData()
        {
            var data = new List<PlayerData>();
            foreach (var player in npcPlayersPresenters)
            {
                data.Add(player.GetPlayerData());
            }
            data.Add(userPlayer.GetPlayerData());
            return data;
        }

        public PlayerPresenter GetUserPlayer()
        {
            return userPlayer;
        }

        public List<PlayerData> GetCPUPlayersData()
        {
            var data = new List<PlayerData>();
            foreach (var player in npcPlayersPresenters)
            {
                data.Add(player.GetPlayerData());
            }
            return data;
        }

        public void ResetDataOnPlayers()
        {
            //Reset score and hands on every player.
            userPlayer.Reset();
            foreach (var player in npcPlayersPresenters)
            {
                player.Reset();
            }
        }
    }
}
