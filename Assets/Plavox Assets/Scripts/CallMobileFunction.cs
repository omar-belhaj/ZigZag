using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallMobileFunction : MonoBehaviour
{
    /*[DllImport("__Internal")]
    private static extern void _onMessageFromUnity(string jsonMessage);*/

    public static void SendJson(string jsonMessage)
    {
        Debug.Log($"CallMobileFunction({jsonMessage})");
#if UNITY_IOS && !UNITY_EDITOR
        //_onMessageFromUnity(jsonMessage);
#elif UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass androidPlugin = new AndroidJavaClass("com.plavox.gaming.UnityMessageListener");
        androidPlugin.CallStatic("onMessageFromUnity", jsonMessage);
#endif
    }
}
