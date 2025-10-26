using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class GeneralGameView : MonoBehaviour
    {
        public Action NewGameButtonClicked;
        
        public Action FinishButtonClicked;

        public Action StartNextRoundButtonClicked;
        
        [SerializeField]
        private TMP_Text gameRoundNumberText;
    
        [SerializeField]
        private Button startNextRoundButton;
    
        [SerializeField]
        private Button newGameButton;
        
        [SerializeField]
        private Button finishGameButton;

        private void OnEnable()
        {
            newGameButton.onClick.AddListener(OnNewGameClicked);
            finishGameButton.onClick.AddListener(OnFinishGameClicked);
            startNextRoundButton.onClick.AddListener(OnStartNextRoundButtonClicked);
        }

        private void OnDisable()
        {
            newGameButton.onClick.RemoveListener(OnNewGameClicked);
            finishGameButton.onClick.RemoveListener(OnFinishGameClicked);
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
        
        private void OnFinishGameClicked()
        {
            FinishButtonClicked?.Invoke();
        }

        public void SetRoundNumber(string roundNumber)
        {
            gameRoundNumberText.text = roundNumber;
        }
    }
}
