using Presenters;
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

      protected override void Configure(IContainerBuilder builder)
      {
         builder.RegisterInstance(generalGameView).As<GeneralGameView>();
         builder.RegisterInstance(playerView).As<PlayerView>();
         
         builder.Register<GeneralGamePresenter>(Lifetime.Scoped).As<GeneralGamePresenter, IInitializable>();
      }
   }
}
