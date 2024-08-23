using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private float volume;
    public float Volume
    {
        get { return volume; }
        set
        {
            volume = value;
            PlayerPrefs.SetFloat("Volume", volume);
        }
    }

    /// <summary>
    /// Run when settings tab is opened
    /// </summary>
    public void Open()
    {
        // set slider if volume changed externally
        volumeSlider.SetValueWithoutNotify(volume);
    }

    /// <summary>
    /// Run when settings tab is closed
    /// </summary>
    public void Close()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
            Volume = PlayerPrefs.GetFloat("Volume");

        volumeSlider.onValueChanged.AddListener((value) =>
        {
            Volume = value;
        });
    }
}
