using System;
using System.Collections;
using UnityEngine;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using TMPro;
using UnityEngine.Events;

public class SocketManager : MonoBehaviour
{
    public static SocketManager I;

    private static SocketIOUnity socket;

    [SerializeField] private string ip, port;
    public UnityAction<string> onBotMessage;
    private bool isConnected = false;

    void Awake()
    {
        I = this;
        ConnectToAI();
    }

    private void ConnectToAI()
    {
        // Initialize and connect to the Socket.IO server
        var uri = new Uri($"http://{ip}:{port}"); // Replace with your server address and port
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            EIO = 4,
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        // Register event handlers
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("Connected to Socket.IO server");
            isConnected = true;
        };
        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log($"Disconnected: {e}");
            isConnected = false;
        };
        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log($"Reconnecting attempt: {e}");
        };
        socket.OnError += (sender, e) =>
        {
            Debug.Log("Connection error: " + e);
        };
        socket.OnReconnectFailed += (sender, e) =>
        {
            Debug.Log("Connection error: " + e);
        };
        socket.OnReconnectError += (sender, e) =>
        {
            Debug.Log("Connection error: " + e);

        };

        socket.OnUnityThread("BOT-MSG", OnBotMessage);

        Debug.Log("Connecting to CHATBOT server...");
        socket.Connect();
    }

    private void OnBotMessage(SocketIOResponse response)
    {
        string message = response.GetValue<string>(0);
        onBotMessage?.Invoke(message);
    }

    public void SendMessageToBot(string message, string selectedChoice)
    {
        socket.Emit("USER-MSG", message, selectedChoice);
    }

    private void OnDestroy()
    {
        isConnected = false;
        if (socket != null)
        {
            socket.Disconnect();
            socket.Dispose();
            socket = null;
            Debug.Log("Disposed off socket");
        }
    }
}
