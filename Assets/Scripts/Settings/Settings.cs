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

    private void OnEnable()
    {
        // set slider if volume changed externally
        volumeSlider.SetValueWithoutNotify(volume);
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
