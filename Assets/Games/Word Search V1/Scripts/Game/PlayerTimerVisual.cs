using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WordSearch
{
    public class PlayerTimerVisual : MonoBehaviour
    {
        public static PlayerTimerVisual I;
        public Timer gameTimer;  // Reference to your Timer class
        public Image timerImage; // UI Image to visualize the timer
        public TextMeshProUGUI timerText;
        public float turnDuration = 10f; // Duration of each player's turn
        private bool gameStarted = false;


   //     public Color localPlayer = Color.blue; // First player's color
    //    public Color enemyPlayer = Color.red;  // Second player's color
        private void Awake()
        {
            I = this;
        }

        void Start()
        {
            gameTimer = new Timer();
            GameManager.onGameStart.SubscribeAndInvokeIfTriggered(OnGameStart);
        }

        private void OnGameStart()
        {
            gameStarted = true;
        }

        void Update()
        {
            if (!gameStarted)
                return;

            // Update the game timer
            gameTimer.UpdateTimer();

            // Calculate the fill value based on remaining time
            float fillValue = gameTimer.RemainingTime() / turnDuration;
            timerImage.fillAmount = fillValue;

            // Update the TextMeshPro text to show the remaining time as an integer
            int remainingSeconds = Mathf.CeilToInt(gameTimer.RemainingTime());
            timerText.text = remainingSeconds.ToString(); // Show remaining time as a whole number

            // Check if the timer has finished
            if (gameTimer.IsFinished())
            {
               // TurnManager.I.EndTurn();
            }
        }

        public void StartPlayerTurn()
        {
            // Set the timer to start counting for the current player's turn
            gameTimer.StartCounting(turnDuration);

            // Set the color of the timer image based on the current player's turn
          //  timerImage.color = TurnManager.I.isLocalPlayerTurn ? localPlayer : enemyPlayer;

            // Reset fill value to 1 at the start of each turn
            timerImage.fillAmount = 1f;

            // Optionally, update the UI to reflect whose turn it is
            timerText.text = turnDuration.ToString();
        }
        [SerializeField] private Animator timeBonusMessage;

        public void AddPoints()
        {
            gameTimer.AddPoints();
            timeBonusMessage.SetTrigger("+2");
        }
    }
}
