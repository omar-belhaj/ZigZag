using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager I;
    [SerializeField] private AudioSource audioSource;
    void Awake()
    {
        I = this;
    }

    public void PlaySFX(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayLoop(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopSound()
    {
        audioSource.Stop();
        audioSource.loop = false;
    }
}
