using System;
using System.Collections.Generic;
using Data;
using PlayerPresenters;
using R3;
using Services;
using VContainer.Unity;
using View;

namespace Presenters
{
    public class UserPlayerPresenter : IInitializable, IDisposable, IObserver<KeyValuePair<CardSuit, int>>
    {
        private readonly PlayersService playersService;
        private readonly PlayerView playerView;

        private PlayerPresenter userPlayerPresenter;
        private PlayerData userPlayerData;

        private IDisposable playerHandDisposable;
        private IDisposable playerScoreDisposable;

        private IDisposable subscriptionToViewInteractionDisposable;

        public UserPlayerPresenter(
            PlayersService playersService,
            PlayerView playerView)
        {
            this.playersService = playersService;
            this.playerView = playerView;

            playersService.OnPlayersInitialized += OnPlayersInitialized;
        }

        public void Initialize()
        {
            SubscribeToViewEvents();
        }

        public void Dispose()
        {
            playersService.OnPlayersInitialized -= OnPlayersInitialized;
            playerHandDisposable?.Dispose();
            playerScoreDisposable?.Dispose();

            subscriptionToViewInteractionDisposable?.Dispose();
        }

        private void OnPlayersInitialized()
        {
            //Get PlayerData and subscribe to its changes!
            userPlayerPresenter = playersService.GetUserPlayer();
            userPlayerData = userPlayerPresenter.GetPlayerData();

            SubscribeToPlayerDataChanges();
        }

        private void SubscribeToPlayerDataChanges()
        {
            playerHandDisposable = userPlayerData.PlayerHand.Subscribe(handList => {
                playerView.SetupCardViews(handList);
            });
            playerScoreDisposable =
                userPlayerData.PlayerScore.Subscribe(score => { playerView.SetPlayerScore(score); });
        }

        private void SubscribeToViewEvents()
        {
            subscriptionToViewInteractionDisposable = playerView.Subscribe(this);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(KeyValuePair<CardSuit, int> value)
        {
            if (userPlayerPresenter.IsPlayerTurn == false) {
                return;
            }
            userPlayerPresenter.PlayCardFromUserHand(value.Key, value.Value);

            playerView.RemoveCard(value);
        }
    }
}