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
        
        [SerializeField]
        private TMP_Text gameRoundNumberText;
    
        [SerializeField]
        private TMP_Text gameRoundsLeftCounterText;
    
        [SerializeField]
        private Button newGameButton;
        
        [SerializeField]
        private Button finishGameButton;

        private void OnEnable()
        {
            newGameButton.onClick.AddListener(OnNewGameClicked);
            finishGameButton.onClick.AddListener(OnFinishGameClicked);
        }
        
        private void OnDisable()
        {
            newGameButton.onClick.RemoveListener(OnNewGameClicked);
            finishGameButton.onClick.RemoveListener(OnFinishGameClicked);
        }
        
        private void OnNewGameClicked()
        {
            NewGameButtonClicked?.Invoke();            
        }
        
        private void OnFinishGameClicked()
        {
            FinishButtonClicked?.Invoke();
        }
    }
}
