using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private Settings settings;

    [Serializable]
    public class SoundList : List<Sound> { }
    [Serializable]
    public class StringSoundsPair { public string type; public List<Sound> sounds; }

    // dictionary of type to sound list
    // for example: (player,(walk,jump,chop)).
    public List<StringSoundsPair> stringSoundsPairs;
    //public List<Sound> sounds;

    private List<Sound> Get(string type)
    {
        return stringSoundsPairs.First(s => s.type.Equals(type)).sounds;
    }

    private void Start()
    {
        instance = this;

        foreach (var pair in stringSoundsPairs)
            foreach (var sound in pair.sounds)
            {
                sound.SetSource(gameObject.AddComponent<AudioSource>());
                sound.settings = settings;
            }

    }

    public void PlayFrom(GameObject obj,string type,string name)
    {
        var sound = Get(type).Find(s => s.name == name);
        if (sound!=null)
        {
            sound.SetPosition(obj.transform.position);
            sound.PlayAtPosition();
        }
    }

    public void Play(string type, string name)
    {
        var sound = Get(type).Find(s => s.name == name);
        if (sound != null) sound.Play();
    }

    public void Stop(string type,string name)
    {
        var sound = Get(type).Find(s => s.name == name);
        if (sound != null) sound.Stop();
    }
}
