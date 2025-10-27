using System;
using System.Collections.Generic;
using Data;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class CardView : MonoBehaviour
    {
        [SerializeField]
        private Image cardSuitImage;
        
        [SerializeField]
        private TMP_Text cardNumberText;

        [SerializeField]
        private Button cardInteractionButton;
        
        [SerializeField]
        private List<Sprite> cardSuitSprites;

        public Action<CardSuit, int> OnCardButtonPressed;

        private CardSuit cardSuit;
        private int cardNumber;
        
        public void SetupCardGraphics(CardSuit cardSuit, int cardNumber)
        {
            this.cardSuit = cardSuit;
            this.cardNumber = cardNumber;
            cardNumberText.text = cardNumber.ToString();
            cardSuitImage.sprite = GetSuitSprite(cardSuit);
        }

        private void OnEnable()
        {
            cardInteractionButton.onClick.AddListener(OnCardPressed);
        }

        private void OnDisable()
        {
            cardInteractionButton.onClick.RemoveListener(OnCardPressed);
        }

        private void OnCardPressed()
        {
            if (cardSuitImage.sprite == null || cardNumber == 0)
            {
                return;
            }

            OnCardButtonPressed?.Invoke(cardSuit, cardNumber);
        }

        public void ResetCardGraphics()
        {
            cardSuitImage.sprite = null;
            cardNumberText.text = string.Empty;
            cardNumber = 0;
        }
        
        private Sprite GetSuitSprite(CardSuit cardSuit)
        {
            return cardSuitSprites[(int)cardSuit];
        }

        public bool IsCardTheSame(CardSuit cardSuit, int cardNumber)
        {
            return this.cardSuit == cardSuit && this.cardNumber == cardNumber;
        }

    }
}
