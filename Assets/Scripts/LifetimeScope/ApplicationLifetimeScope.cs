using System;
using Data;
using Factories;
using Presenters;
using Providers;
using Services;
using Training;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using View;

namespace LifetimeScope
{
    public class ApplicationLifetimeScope : VContainer.Unity.LifetimeScope
    {
        [SerializeField]
        private GeneralGameView generalGameView;

        [SerializeField]
        private PlayerView playerView;

        [SerializeField]
        private TableUIView tableUIView;

        [SerializeField]
        private CardSuitSpriteProvider cardSuitSpriteSpriteProvider;

        [SerializeField]
        private PlayerTrainingAgent playerTrainingAgent;

        protected override void Configure(IContainerBuilder builder)
        {
            // First bind data sources.
            builder.Register<GameManagerData>(Lifetime.Scoped).AsSelf();

            // Then bind view.
            builder.RegisterInstance(generalGameView).As<GeneralGameView>();
            builder.RegisterInstance(playerView).As<PlayerView>();
            builder.RegisterInstance(cardSuitSpriteSpriteProvider).As<CardSuitSpriteProvider>();

            //Force Monobehaviours to be injected even if not resolved yet using RegisterComponent!
            builder.RegisterComponent(tableUIView).AsSelf();

            // Then bind presenters which take care of managing both.
            builder.Register<PlayersService>(Lifetime.Scoped).As<PlayersService, IInitializable, IDisposable>();

            builder.Register<UserPlayerPresenter>(Lifetime.Scoped)
                .As<UserPlayerPresenter, IInitializable, IDisposable>();
            builder.Register<GameManagerPresenter>(Lifetime.Scoped)
                .As<GameManagerPresenter, IInitializable, IDisposable>();
            builder.Register<TableUIPresenter>(Lifetime.Scoped).As<TableUIPresenter, IInitializable, IDisposable>();
            builder.Register<StrategiesFactory>(Lifetime.Scoped).As<StrategiesFactory>();

            if (playerTrainingAgent != null)
            {
                builder.RegisterComponent(playerTrainingAgent).As<PlayerTrainingAgent, IDisposable>();
            }
        }
    }
}