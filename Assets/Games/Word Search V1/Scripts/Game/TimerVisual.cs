using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WordSearch
{
    public class TimerVisual : MonoBehaviour
    {
        public Timer gameTimer; // Reference to your Timer class
        public Image timerImage; // UI Image to visualize the timer
        public TextMeshProUGUI timerText;
        public float gameDuration = 120f; // Example: 2 minutes
        private bool gameStarted = false;

        void Start()
        {
            gameTimer = new Timer();
            GameManager.onGameStart.SubscribeAndInvokeIfTriggered(OnGameStart);
        }

        private void OnGameStart()
        {
            gameStarted = true;
            gameTimer.StartCounting(gameDuration);
            timerText.text = gameDuration.ToString();
        }
        void Update()
        {
            if (!gameStarted)
                return;
            // Update the game timer
            gameTimer.UpdateTimer();

            // Update the UI fillAmount based on the remaining time
            float fillValue = gameTimer.RemainingTime() / gameDuration;
            timerImage.fillAmount = fillValue;

            // Update the TextMeshPro text to show the remaining time as an integer
            int remainingSeconds = Mathf.CeilToInt(gameTimer.RemainingTime());
            timerText.text = remainingSeconds.ToString(); // Show remaining time as whole number

            // Optionally: Check if timer has finished
            if (gameTimer.IsFinished())
            {
                ScoreManager.I.CheckForGameEnd();
                gameStarted = false;
                timerText.text = "0"; // Show "0" when time is up
                SceneManager.I.OnTimeEnd();
                Debug.Log("Time's Up!");
            }
        }
    }
}