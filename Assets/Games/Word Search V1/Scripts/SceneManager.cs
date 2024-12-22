using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WordSearch
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager I;

        [Header("Players")]
        [SerializeField] private Transform playerSpawnPos;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject botPrefab;
        [HideInInspector] public GameObject localPlayer;
        [HideInInspector] public GameObject enemyPlayer;


        [Header("Score")]
        public TextMeshProUGUI localScoreText;
        public TextMeshProUGUI enemyScoreText;
        private void Awake()
        {
            I = this;
        }

        private void Start()
        {
            GameManager.onMustPrepareScene.SubscribeAndInvokeIfTriggered(OnMustPrepareScene);
        }

        private void OnMustPrepareScene()
        {
            StartCoroutine(PrepareSceneCoroutine());
        }

        private IEnumerator PrepareSceneCoroutine()
        {
            //Spawn Players
            if (GameManager.isAgainstBot)
            {
            }
            else
            {
            }

            GameManager.I.OnPreparedScene();
            yield break;
        }

        public void OnTimeEnd()
        {

        }
    }
}
