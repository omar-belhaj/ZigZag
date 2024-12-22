using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[RequireComponent(typeof(SpriteRenderer))]
public class RemoteSpriteHolder : MonoBehaviour, IRemoteAssetReceiver
{
    public void OnAssetDownloaded(AssetReference assetReference)
    {
        GetComponent<SpriteRenderer>().sprite = (Sprite)assetReference.Asset;
    }
}
