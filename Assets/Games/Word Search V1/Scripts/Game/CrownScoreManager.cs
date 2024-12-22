using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace WordSearch
{

    public class CrownScoreManager : MonoBehaviourPun
    {
        [SerializeField] private TextMeshProUGUI localText;
        [SerializeField] private TextMeshProUGUI enemyText;
        public int localTextInt;
        public int enemyTextInt;
        public static CrownScoreManager I;
        private void Awake()
        {
            I = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            localTextInt = 0;
            enemyTextInt = 0;
            localText.text = localTextInt.ToString();
            enemyText.text = enemyTextInt.ToString();
        }
        public void AddToScore(bool local, int addition)
        {
            if (GameManager.isAgainstBot)
            {
                if (local)
                    localTextInt += addition;
                else
                    enemyTextInt += addition;

                localText.text = localTextInt.ToString();
                enemyText.text = enemyTextInt.ToString();
            }
            else
            {
                photonView.RPC("UpdateScore", RpcTarget.Others, local ? PhotonManager.localPlayer.ActorNumber : PhotonManager.enemyPlayer.ActorNumber, addition);
            }
        }
  
        [PunRPC]
        private void UpdateScore(int actorNumber, int addition)
        {
            bool scoreUpdated = false;

            if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
            {
                localTextInt += addition;
                scoreUpdated = true;
            }
            else
            {
                enemyTextInt += addition;
                scoreUpdated = true;
            }

            if (scoreUpdated)
            {
                localText.text = localTextInt.ToString();
                enemyText.text = enemyTextInt.ToString();
            }
        }

    }
}
