using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip audioClip;
    [Range(0f,2f)]
    [SerializeField] private float volume;

    [Range(0.1f,3f)]
    public float pitch;

    [HideInInspector]
    public AudioSource audioSource;

    public float Volume => volume * Settings.current.Volume;

    public float PlayTime => audioClip.length;

    public void SetSource(AudioSource source)
    {
        source.clip=audioClip;
        audioSource=source;

        audioSource.panStereo = 0;
    }

    public void SetPosition(Vector3 position) => audioSource.transform.position = position;

    public void PlayAtPosition()
    {
        audioSource.spatialBlend = 1;
        audioSource.spatialize = true;
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.pitch = pitch;
            audioSource.volume = Volume;
            audioSource.Play();
        }
    }

    public void Play()
    {
        audioSource.spatialBlend = 0;
        audioSource.spatialize = false;
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
