using System;
using Data;
using Presenters;
using R3;
using Services;
using UnityEngine;
using VContainer.Unity;
using View;

public class UserPlayerPresenter : IInitializable, IDisposable
{
    const int PlayerId = -1;

    private readonly PlayersService playersService;
    private readonly PlayerView playerView;

    private PlayerData userPlayerData;

    private IDisposable playerHandDisposable;

    public UserPlayerPresenter(
        PlayersService playersService,
        PlayerView playerView)
    {
        this.playersService = playersService;
        this.playerView = playerView;
    }

    public void Initialize()
    {
        playersService.OnPlayersInitialized += OnPlayersInitialized;
    }

    public void Dispose()
    {
        playersService.OnPlayersInitialized -= OnPlayersInitialized;
        playerHandDisposable?.Dispose();
    }

    private void OnPlayersInitialized()
    {
        //Get PlayerData and subscribe to its changes!
        var players = playersService.GetAllPlayers();

        for (int i = 0; players.Count > 0; i++)
        {
            if (players[i].PlayerId == PlayerId)
            {
                userPlayerData = players[i];
            }
        }

        SubscribeToPlayerDataChanges();
    }

    private void SubscribeToPlayerDataChanges()
    {
        playerHandDisposable = userPlayerData.PlayerHand.Subscribe(handList => { playerView.SetupCardViews(handList);});
    }

}
