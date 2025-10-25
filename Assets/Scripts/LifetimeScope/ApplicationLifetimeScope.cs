using System;
using Data;
using Presenters;
using Providers;
using Services;
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
      
      protected override void Configure(IContainerBuilder builder)
      {
         // First bind data sources.
         builder.Register<GameManagerData>(Lifetime.Scoped).AsSelf();
         
         // Then bind view.
         builder.RegisterInstance(generalGameView).As<GeneralGameView>();
         builder.RegisterInstance(playerView).As<PlayerView>();
         builder.RegisterInstance(cardSuitSpriteSpriteProvider).As<CardSuitSpriteProvider>();
         builder.RegisterInstance(tableUIView).As<TableUIView>();

         // Then bind presenters which take care of managing both.
         builder.Register<PlayersService>(Lifetime.Scoped).As<PlayersService, IInitializable, IDisposable>();
         builder.Register<GeneralGamePresenter>(Lifetime.Scoped).As<GeneralGamePresenter, IInitializable, IDisposable>();
         builder.Register<TableUIPresenter>(Lifetime.Scoped).As<TableUIPresenter, IInitializable, IDisposable>();
      }
   }
}
