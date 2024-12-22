using ExampleGame.Views;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviourPun
{
    public static GameManager I;

    public static OneTriggerEvent onMustPrepareScene;
    public static OneTriggerEvent onGameStart;
    public static OneTriggerEvent<bool> onGameEnd;
    public static UnityAction<GameState> onStateChanged;

    /// <summary> Is MasterClient or playing against a Bot. </summary>
    public static bool isDecisionMaker { get; private set; }
    public static bool isAgainstBot { get; private set; }
    public static bool ignorePlayerPrefs;

    public static int localScore;
    public static int enemyScore;

    public static string player1Id => I.gameParams.player1Id;
    public static string player2Id => I.gameParams.player2Id;

    [Header("UI")]
    public ScreenOrientation screenOrientation = ScreenOrientation.Portrait;

    [Header("SFX")]
    [SerializeField] private AudioClip victorySFX;
    [SerializeField] private AudioClip defeatSFX;

    private MultiplayerGameParams gameParams;

    public enum GameState
    {
        WaitingForManagerInitialization,
        AlreadyPlayedBefore,
        ConnectingToServer,
        JoiningRoom,
        WaitingForOpponent,
        /// <summary>
        /// Spawning players, setting up turns, etc..
        /// </summary>
        PreparingScene,
        AwaitingStartTimer,
        Playing,
        ConcludedResults, //Either won or lost
        NetworkDisconnection
    }
    public static GameState state { get; private set; } = GameState.WaitingForManagerInitialization;

    private void Awake()
    {
        I = this;

        //Events
        onMustPrepareScene = new OneTriggerEvent();
        onGameStart = new OneTriggerEvent();
        onGameEnd = new OneTriggerEvent<bool>();
        onStateChanged = null;
        Debug.Log("Reset on state changed");

        //State
        state = GameState.WaitingForManagerInitialization;

        //Game Orientation
        Screen.orientation = screenOrientation;
    }

    private void Start()
    {
        PhotonManager.onConnected.AddListener(Photon_OnConnected);
        PhotonManager.onDisconnected.AddListener(Photon_OnDisconnected);
        PhotonManager.onRoomChanged.AddListener(Photon_OnRoomChanged);
        PhotonManager.onPlayerListChanged.AddListener(Photon_OnPlayerListChanged);

        GameEntryPoint.I.StartAgainstBot();
    }

    private void OnDestroy()
    {
        onStateChanged = null;
        onMustPrepareScene = null;
        onGameEnd = null;
        onGameStart = null;
        isAgainstBot = false;
        isDecisionMaker = false;
        I = null;
    }

    public void Initialize(MultiplayerGameParams gameParams)
    {
        if (state != GameState.WaitingForManagerInitialization)
        {
            Debug.Log("Already initialized the game. You must restart to change the player");
            return;
        }

#if !UNITY_EDITOR
            if (!ignorePlayerPrefs && PlayerPrefs.GetInt(gameParams.matchId, -1) == 1)
            {
                SetState(GameState.AlreadyPlayedBefore);
                return;
            }
#endif

        Application.runInBackground = true;

        this.gameParams = gameParams;

        if (gameParams.player2Id.StartsWith("b99") || gameParams.player2Id.StartsWith("a99"))
        {
            isAgainstBot = true;
            isDecisionMaker = true;
        }
        else
        {
            isAgainstBot = true;
        }

        SetState(GameState.ConnectingToServer);
        PhotonManager.I.ConnectPhoton(gameParams.player1Id);
    }

    private void Photon_OnConnected()
    {
        if (state == GameState.ConnectingToServer) //First time I connect
        {
            PhotonManager.I.JoinRoom(gameParams.matchId);
            SetState(GameState.JoiningRoom);
        }
    }

    private void SetState(GameState newState)
    {
        GameState currentState = GameManager.state;
        GameManager.state = newState;

        if (newState == GameState.Playing && currentState != GameState.Playing)
        {
            onGameStart.Invoke();
        }
        if (newState == GameState.PreparingScene && currentState != GameState.PreparingScene)
        {
            Debug.Log("Awaiting Scene Preperation...");
            onMustPrepareScene.Invoke();
        }

        onStateChanged?.Invoke(newState);
    }

    private void Photon_OnDisconnected(DisconnectCause cause)
    {
        if (state == GameState.ConcludedResults || state == GameState.NetworkDisconnection)
            return;
        if (cause == DisconnectCause.DisconnectByClientLogic)
        {
            SetState(GameState.NetworkDisconnection);
            return;
        }

        if (state == GameState.JoiningRoom ||
            state == GameState.ConnectingToServer ||
            state == GameState.WaitingForOpponent ||
            state == GameState.AwaitingStartTimer) //Reconnect
        {
            PhotonManager.I.ConnectPhoton(gameParams.player1Id);
            SetState(GameState.ConnectingToServer);
            return;
        }

        if (state == GameState.Playing)
        {
            OnGameEnded(false, true);
            return;
        }
    }

    private void Photon_OnRoomChanged(bool isInRoom, string error)
    {
        if (isInRoom)
        {
            isDecisionMaker = isAgainstBot || PhotonNetwork.IsMasterClient;
            //wait for opponent
            SetState(GameState.WaitingForOpponent);
            if (isAgainstBot)
            {
                gameObject.AddComponent<BotDelayedJoin>().onTimerFinished += () =>
                {
                    Debug.Log("Bot Joined!");
                    SetState(GameState.PreparingScene);
                };
            }
        }
        else
        {
            //nothing
        }
    }


    public void Win(bool won)
    {
        if (isAgainstBot)
        {
            OnGameEnded(won, false);
        }
        else
        {
            photonView.RPC("SetWinner", RpcTarget.All, won ? PhotonManager.localPlayer.ActorNumber : PhotonManager.enemyPlayer.ActorNumber);
        }
    }

    [PunRPC]
    private void SetWinner(int actorNumber)
    {
        if (PhotonManager.localPlayer.ActorNumber == actorNumber)
        {
            OnGameEnded(true, false);
        }
        else
        {
            OnGameEnded(false, false);
        }
    }

    private void OnGameEnded(bool won, bool enemyAborted)
    {
        if (state == GameState.ConcludedResults)
            return;

        Debug.Log($"Game ended (won={won}, enemy aborted={enemyAborted})");
        SetState(GameState.ConcludedResults);
        onGameEnd.Invoke(won);
        if (won)
        {
            //Send API call
            SoundManager.I.PlaySFX(victorySFX);
            ConcludeResults.I.SendMatchResults(enemyAborted, gameParams.player1Id, gameParams.player2Id, localScore, enemyScore, gameParams.returnURL, gameParams.token);
        }
        else
        {
            SoundManager.I.PlaySFX(defeatSFX);
            PhotonManager.I.Disconnect();
            string winnerId = won ? gameParams.player1Id : gameParams.player2Id;
            string loserId = won ? gameParams.player2Id : gameParams.player1Id;
            if (isAgainstBot)
                ConcludeResults.I.SendMatchResults(false, winnerId, loserId, localScore, enemyScore, gameParams.returnURL, gameParams.token);
        }
    }

    public void OnPreparedScene()
    {
        Debug.Log("Scene was prepared successfully.");
        SetState(GameState.AwaitingStartTimer);
    }

    private void Photon_OnPlayerListChanged()
    {
        if (PhotonManager.enemyPlayer != null)
        {
            //Ready to start game
            SetState(GameState.PreparingScene);
        }
        else //Enemy left
        {
            if (state == GameState.AwaitingStartTimer) //Wait for opponent to reconnect
                SetState(GameState.WaitingForOpponent);
            if (state == GameState.Playing || state == GameState.PreparingScene) //Win
                OnGameEnded(true, true);
        }
    }

    public void StartGame()
    {
        if (state != GameState.AwaitingStartTimer)
        {
            Debug.Log($"Can't start the game at this state: {state}");
            return;
        }
        Debug.Log("Game started!");
        PlayerPrefs.SetInt(gameParams.matchId, 1);
        SetState(GameState.Playing);
    }
}