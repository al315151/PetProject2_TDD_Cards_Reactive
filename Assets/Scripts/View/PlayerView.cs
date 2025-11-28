using System;
using System.Collections.Generic;
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

            ResetAllCardViews();
            SetPlayerScore(0);
        }

        private void OnDisable()
        {
            for (var i = 0; i < cardViews.Length; i++) {
                cardViews[i].OnCardButtonPressed -= OnCardButtonPressed;
            }
        }

        public void SetupCardViews(List<CardData> cards)
        {
            if (cards == null || cards.Count == 0) {
                // Rest all card views!
                ResetAllCardViews();
                return;
            }

            for (var i = 0; i < cardViews.Length; i++) {
                if (i >= cards.Count) {
                    cardViews[i].ResetCardGraphics();
                    continue;
                }

                cardViews[i].SetupCardGraphics(cards[i].CardSuit, cards[i].CardNumber);
            }
        }

        public IDisposable Subscribe(IObserver<KeyValuePair<CardSuit, int>> observer)
        {
            playerInputObserver = observer;
            return this;
        }

        public void SetPlayerScore(int score)
        {
            playerScoreText.text = score.ToString();
        }

        public void Dispose()
        {
            playerInputObserver = null;
        }

        private void ResetCardView(CardSuit arg1, int arg2)
        {
            for (var i = 0; i < cardViews.Length; i++) {
                if (cardViews[i].IsCardTheSame(arg1, arg2)) {
                    cardViews[i].ResetCardGraphics();
                }
            }
        }

        private void ResetAllCardViews()
        {
            for (var i = 0; i < cardViews.Length; i++) {
                cardViews[i].ResetCardGraphics();
            }
        }

        private void OnCardButtonPressed(CardSuit arg1, int arg2)
        {
            if (playerInputObserver == null) {
                return;
            }
            // Tell the observer what you found!
            playerInputObserver.OnNext(new KeyValuePair<CardSuit, int>(arg1, arg2));
        }

        public void RemoveCard(KeyValuePair<CardSuit, int> value)
        {
            // Set card that called this event as blank!
            ResetCardView(value.Key, value.Value);
        }
    }
}