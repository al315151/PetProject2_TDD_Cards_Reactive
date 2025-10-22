using System.Collections;
using LifetimeScope;
using NUnit.Framework;
using Presenters;
using UnityEngine;
using UnityEngine.TestTools;
using VContainer;

namespace Tests
{
    public class PlayModeValidationTests
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator PlayModeValidationTest_PlayModeGameInitialStateIsReached()
        {
            var gameObject = new GameObject();
            var lifetimeScope = gameObject.AddComponent<ApplicationLifetimeScope>();
            yield return new WaitForSeconds(1);

            var generalGamePresenter = lifetimeScope.Container.Resolve<GeneralGamePresenter>();
            
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;

            generalGamePresenter.StartGameButtonPressed();
            
            
            
        }
    }
}
