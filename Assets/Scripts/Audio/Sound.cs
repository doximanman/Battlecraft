using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip audioClip;
    [Range(0f,1f)]
    [SerializeField] private float volume;

    [Range(0.1f,3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource audioSource;

    public float Volume => volume * Settings.current.Volume;

    public void SetSource(AudioSource source)
    {
        source.clip=audioClip;
        audioSource=source;
    }

    public void Play()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.pitch = pitch;
            audioSource.volume = Volume;
            audioSource.Play();
        }
    }

    public void Stop()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
