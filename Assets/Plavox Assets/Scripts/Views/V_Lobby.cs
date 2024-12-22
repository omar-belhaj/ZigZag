using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class V_Lobby : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI startTimerText;
    [SerializeField] private GameObject v_lobby;
    [SerializeField] private GameObject v_timer;

    [Header("Development")]
    [SerializeField] private GameObject v_dev;
    [SerializeField] private Button botBtn;
    [SerializeField] private Button localBtn;
    [SerializeField] private Button enemyBtn;

    private float timer;

    private IEnumerator Start()
    {
        GameManager.onStateChanged += OnStateChanged;
        GameManager.onGameEnd.SubscribeAndInvokeIfTriggered(OnGameEnd);
        v_timer.SetActive(false);
        v_lobby.SetActive(true);

        yield return new WaitForSeconds(3f);
        if (GameManager.state != GameManager.GameState.WaitingForManagerInitialization)
            yield break;

        GameManager.ignorePlayerPrefs = true;
        v_dev.SetActive(true);
        botBtn.onClick.AddListener(() =>
        {
            GameEntryPoint.I.StartAgainstBot();
            v_dev.SetActive(false);
        });
        localBtn.onClick.AddListener(() =>
        {
            GameEntryPoint.I.StartAsPlayer1();
            v_dev.SetActive(false);
        });
        enemyBtn.onClick.AddListener(() =>
        {
            GameEntryPoint.I.StartAsPlayer2();
            v_dev.SetActive(false);
        });
    }

    private void OnGameEnd(bool won)
    {
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (GameManager.onStateChanged != null)
            GameManager.onStateChanged -= OnStateChanged;
        GameManager.onGameEnd?.Unsubscribe(OnGameEnd);
    }

    private void OnStateChanged(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.ConnectingToServer:
                stateText.text = "Connecting to servers";
                break;
            case GameManager.GameState.AlreadyPlayedBefore:
                stateText.text = "This match has been played";
                break;
            case GameManager.GameState.JoiningRoom:
                stateText.text = "Joining room";
                break;
            case GameManager.GameState.WaitingForOpponent:
                v_lobby.SetActive(true);
                v_timer.SetActive(false);
                stateText.text = "Waiting for opponent";
                break;
            case GameManager.GameState.PreparingScene:
                stateText.text = "Preparing Game...";
                break;
            case GameManager.GameState.AwaitingStartTimer:
                StartCoroutine(LaunchGameCoroutine());
                break;
            case GameManager.GameState.NetworkDisconnection:
                v_lobby.SetActive(true);
                v_timer.SetActive(false);
                stateText.text = "Disconnected";
                break;
        }
    }

    private const string GO = "GO";

    private IEnumerator LaunchGameCoroutine()
    {
        v_lobby.SetActive(false);
        v_timer.SetActive(true);
        timer = 0;
        startTimerText.text = "3";

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 1)
            {
                startTimerText.text = GO;
            }
            else
            {
                startTimerText.text = $"{Mathf.CeilToInt(timer - 1)}";
            }
            yield return null;
        }

        v_timer.SetActive(false);
        GameManager.I.StartGame();
    }
}