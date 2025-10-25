using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using R3;
using UnityEditor;

namespace Data
{
    public class PlayerData : IPlayerPrototype, IObservable<KeyValuePair<int, CardData>>, IDisposable
    {
        public static int MaxHandSize = 3;

        public ReactiveProperty<List<CardData>> PlayerHand { get; private set; }

        public int PlayerHandSize => PlayerHand.CurrentValue.Count;
        public int PlayerId => id;

        private readonly int id;
        
        int score = 0;        
        private bool inputEnabled = false;

        private IObserver<KeyValuePair<int, CardData>> roundDataObserver;
        
        public PlayerData(int id = 0)
        {
            this.id = id;
            PlayerHand = new ReactiveProperty<List<CardData>>(new List<CardData>());
            inputEnabled = true;
        }

        public void AddCardToHandFromDeck(DeckData deck)
        {
            if (PlayerHand.CurrentValue.Count >= MaxHandSize) {
                return;
            }
            var card = deck.GetTopCardFromDeck();
            // If cards have run out, do not add anything!
            if (card == null) {
                return;
            }
            PlayerHand.CurrentValue.Add(card);
        }

        public IPlayerPrototype Clone(int id)
        {
            return new PlayerData(id);
        }

        public void RequestCardFromPlayer()
        {
            //For now, choose card at random.
            var randomIndex = new Random().Next(PlayerHand.Value.Count);
            var randomCard = PlayerHand.CurrentValue[randomIndex];
            PlayerHand.CurrentValue.Remove(randomCard);
            roundDataObserver?.OnNext(new KeyValuePair<int, CardData>(id, randomCard));
        }

        public void AddCard(CardData cardData)
        {
            PlayerHand.CurrentValue.Add(cardData);
        }

        public int GetScore()
        {
            return score;
        }

        public void AddScoreToPlayer(int roundScore)
        {
            score += roundScore;
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

        private async Task TriggerPlayCard()
        {
#if UNITY_EDITOR
            //Allow for tests to not await on delays.
            if (EditorApplication.isPlaying == false) {
                RequestCardFromPlayer();
                return;
            }
#endif
            //Play mode + input enabled: normal gameplay flow.
            if (inputEnabled) {
                return;
            }
            
            await Task.Delay(1);
            RequestCardFromPlayer();
        }
    }
}
