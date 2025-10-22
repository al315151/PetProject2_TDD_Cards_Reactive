using Data;
using UnityEngine;
using VContainer.Unity;

namespace Presenters
{
    public class GeneralGamePresenter : IInitializable
    {
        private const int MaxCPUPlayers = 3;
        
        private GameManagerData gameManagerData;

        public void StartGameButtonPressed()
        {
            gameManagerData.StartGame();
        }

        public void Initialize()
        {
            gameManagerData ??= new GameManagerData();

            gameManagerData.CreateGame(MaxCPUPlayers);
        }
    }
}
