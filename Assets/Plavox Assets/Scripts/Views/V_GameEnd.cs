using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExampleGame.Views
{
    public class V_GameEnd : MonoBehaviour
    {
        [SerializeField] private GameObject v_gameEnd;
        [SerializeField] private GameObject v_won;
        [SerializeField] private GameObject v_lost;

        private void OnEnable()
        {
            GameManager.onGameEnd.SubscribeAndInvokeIfTriggered(OnGameEnded);
        }

        private void OnDisable()
        {
            GameManager.onGameEnd?.Unsubscribe(OnGameEnded);
        }

        public void OnGameEnded(bool won)
        {
            v_gameEnd.SetActive(true);
            v_won.SetActive(won);
            v_lost.SetActive(!won);
        }
    }
}
