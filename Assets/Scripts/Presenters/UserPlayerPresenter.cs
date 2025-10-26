using System;
using System.Collections.Generic;
using Data;
using Services;
using VContainer.Unity;
using View;

public class UserPlayerPresenter : IInitializable, IDisposable
{
    private readonly PlayersService playersService;
    private readonly PlayerView playerView;

    private PlayerData userPlayerData;

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
    }

    public void Dispose()
    {
        playersService.OnPlayersInitialized -= OnPlayersInitialized;
        if (userPlayerData != null)
        {
            userPlayerData.PlayerHandUpdated -= OnPlayerHandUpdated;
            userPlayerData.PlayerScoreUpdated -= OnPlayerScoreUpdated;
        }
        
    }

    private void OnPlayerHandUpdated(List<CardData> list)
    {
        playerView.SetupCardViews(list);
    }

    private void OnPlayersInitialized()
    {
        userPlayerData = playersService.GetUserPlayer();
        SubscribeToPlayerDataChanges();
    }

    private void SubscribeToPlayerDataChanges()
    {
        userPlayerData.PlayerHandUpdated += OnPlayerHandUpdated;
        userPlayerData.PlayerScoreUpdated += OnPlayerScoreUpdated;
    }
        
    private void OnPlayerScoreUpdated()
    {
        playerView.SetPlayerScore(userPlayerData.GetScore());
    }
}
