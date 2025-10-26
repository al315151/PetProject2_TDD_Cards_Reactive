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

        public Action<List<CardData>> PlayerHandUpdated;
        public Action PlayerScoreUpdated;

        private List<CardData> PlayerHand = new();
        private int PlayerScore;

        public int PlayerHandSize => PlayerHand.Count;
        public int PlayerId => id;

        private readonly int id;
        
        
        private bool inputEnabled = false;

        private IObserver<KeyValuePair<int, CardData>> roundDataObserver;
        
        public PlayerData(int id = 0)
        {
            this.id = id;
            PlayerHand = new List<CardData>();
            PlayerScore = 0;
            inputEnabled = true;
        }

        public void AddCardToHandFromDeck(DeckData deck)
        {
            if (PlayerHand.Count >= MaxHandSize) {
                return;
            }
            var card = deck.GetTopCardFromDeck();
            // If cards have run out, do not add anything!
            if (card == null) {
                return;
            }
            PlayerHand.Add(card);
            PlayerHandUpdated?.Invoke(PlayerHand);
        }

        public IPlayerPrototype Clone(int id)
        {
            return new PlayerData(id);
        }

        public void RequestCardFromPlayer()
        {
            //For now, choose card at random.
            var randomIndex = new Random().Next(PlayerHand.Count);
            var randomCard = PlayerHand[randomIndex];
            PlayerHand.Remove(randomCard);
            PlayerHandUpdated?.Invoke(PlayerHand);
            roundDataObserver?.OnNext(new KeyValuePair<int, CardData>(id, randomCard));
        }

        public void AddCard(CardData cardData)
        {
            PlayerHand.Add(cardData);
            PlayerHandUpdated?.Invoke(PlayerHand);
        }

        public int GetScore()
        {
            return PlayerScore;
        }

        public void AddScoreToPlayer(int roundScore)
        {
            PlayerScore += roundScore;
            PlayerScoreUpdated?.Invoke();
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
            //Play mode + input enabled: normal gameplay flow for the player (pressing the buttons).
            if (inputEnabled && id == -1) {
                return;
            }
            
            await Task.Delay(1);
            RequestCardFromPlayer();
        }

        public void Reset()
        {
            PlayerScore = 0;
            PlayerHand.Clear();
            PlayerHandUpdated?.Invoke(PlayerHand);
        }
    }
}
