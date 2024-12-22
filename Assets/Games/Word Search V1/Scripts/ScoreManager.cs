using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WordSearch
{
    [System.Serializable]
    public class PlayerScore
    {
        [SerializeField] private TextMeshProUGUI text;
        public int value { get; private set; }
        public void Set(int newValue)
        {
            value = newValue > 0 ? newValue : 0;
            text.text = value.ToString();
        }
        public void Add(int newValue)
        {
            if (value + newValue < 0)
            {
                Set(0);
                return;
            }
            Set(value + newValue);
        }
    }

    public class ScoreManager : MonoBehaviourPun
    {
        public static ScoreManager I;

        public PlayerScore localScore;
        public PlayerScore enemyScore;

        public UnityAction onScoreChanged;

        private void Awake()
        {
            I = this;
        }

        private void Start()
        {
            localScore.Set(0);
            enemyScore.Set(0);
        }

        public void AddToScore(bool local, int addition)
        {
            Debug.Log($"ADD to {(local?'l' :'e')}: {addition}");
            if (GameManager.isAgainstBot)
            {
                if (local)
                    localScore.Add(addition);
                else
                    enemyScore.Add(addition);
            }
            else
            {
                photonView.RPC("UpdateScore", RpcTarget.Others, local ? PhotonManager.localPlayer.ActorNumber : PhotonManager.enemyPlayer.ActorNumber, addition);
            }
            onScoreChanged?.Invoke();
              CheckForGameEnd();
        }

        [PunRPC]
        private void UpdateScore(int actorNumber, int addition)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
            {
                localScore.Add(addition);
            }
            else
            {
                enemyScore.Add(addition);
            }
            onScoreChanged?.Invoke();
               CheckForGameEnd();
        }

        public void CheckForGameEnd()
        {

            if (localScore.value >= 51)
                GameManager.I.Win(true);
            else if (enemyScore.value >= 51){
                GameManager.I.Win(false);
            }
        }
    }
}