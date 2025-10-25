using System;
using System.Collections.Generic;
using Data;
using TMPro;
using UnityEngine;

namespace View
{
    public class PlayerView : MonoBehaviour, IObservable<KeyValuePair<CardSuit, int>>, IDisposable
    {
        [SerializeField]
        private TMP_Text playerScoreText;
        
        [SerializeField]
        private CardView[] cardViews;

        private IObserver<KeyValuePair<CardSuit, int>> playerInputObserver;
        
        private void OnEnable()
        {
            for (var i = 0; i < cardViews.Length; i++) {
                cardViews[i].OnCardButtonPressed += OnCardButtonPressed;
            }
        }
        
        private void OnDisable()
        {
            for (var i = 0; i < cardViews.Length; i++) {
                cardViews[i].OnCardButtonPressed -= OnCardButtonPressed;
            }
        }
        
        private void OnCardButtonPressed(CardSuit arg1, int arg2)
        {
            if (playerInputObserver == null) {
                return;
            }
            // Set card that called this event as blank!
            
            // Tell the observer what you found!
            playerInputObserver.OnNext(new KeyValuePair<CardSuit, int>(arg1, arg2));
        }
        
        public IDisposable Subscribe(IObserver<KeyValuePair<CardSuit, int>> observer)
        {
            playerInputObserver = observer;
            return this;
        }

        public void Dispose()
        {
            playerInputObserver = null;
        }
    }
}
