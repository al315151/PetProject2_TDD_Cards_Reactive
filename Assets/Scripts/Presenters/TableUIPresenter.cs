using System;
using System.Collections.Generic;
using Data;
using NUnit.Framework;
using R3;
using Services;
using UnityEngine;
using VContainer.Unity;
using View;

namespace Presenters
{
    public class TableUIPresenter : IInitializable, IDisposable
    {
        private readonly GeneralGamePresenter gameManagerPresenter;
        private readonly GameManagerData gameManagerData;
        private readonly PlayersService playersService;
        private readonly TableUIView tableUIView;

        public CardSuit SelectedCardSuit => selectedSuit;

        private CardSuit selectedSuit;
        private IDisposable playerDisposables;

        public TableUIPresenter(
            GeneralGamePresenter gameManagerPresenter,
            GameManagerData gameManagerData,
            PlayersService playersService,
            TableUIView tableUIView)
        {
            this.gameManagerPresenter = gameManagerPresenter;
            this.gameManagerData = gameManagerData;
            this.playersService = playersService;
            this.tableUIView = tableUIView;            
        }

        public void Initialize()
        {
            gameManagerPresenter.OnGameStarted += OnGameStarted;
            gameManagerPresenter.OnGameRoundStarted += OnGameRoundStarted;
            playersService.OnPlayersInitialized += OnPlayersInitialized;
            tableUIView.RequestDeckCardCountUpdate += OnRequestDeckCardCountUpdate;
        }

        private void OnGameRoundStarted()
        {
            tableUIView.ResetCardViews();
            SetupRoundRelatedData();
        }

        private void OnRequestDeckCardCountUpdate()
        {
            tableUIView.SetCardsLeftInDeck(gameManagerData.CurrentDeckSize());
        }

        private void OnGameStarted(CardSuit initialCardSuit)
        {
            tableUIView.SetupSelectedCardSuitVisuals(initialCardSuit);
        }

        private void OnPlayersInitialized()
        {
            SubscribeToPlayerRelatedData();
        }

        public void Dispose()
        {
            gameManagerPresenter.OnGameStarted -= OnGameStarted;
            gameManagerPresenter.OnGameRoundStarted -= OnGameRoundStarted;
            playersService.OnPlayersInitialized -= OnPlayersInitialized;
            tableUIView.RequestDeckCardCountUpdate -= OnRequestDeckCardCountUpdate;
            playerDisposables?.Dispose();
        }

        private void SetupRoundRelatedData()
        {
            var currentGameRound = gameManagerData.GetCurrentRound();
            tableUIView.SetPlayerRoundOrderText(currentGameRound.PlayerOrder);
        }

        private void SubscribeToPlayerRelatedData()
        {
            var players = playersService.GetCPUPlayers();
            var disposablesBuilder = new DisposableBuilder();
            foreach (var player in players)
            {
                var newDisposable = player.PlayerScore.Subscribe(_ => tableUIView.SetNPCPlayerScores(GetScorePerCPU()));
                disposablesBuilder.Add(newDisposable);
            }
            playerDisposables = disposablesBuilder.Build();
        }

        private List<KeyValuePair<int, int>> GetScorePerCPU()
        {
            var cpuPlayers = playersService.GetCPUPlayers();
            var result = new List<KeyValuePair<int, int>>();

            foreach(var player in cpuPlayers)
            {
                result.Add(new KeyValuePair<int, int>(player.PlayerId, player.GetScore()));
            }
            return result;
        }
    }
}
