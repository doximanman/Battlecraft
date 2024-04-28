using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static Settings current;

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button backButton;

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

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("Volume"))
            Volume = PlayerPrefs.GetFloat("Volume");

        current = this;
        MenuLogic.onSettingsTabAfter += (on) =>
        {
            // set slider if volume changed externally
            if(on) volumeSlider.SetValueWithoutNotify(volume);
        };

        volumeSlider.onValueChanged.AddListener((value) =>
        {
            Volume = value;
        });
    }
}
