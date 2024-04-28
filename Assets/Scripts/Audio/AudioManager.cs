using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public List<Sound> sounds;

    private void Start()
    {
        instance = this;

        foreach (var sound in sounds)
            sound.SetSource(gameObject.AddComponent<AudioSource>());
    }

    public void Play(string name)
    {
        var sound = sounds.Find(s => s.name == name);
        if (sound != null) sound.Play();
    }

    public void Stop(string name)
    {
        var sound = sounds.Find(s => s.name == name);
        if (sound != null) sound.Stop();
    }
}
