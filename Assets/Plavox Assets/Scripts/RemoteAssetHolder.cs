using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RemoteAssetHolder : MonoBehaviour
{
    public AssetReference[] references;

    public void OnDownloaded()
    {
        IRemoteAssetReceiver[] receivers = GetComponents<IRemoteAssetReceiver>();
        foreach (IRemoteAssetReceiver receiver in receivers)
        {
            foreach (AssetReference asset in references)
            {
                Debug.Log("ref: " + asset.Asset.GetType() + " " + asset.AssetGUID);
                receiver.OnAssetDownloaded(asset);
            }
        }
        Destroy(this);
    }
}
