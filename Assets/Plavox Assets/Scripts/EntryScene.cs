using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EntryScene : MonoBehaviour
{
    [SerializeField] private GameEntryPoint gameEntryPoint;

    [Header("Development UI")]
    [SerializeField] private GameObject developmentCanvas;
    [SerializeField] private TMP_InputField gameIdIF;
    [SerializeField] private Button botBtn;
    [SerializeField] private Button localBtn;
    [SerializeField] private Button enemyBtn;

    private void Awake()
    {
        gameEntryPoint.onDevInterruptionRequired += OnDevInterruptionRequired;
        //UI
        developmentCanvas.SetActive(false);
        gameIdIF.onValueChanged.AddListener(UI_OnGameIdChanged);
        botBtn.onClick.AddListener(() =>
        {
            UI_Play(3);
        });
        localBtn.onClick.AddListener(() =>
        {
            UI_Play(1);
        });
        enemyBtn.onClick.AddListener(() =>
        {
            UI_Play(2);
        });
    }

    private void OnDevInterruptionRequired()
    {
        developmentCanvas.SetActive(true);
        UI_OnGameIdChanged(gameIdIF.text);
    }

    private void Start()
    {
#if UNITY_EDITOR
        Caching.ClearCache();
        Debug.Log("EDITOR MODE: Entire cache was cleaned.");
#endif
        Screen.orientation = ScreenOrientation.Portrait;
    }

    private void UI_OnGameIdChanged(string value)
    {
        bool isRoomIdValid = !string.IsNullOrWhiteSpace(value);
        botBtn.interactable = isRoomIdValid;
        localBtn.interactable = isRoomIdValid;
        enemyBtn.interactable = isRoomIdValid;
    }

    /// <param name="target">1: Player 1, 2: Player 2, 3: Bot</param>
    private void UI_Play(int target)
    {
        if (string.Equals(gameIdIF.text, string.Empty))
            return;

        developmentCanvas.SetActive(false);
        gameEntryPoint.developmentParams.gameId = gameIdIF.text;
        GameManager.ignorePlayerPrefs = true;
        switch (target)
        {
            case 1:
                gameEntryPoint.StartAsPlayer1();
                break;
            case 2:
                gameEntryPoint.StartAsPlayer2();
                break;
            case 3:
                gameEntryPoint.StartAgainstBot();
                break;
        }
    }
}
