using System;
using Data;
using R3;
using Services;
using VContainer.Unity;
using View;

public class UserPlayerPresenter : IInitializable, IDisposable
{
    private readonly PlayersService playersService;
    private readonly PlayerView playerView;

    private PlayerData userPlayerData;

    private IDisposable playerHandDisposable;
    private IDisposable playerScoreDisposable;

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
        playerHandDisposable?.Dispose();
        playerScoreDisposable?.Dispose();
    }

    private void OnPlayersInitialized()
    {
        //Get PlayerData and subscribe to its changes!
        userPlayerData = playersService.GetUserPlayer();

        SubscribeToPlayerDataChanges();
    }

    private void SubscribeToPlayerDataChanges()
    {
        playerHandDisposable = userPlayerData.PlayerHand.Subscribe(handList => { playerView.SetupCardViews(handList);});
        playerScoreDisposable = userPlayerData.PlayerScore.Subscribe(score => { playerView.SetPlayerScore(score); });
    }

}
