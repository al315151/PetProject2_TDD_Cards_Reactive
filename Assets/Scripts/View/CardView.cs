using System;
using System.Collections.Generic;
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

        public Action OnCardButtonPressed;
        
        public void SetupCardGraphics(CardSuit cardSuit, string cardNumber)
        {
            cardNumberText.text = cardNumber;
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
            OnCardButtonPressed?.Invoke();
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
