using System;
using System.Collections.Generic;
using Data;
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
            cardInteractionButton.interactable = true;
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
            OnCardButtonPressed?.Invoke(cardSuit, cardNumber);
        }

        public void ResetCardGraphics()
        {
            cardSuitImage.sprite = null;
            cardNumberText.text = string.Empty;
            cardInteractionButton.interactable = false;
        }
        
        private Sprite GetSuitSprite(CardSuit cardSuit)
        {
            return cardSuitSprites[(int)cardSuit];
        }
    }
}
