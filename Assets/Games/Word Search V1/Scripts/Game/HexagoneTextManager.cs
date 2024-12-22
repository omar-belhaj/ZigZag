using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

namespace WordSearch
{
    public class HexagoneTextManager : MonoBehaviourPun
    {
        public TextMeshProUGUI hexaText;
        public Vector3Int gridPosition;  // Optional: to store the grid position if needed
        public bool isLetterGuessed;
        public bool isHexOcupied;
        [SerializeField] private Color swipedColor;
        public char Letter => hexaText.text[0];

        public static HexagoneTextManager I;

        private Coroutine uncolorCor;


        private void Awake()
        {
            I = this;
        }
        void Start()
        {
            isLetterGuessed = false;  // Reset guessed state
            RemoveHexText();
        }

        [PunRPC]
        public void PunSetHexText(string letter)
        {
            if (hexaText != null && !isHexOcupied)  // Only set the letter if the hex is not occupied
            {
                hexaText.text = letter;
                isHexOcupied = true;  // Mark the hex as occupied
            }
        }

        public void SetHexText(string letter)
        {
            if (!isHexOcupied)  // Only send the RPC if the hex is not already occupied
            {
                photonView.RPC("PunSetHexText", RpcTarget.All, letter); // Sync the letter with all players
            }
        }

        [PunRPC]
        public void PunRemoveHexText()
        {
            if (hexaText != null)
            {
                isHexOcupied = false;
                isLetterGuessed = false;
                hexaText.text = "";  // Clear the hex text
                ResetArrows();
                gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }

        public void RemoveHexText()
        {
            if (isHexOcupied)  // Only send the RPC if the hex is occupied
            {
                photonView.RPC("PunRemoveHexText", RpcTarget.All);  // Sync the removal with all players
            }
        }
        public void ColorTile()
        {
            SoundManager.I.PlaySFX(TileManager.I.colorSFX);
            gameObject.GetComponent<SpriteRenderer>().color = swipedColor;
        }
        public void ColorGuessedTile()
        {
            photonView.RPC("PunColorGuessedTile", RpcTarget.All);
        }
        [PunRPC]
        public void PunColorGuessedTile()
        {
            isLetterGuessed = true;
            gameObject.GetComponent<SpriteRenderer>().color = swipedColor;
        }
        public GameObject[] arrows;
        public void ResetArrows()
        {
            foreach (GameObject arrow in arrows)
            {
                arrow.SetActive(false);
            }
        }
        public void UncolorTile(int delayMultiplier)
        {
            uncolorCor = StartCoroutine(UnColorTile(delayMultiplier));
        }

        private static WaitForSeconds Delay = new WaitForSeconds(0.05f);
        private IEnumerator UnColorTile(int delayMultiplier)
        {
            for (int i = 0; i < delayMultiplier; i++)
                yield return Delay;
            gameObject.GetComponent<SpriteRenderer>().color = TileManager.I.originalColor;
            SoundManager.I.PlaySFX(TileManager.I.uncolorSFX);
        }

    }
}