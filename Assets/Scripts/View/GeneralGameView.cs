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

        [SerializeField]
        private Button restartGameButton;

        [SerializeField]
        private GameObject gameOverScreenContainer;

        [SerializeField]
        private TMP_Text gameWinnerText;

        private void Awake()
        {
            newGameButton.onClick.AddListener(OnNewGameClicked);
            startNextRoundButton.onClick.AddListener(OnStartNextRoundButtonClicked);
            restartGameButton.onClick.AddListener(() => SetGameOverScreen(false));
        }

        private void OnEnable()
        {
            SetRoundNumber(string.Empty);
            SetGameOverScreen(false);
        }

        private void OnDestroy()
        {
            newGameButton.onClick.RemoveListener(OnNewGameClicked);
            startNextRoundButton.onClick.RemoveListener(OnStartNextRoundButtonClicked);
            restartGameButton.onClick.RemoveListener(() => SetGameOverScreen(false));
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

        public void SetGameWinner(string gameWinner)
        {
            gameWinnerText.text = gameWinner;
        }

        public void SetGameOverScreen(bool value)
        {
            gameOverScreenContainer.SetActive(value);
        }
    }
}