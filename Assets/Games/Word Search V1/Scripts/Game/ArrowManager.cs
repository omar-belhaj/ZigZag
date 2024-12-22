using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace WordSearch
{

public class ArrowManager : MonoBehaviourPun
{
       public    GameObject arrow;
        public void ShowArrow()
        {
            arrow.gameObject.GetComponent<SpriteRenderer>().color = TurnManager.I.isLocalPlayerTurn ? localPlayer : enemyPlayer;
            arrow.SetActive(true);
            //photonView.RPC("ShowArrowRPC", RpcTarget.All);
        }
        
        private void Update()
        {

           
        }
        public Color localPlayer = Color.blue; // First player's color
        public Color enemyPlayer = Color.red;  // Second player's color
        [PunRPC]
        public void ShowArrowRPC()
        {
            arrow.gameObject.GetComponent<SpriteRenderer>().color = TurnManager.I.isLocalPlayerTurn ? localPlayer : enemyPlayer;
            arrow.SetActive(true);
        }
    }
}
