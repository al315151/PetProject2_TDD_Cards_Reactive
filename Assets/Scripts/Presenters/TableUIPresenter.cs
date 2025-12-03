using System;
using System.Collections.Generic;
using Data;
using R3;
using Services;
using VContainer.Unity;
using View;

namespace Presenters
{
    public class TableUIPresenter : IInitializable, IDisposable
    {
        private readonly GameManagerPresenter gameManagerPresenter;
        private readonly GameManagerData gameManagerData;
        private readonly PlayersService playersService;
        private readonly TableUIView tableUIView;

        public CardSuit SelectedCardSuit => selectedSuit;

        private CardSuit selectedSuit;

        private IDisposable playerDisposables;
        private IDisposable playedCardsDisposables;

        public TableUIPresenter(
            GameManagerPresenter gameManagerPresenter,
            GameManagerData gameManagerData,
            PlayersService playersService,
            TableUIView tableUIView)
        {
            this.gameManagerPresenter = gameManagerPresenter;
            this.gameManagerData = gameManagerData;
            this.playersService = playersService;
            this.tableUIView = tableUIView;

            playersService.OnPlayersInitialized += OnPlayersInitialized;
        }

        public void Initialize()
        {
            gameManagerPresenter.OnGameStarted += OnGameStarted;
            gameManagerPresenter.OnGameRoundStartSetupFinished += OnGameRoundStarted;
            gameManagerPresenter.OnGameRoundFinished += OnGameRoundFinished;
            tableUIView.RequestDeckCardCountUpdate += OnRequestDeckCardCountUpdate;

            tableUIView.ResetGameGraphics();
        }

        private void OnGameRoundFinished()
        {
            var currentRound = gameManagerData.GetCurrentRoundData();
            if (currentRound != null) {
                var winnerId = currentRound.RoundWinnerId;
                var winnerString = winnerId == -1 ? "You" : winnerId.ToString();

                tableUIView.SetRoundWinnerText(winnerString);
            }

            playedCardsDisposables?.Dispose();
        }

        private void OnGameRoundStarted()
        {
            ResetRoundGraphics();
            SetupRoundRelatedData();
        }

        private void OnRequestDeckCardCountUpdate()
        {
            tableUIView.SetCardsLeftInDeck(gameManagerData.CurrentDeckSize());
        }

        private void OnGameStarted(CardSuit initialCardSuit)
        {
            ResetRoundGraphics();
            tableUIView.ResetGameGraphics();
            tableUIView.SetupSelectedCardSuitVisuals(initialCardSuit);
        }

        private void OnPlayersInitialized()
        {
            SubscribeToPlayerRelatedData();
        }

        private void ResetRoundGraphics()
        {
            tableUIView.ResetCardViews();
            tableUIView.SetRoundWinnerText(string.Empty);
        }

        public void Dispose()
        {
            gameManagerPresenter.OnGameStarted -= OnGameStarted;
            gameManagerPresenter.OnGameRoundStartSetupFinished -= OnGameRoundStarted;
            gameManagerPresenter.OnGameRoundFinished -= OnGameRoundFinished;
            playersService.OnPlayersInitialized -= OnPlayersInitialized;
            tableUIView.RequestDeckCardCountUpdate -= OnRequestDeckCardCountUpdate;
            playerDisposables?.Dispose();
        }

        private void SetupRoundRelatedData()
        {
            var currentGameRound = gameManagerData.GetCurrentRoundData();
            tableUIView.SetPlayerRoundOrderText(currentGameRound.PlayerOrder);
            playedCardsDisposables =
                currentGameRound.PlayedCardsInRound.Subscribe(playedCards =>
                    tableUIView.SetupRoundCardsView(playedCards));
        }

        private void SubscribeToPlayerRelatedData()
        {
            var players = playersService.GetAllPlayersData();
            var disposablesBuilder = new DisposableBuilder();
            foreach (var player in players) {
                var newDisposable = player.PlayerScore.Subscribe(_ => tableUIView.SetNPCPlayerScores(GetScorePerCPU()));
                disposablesBuilder.Add(newDisposable);
            }
            playerDisposables = disposablesBuilder.Build();
        }

        private List<KeyValuePair<int, int>> GetScorePerCPU()
        {
            var cpuPlayers = playersService.GetCPUPlayersData();
            var result = new List<KeyValuePair<int, int>>();

            foreach (var player in cpuPlayers) {
                result.Add(new KeyValuePair<int, int>(player.PlayerId, player.GetScore()));
            }
            return result;
        }
    }
}