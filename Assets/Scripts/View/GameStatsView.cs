using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class GameStatsView : MonoBehaviour
    {
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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        private void OnNewGameClicked()
        {
            throw new NotImplementedException();
        }
        
        private void OnFinishGameClicked()
        {
            throw new NotImplementedException();
        }
    }
}
