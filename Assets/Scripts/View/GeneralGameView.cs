using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class GeneralGameView : MonoBehaviour
    {
        public Action NewGameButtonClicked;

        public Action StartNextRoundButtonClicked;
        
        [SerializeField]
        private TMP_Text gameRoundNumberText;
    
        [SerializeField]
        private Button startNextRoundButton;
    
        [SerializeField]
        private Button newGameButton;

        private void OnEnable()
        {
            newGameButton.onClick.AddListener(OnNewGameClicked);
            startNextRoundButton.onClick.AddListener(OnStartNextRoundButtonClicked);
            SetRoundNumber(string.Empty);
        }

        private void OnDisable()
        {
            newGameButton.onClick.RemoveListener(OnNewGameClicked);
            startNextRoundButton.onClick.RemoveListener(OnStartNextRoundButtonClicked);
        }

        private void OnStartNextRoundButtonClicked()
        {
            StartNextRoundButtonClicked?.Invoke();
        }

        private void OnNewGameClicked()
        {
            NewGameButtonClicked?.Invoke();            
        }

        public void SetRoundNumber(string roundNumber)
        {
            gameRoundNumberText.text = roundNumber;
        }
    }
}
