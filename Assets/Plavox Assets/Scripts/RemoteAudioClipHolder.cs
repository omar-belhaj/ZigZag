using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[RequireComponent(typeof(AudioClip))]
public class RemoteAudioClipHolder : MonoBehaviour, IRemoteAssetReceiver
{
    [SerializeField] private AssetReference audioClip;
    public void OnAssetDownloaded(AssetReference assetReference)
    {
        if (audioClip.AssetGUID != assetReference.AssetGUID)
            return;

        AudioSource source = GetComponent<AudioSource>();
        source.clip = (AudioClip)assetReference.Asset;
        if (source.playOnAwake)
        {
            source.Play();
        }
        Destroy(this);
    }
}