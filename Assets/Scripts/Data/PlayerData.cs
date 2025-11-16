using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using R3;
using UnityEditor;
using UnityEngine;

namespace Data
{
    public class PlayerData : IPlayerPrototype, IObservable<KeyValuePair<int, CardData>>, IDisposable
    {
        public static int MaxHandSize = 3;

        public ReactiveProperty<List<CardData>> PlayerHand { get; private set; }
        public ReactiveProperty<int> PlayerScore { get; private set; }

        public bool IsPlayerTurn => roundDataObserver != null;

        public int PlayerHandSize => PlayerHand.Value.Count;
        public int PlayerId => id;

        private readonly int id;
        
        
        private bool inputEnabled = false;

        private IObserver<KeyValuePair<int, CardData>> roundDataObserver;
        
        public PlayerData(int id = 0)
        {
            this.id = id;
            PlayerHand = new ReactiveProperty<List<CardData>>(new List<CardData>());
            PlayerScore = new ReactiveProperty<int>(0);
            inputEnabled = true;
        }

        public void AddCardToHandFromDeck(DeckData deck)
        {
            if (PlayerHand.Value.Count >= MaxHandSize) {
                return;
            }
            var card = deck.GetTopCardFromDeck();
            // If cards have run out, do not add anything!
            if (card == null) {
                return;
            }
            Debug.Log($"Player: {PlayerId} Draws card: {card.CardSuit.ToString()} , {card.CardNumber.ToString()}");
            Debug.Log($"Player: {PlayerId} player hand size: {PlayerHandSize}");
            PlayerHand.Value.Add(card);
            PlayerHand.OnNext(PlayerHand.Value);
        }

        public IPlayerPrototype Clone(int id)
        {
            return new PlayerData(id);
        }

        public void RequestCardFromPlayer()
        {
            //For now, choose card at random.
            var randomIndex = new System.Random().Next(PlayerHand.Value.Count);
            var randomCard = PlayerHand.Value[randomIndex];
            SendCardFromHandToRound(randomCard);
        }

        public void DrawCardsUntilMaxAllowed(DeckData deck)
        {
            var numberOfCardsToBeDrawn = MaxHandSize - PlayerHand.Value.Count;
            for (int i = 0; i < numberOfCardsToBeDrawn; i++)
            {
                AddCardToHandFromDeck(deck);
            }
        }

        public void PlayCardFromUserHand(CardSuit cardSuit, int number)
        {
            if (roundDataObserver == null)
            {
                return;
            }

            //find card in hand that matches the one 
            for (int i = 0; i < PlayerHand.CurrentValue.Count; i++)
            {
                if (PlayerHand.Value[i].CardSuit == cardSuit && PlayerHand.Value[i].CardNumber == number)
                {
                    var selectedCard = PlayerHand.Value[i];
                    SendCardFromHandToRound(selectedCard);
                    break;
                }
            }
        }

        public void AddCard(CardData cardData)
        {
            PlayerHand.CurrentValue.Add(cardData);
            PlayerHand.OnNext(PlayerHand.CurrentValue);
        }

        public int GetScore()
        {
            return PlayerScore.Value;
        }

        public void AddScoreToPlayer(int roundScore)
        {
            PlayerScore.Value += roundScore;
            PlayerScore.OnNext(PlayerScore.CurrentValue);
        }

        public void DisablePlayerInput()
        {
            inputEnabled = false;
        }

        public void EnablePlayerInput()
        {
            inputEnabled = true;
        }
        
        public IDisposable Subscribe(IObserver<KeyValuePair<int, CardData>> observer)
        {
            roundDataObserver = observer;
            _ = TriggerPlayCard();
            return this;
        }

        public void Dispose()
        {
            roundDataObserver = null;
        }

        private void SendCardFromHandToRound(CardData cardData)
        {
            PlayerHand.Value.Remove(cardData);
            PlayerHand.OnNext(PlayerHand.CurrentValue);
            roundDataObserver?.OnNext(new KeyValuePair<int, CardData>(id, cardData));
        }

        private async Task TriggerPlayCard()
        {
#if UNITY_EDITOR
            //Allow for tests to not await on delays.
            if (EditorApplication.isPlaying == false) {
                RequestCardFromPlayer();
                return;
            }
#endif
            //Play mode + input enabled: normal gameplay flow for the player (pressing the buttons).
            if (inputEnabled && id == -1) {
                return;
            }
            
            await Task.Delay(1);
            RequestCardFromPlayer();
        }

        public void Reset()
        {
            PlayerScore.Value = 0;
            PlayerScore.OnNext(PlayerScore.CurrentValue);

            PlayerHand.Value.Clear();
            PlayerHand.OnNext(PlayerHand.CurrentValue);
        }
    }
}
