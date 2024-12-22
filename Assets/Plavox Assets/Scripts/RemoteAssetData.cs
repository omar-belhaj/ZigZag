using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "New Remote Asset Data", menuName = "Plavox Games/Remote Asset")]
public class RemoteAssetData : ScriptableObject
{
    public string key;
    public GameObject[] holders;
    [HideInInspector] public object asset;
}

public interface IRemoteAssetReceiver
{
    //public void OnAssetDownloaded(RemoteAssetData remoteAssetData);
    public void OnAssetDownloaded(AssetReference assetReference);
}
