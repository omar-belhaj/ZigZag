using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager I;
    public static UnityEvent onConnected = new UnityEvent();
    public static UnityEvent<DisconnectCause> onDisconnected = new UnityEvent<DisconnectCause>();
    public static UnityEvent<bool, string> onRoomChanged = new UnityEvent<bool, string>();
    public static UnityEvent onPlayerListChanged = new UnityEvent();

    public static Player localPlayer;
    public static Player enemyPlayer;

    public static string Region => PhotonNetwork.CloudRegion;
    public static int ping { get; private set; }

    public static bool IsMaster => PhotonNetwork.IsMasterClient;

    private void Awake()
    {
        if (I)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            I = this;
        }
        onConnected = new UnityEvent();
        onDisconnected = new UnityEvent<DisconnectCause>();
        onRoomChanged = new UnityEvent<bool, string>();
        onPlayerListChanged = new UnityEvent();
        localPlayer = null;
        enemyPlayer = null;
    }

    private void OnDestroy()
    {
        Disconnect();
        onPlayerListChanged = null;
        onRoomChanged = null;
        onDisconnected = null;
        onConnected = null;
        enemyPlayer = null;
        localPlayer = null;
        enemyPlayer = null;
    }

    private void Update()
    {
        ping = PhotonNetwork.GetPing();
    }

    public void ConnectPhoton(string nickname)
    {
        Debug.Log($"Connecting to Photon as {nickname}...");
        PhotonNetwork.NickName = nickname;
#if UNITY_EDITOR
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
#else
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
#endif
        //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "asia";
        PhotonNetwork.SendRate = 45;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon");
        localPlayer = PhotonNetwork.LocalPlayer;
        onConnected?.Invoke();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        localPlayer = null;
        enemyPlayer = null;
        onDisconnected?.Invoke(cause);
    }

    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.NetworkClientState == ClientState.Joining)
        {
            Debug.Log("Can't join room: Already trying to join a room.");
            return;
        }

        Debug.Log($"Is joining room {roomName}");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = false;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room.");
        onRoomChanged?.Invoke(false, message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room {PhotonNetwork.CurrentRoom.Name}");
        Player[] otherPlayers = PhotonNetwork.PlayerListOthers;
        onRoomChanged?.Invoke(true, string.Empty);
        if (otherPlayers != null && otherPlayers.Length > 0 && string.Equals(otherPlayers[0].NickName, PhotonNetwork.NickName))
        {
            Debug.Log("Tried to join the game from a second point.");
            //Rely on the other instance to disconnect themselves and till then ignore their presence.
            return;
        }
        if (otherPlayers.Length > 0)
            OnPlayerEnteredRoom(PhotonNetwork.PlayerListOthers[0]);
    }

    public override void OnLeftRoom()
    {
        enemyPlayer = null;
        onRoomChanged?.Invoke(false, "Manually left the room.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (string.Equals(newPlayer.NickName, PhotonNetwork.NickName))
        {
            Debug.Log("This player joined from another point, will disconnect this one.");
            PhotonNetwork.Disconnect();
            return;
        }
        Debug.Log($"{newPlayer.NickName} joined the room");
        enemyPlayer = newPlayer;
        onPlayerListChanged?.Invoke();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} left the room");
        if (enemyPlayer != null)
        {
            enemyPlayer = null;
            onPlayerListChanged?.Invoke();
        }
    }

    public void Disconnect()
    {
        Debug.Log("Will disconnect manually");
        PhotonNetwork.Disconnect();
    }
}