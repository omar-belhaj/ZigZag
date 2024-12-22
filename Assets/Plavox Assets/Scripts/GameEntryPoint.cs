using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameEntryPoint : MonoBehaviour
{
    public static GameEntryPoint I;
    private const string SCENE_DIR = "Assets/Scenes/";
    private string scenePath = string.Empty;
    public UnityAction onDevInterruptionRequired;
    private bool isInitialized = false;
    private bool isDevMode = false;

    [Header("Development")]
    [Tooltip("Game parameters used for testing in the Unity Editor.")]
    public MultiplayerGameParams developmentParams;

    [Header("Download UI")]
    [SerializeField] private GameObject sceneLoadingCanvas;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject downloadBarParent;
    [SerializeField] private Image downloadBar;
    [SerializeField] private Button retryButton;

    private void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        retryButton.onClick.AddListener(RetryDownload);
        sceneLoadingCanvas.SetActive(true);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        if (isInitialized)
            yield break;
        isDevMode = true;
        onDevInterruptionRequired?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CallMobileFunction.SendJson(string.Concat('{', "\"event\" : \"back_pressed\"", '}'));
        }
    }

    public void InitializeGameFromMobile(string jsonParams)
    {
        StartCoroutine(InitializeGameFromMobileCoroutine(jsonParams));
    }

    private IEnumerator InitializeGameFromMobileCoroutine(string jsonParams)
    {
        isInitialized = true;
        //Parse game params
        MultiplayerGameParams gameParams = JsonUtility.FromJson<MultiplayerGameParams>(jsonParams);
        if (gameParams == null)
        {
            Debug.Log("Failed to parse JSON parameters.");
            yield break;
        }
        Debug.Log($"Received Game Params JSON: {jsonParams}");

        //Load Game Scene
        scenePath = string.Concat(SCENE_DIR, gameParams.gameId, ".unity");
        int sceneBuildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);
        if (sceneBuildIndex == -1) //Did not find scene
        {
            Debug.Log($"Did not find scene {scenePath}");
            yield return LoadRemoteSceneAsync();
        }
        else
        {
            if (SceneManager.GetActiveScene().buildIndex != sceneBuildIndex)
                yield return SceneManager.LoadSceneAsync(sceneBuildIndex);
            OnSceneLoaded();
        }

        //Initialize Game Manager
        while (GameManager.I == null)
        {
            yield return null;
        }
        GameManager.I.Initialize(gameParams);
    }

    private void RetryDownload()
    {
        StartCoroutine(LoadRemoteSceneAsync());
    }

    private IEnumerator LoadRemoteSceneAsync()
    {
        //UI
        downloadBarParent.SetActive(true);
        retryButton.gameObject.SetActive(false);

        //Download scene
        Debug.Log($"Will try to download the scene {scenePath}");
        AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(scenePath);

        //Track download progress
        downloadBar.fillAmount = 0;
        loadingText.text = "Loading...";
        while (!handle.IsDone)
        {
            downloadBar.fillAmount = handle.PercentComplete;
            yield return null;
        }

        //On scene downloaded
        if (handle.Status == AsyncOperationStatus.Succeeded) //Downloaded scene successfully
        {
            OnSceneLoaded();
        }
        else //Failed to download scene
        {
            loadingText.text = "Failed to Load!";
            retryButton.gameObject.SetActive(true);
            downloadBarParent.SetActive(false);

            if (isDevMode)
            {
                onDevInterruptionRequired?.Invoke();
            }
        }
    }

    private void OnSceneLoaded()
    {
        Debug.Log("Scene successfully loaded");
        Destroy(sceneLoadingCanvas);
    }

    public void StartAsPlayer1()
    {
        if (string.Equals(developmentParams.gameId, string.Empty))
        {
            developmentParams.gameId = SceneManager.GetActiveScene().name;
        }
        InitializeGameFromMobile(JsonUtility.ToJson(developmentParams));
    }

    public void StartAsPlayer2()
    {
        if (string.Equals(developmentParams.gameId, string.Empty))
        {
            developmentParams.gameId = SceneManager.GetActiveScene().name;
        }
        string player2Id = developmentParams.player2Id;
        developmentParams.player2Id = developmentParams.player1Id;
        developmentParams.player1Id = player2Id;

        InitializeGameFromMobile(JsonUtility.ToJson(developmentParams));
    }

    public void StartAgainstBot()
    {
        if (string.Equals(developmentParams.gameId, string.Empty))
        {
            developmentParams.gameId = SceneManager.GetActiveScene().name;
        }
        developmentParams.player2Id = string.Concat("a99", developmentParams.player2Id);
        InitializeGameFromMobile(JsonUtility.ToJson(developmentParams));
    }

    private void OnDestroy()
    {
        I = null;
    }
}
