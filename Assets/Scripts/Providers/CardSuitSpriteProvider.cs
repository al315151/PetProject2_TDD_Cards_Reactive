using System.Collections.Generic;
using UnityEngine;

namespace Providers
{
    public class CardSuitSpriteProvider : MonoBehaviour
    {
        [Tooltip("Follow enum order: 0 is Swords, 1 is Coins, 2 is Cups, 3 is Clubs")]
        [SerializeField]
        private List<Sprite> cardSuitSprites;
        
        public Sprite GetCardSuitSprite(CardSuit suit)
        {
            return cardSuitSprites[(int)suit];
        }
    }
}
