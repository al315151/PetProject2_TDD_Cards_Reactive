using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Strategies;
using UnityEditor;
using UnityEngine;

namespace PlayerPresenters
{
    public class PlayerPresenter : IPlayerPrototype, IObservable<KeyValuePair<int, CardData>>, IDisposable
    {
        public bool IsPlayerTurn => roundDataObserver != null;

        public int PlayerId => playerData.PlayerId;

        private readonly PlayerData playerData;

        private IObserver<KeyValuePair<int, CardData>> roundDataObserver;

        private IPlayerStrategy playerStrategy;

        public PlayerPresenter(int id = 0)
        {
            playerData = new PlayerData(id);
        }

        public void SetPlayerStrategy(PlayerStrategyType playerStrategyType, IPlayerStrategy playerStrategy)
        {
            this.playerStrategy = playerStrategy;
            playerData.SetPlayerStrategy(playerStrategyType);
        }

        public void AddCardToHandFromDeck(DeckData deck)
        {
            if (playerData.PlayerHand.Value.Count >= PlayerData.MaxHandSize) {
                return;
            }
            var card = deck.GetTopCardFromDeck();
            // If cards have run out, do not add anything!
            if (card == null) {
                return;
            }
            //Debug.Log($"Player: {PlayerId} Draws card: {card.CardSuit.ToString()} , {card.CardNumber.ToString()}");
            //Debug.Log($"Player: {PlayerId} player hand size: {PlayerHandSize}");
            playerData.AddCard(card);
        }

        public IPlayerPrototype Clone(int id)
        {
            return new PlayerPresenter(id);
        }

        private void RequestCardToPlayFromStrategy()
        {
            CardData chosenCard;

            chosenCard = playerStrategy != null 
                ? playerStrategy.ExecuteStrategy() 
                : GetCardFromFallbackStrategy();

            SendCardFromHandToRound(chosenCard);
        }

        private CardData GetCardFromFallbackStrategy()
        {
            var randomIndex = new System.Random().Next(playerData.PlayerHand.Value.Count);
            var randomCard = playerData.PlayerHand.Value[randomIndex];
            return randomCard;
        }

        public void DrawCardsUntilMaxAllowed(DeckData deck)
        {
            var numberOfCardsToBeDrawn = PlayerData.MaxHandSize - playerData.PlayerHand.Value.Count;
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

            var playerHand = playerData.PlayerHand.CurrentValue;
            //find card in hand that matches the one 
            for (int i = 0; i < playerHand.Count; i++)
            {
                if (playerHand[i].CardSuit == cardSuit && playerHand[i].CardNumber == number)
                {
                    var selectedCard = playerHand[i];
                    SendCardFromHandToRound(selectedCard);
                    break;
                }
            }
        }

        public PlayerData GetPlayerData() 
        {
            return playerData; 
        }

        public void TestAddCard(CardData cardData)
        {
           playerData.AddCard(cardData);
        }       

        public void AddScoreToPlayer(int roundScore)
        {
            Debug.Log($"Player: {PlayerId} adds score: {roundScore} , to current score:{playerData.PlayerScore.Value}");
            playerData.AddScoreToPlayer(roundScore);
        }

        public void DisablePlayerInput()
        {
            playerData.DisablePlayerInput();
        }

        public void EnablePlayerInput()
        {
            playerData.EnablePlayerInput();
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
                RequestCardToPlayFromStrategy();
                return;
            }
#endif
            //Play mode + input enabled: normal gameplay flow for the player (pressing the buttons).
            if (playerData.InputEnabled && playerData.PlayerId == -1) {
                return;
            }
            
            await Task.Delay(1);
            RequestCardToPlayFromStrategy();
        }

        public void Reset()
        {
            playerData.Reset();
        }

        private void SendCardFromHandToRound(CardData cardData)
        {
            playerData.UpdateDataAfterPlayPhase(cardData);
            roundDataObserver?.OnNext(new KeyValuePair<int, CardData>(playerData.PlayerId, cardData));
        }
    }
}
