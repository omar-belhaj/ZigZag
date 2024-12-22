using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace WordSearch
{
    public class TurnManager : MonoBehaviourPunCallbacks
    {
        public static TurnManager I;

        [SerializeField] private AudioClip gameTrack;
        [SerializeField] private Animator uiAnimator;

        [SerializeField] private GameObject localPlayerBackground; // Plane for local player
        [SerializeField] private GameObject opponentBackground;
        private void Awake()
        {
            I = this;
        }
        public bool isLocalPlayerTurn = true; // Start with local player

        private void Start()
        {
            GameManager.onGameStart.SubscribeAndInvokeIfTriggered(OnGameStart);

        }

        private void OnGameStart()
        {
            StartTurn(); // Start the first turn
            SoundManager.I.PlayLoop(gameTrack);
            uiAnimator.SetBool("isInitialized", true);
        }

        private void StartTurn()
        {
            if (GameManager.isAgainstBot)
            {
                isLocalPlayerTurn = true;
                uiAnimator.SetBool("isLocalTurn", isLocalPlayerTurn);
                TileManager.I.EnablePlayerInput(); // Prevent the enemy player from playing
                BotManager.I.DisableBotInput();
                PlayerTimerVisual.I.StartPlayerTurn();

            }
            else
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("play");
                    // Master client is the local player
                    isLocalPlayerTurn = true;
                    uiAnimator.SetBool("isLocalTurn", isLocalPlayerTurn);
                    TileManager.I.EnablePlayerInput(); // Allow the local player to play
                    PlayerTimerVisual.I.StartPlayerTurn(); // Start the timer for the local player
                }
                else
                {
                    Debug.Log("no play");
                    // The other player is the enemy player
                    isLocalPlayerTurn = false;
                    uiAnimator.SetBool("isLocalTurn", isLocalPlayerTurn);
                    TileManager.I.DisablePlayerInput(); // Prevent the enemy player from playing
                    PlayerTimerVisual.I.StartPlayerTurn(); // Start the timer for the enemy player
                }
            }
        }
        private void Update()
        {
            localPlayerBackground.SetActive(isLocalPlayerTurn);
            opponentBackground.SetActive(!isLocalPlayerTurn);

        }

        public void EndTurn()
        {
            // Switch turns and inform other player
            if (GameManager.isAgainstBot)
            {
                SwitchTurn();

            }
            else
            {
                isLocalPlayerTurn = !isLocalPlayerTurn; //Youssef sure mel ligne hedhi? thaharli lezm tetnaha
                photonView.RPC("SwitchTurn", RpcTarget.All);
            }

        }

        [PunRPC]
        private void SwitchTurn()
        {
            isLocalPlayerTurn = !isLocalPlayerTurn; // Switch turns
            uiAnimator.SetBool("isLocalTurn", isLocalPlayerTurn);
            if (PhotonNetwork.IsMasterClient)
            {
                HexagonTextSpawner.I.RespawnWords();
            }
            if (isLocalPlayerTurn)
            {
                TileManager.I.EnablePlayerInput(); // Enable input for local player
                if (GameManager.isAgainstBot)
                {
                    BotManager.I.DisableBotInput();
                }
                PlayerTimerVisual.I.StartPlayerTurn(); // Start the timer for the local player
            }
            else
            {
                TileManager.I.DisablePlayerInput(); // Disable input for enemy player
                if (GameManager.isAgainstBot)
                {
                    BotManager.I.EnableBotInput();
                }
                PlayerTimerVisual.I.StartPlayerTurn(); // Start the timer for the enemy player
            }
        }

    }
}