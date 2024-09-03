using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider framerateSlider;
    [SerializeField] private TMP_Text framerateText;
    public bool sliderAffectThisScene = true;
    [SerializeField]
    [Range(60, 240)]
    [Tooltip("Ignored if \"slider affect this scene\" is on")] private int _framerateTarget;

    private int framerateTarget;
    public int FramerateTarget
    {
        get => framerateTarget;
        set
        {
            framerateTarget = value;
            if (sliderAffectThisScene)
                Application.targetFrameRate = value;
            framerateText.text = value.ToString();
            PlayerPrefs.SetInt("FrameRate", framerateTarget);
        }
    }

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
        framerateSlider.SetValueWithoutNotify(framerateTarget);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!sliderAffectThisScene)
        {
            // set framerate target manually
            Application.targetFrameRate = _framerateTarget;
        }

        if (PlayerPrefs.HasKey("Volume"))
            Volume = PlayerPrefs.GetFloat("Volume");
        if (PlayerPrefs.HasKey("FrameRate"))
            FramerateTarget = PlayerPrefs.GetInt("FrameRate");

        volumeSlider.onValueChanged.AddListener((value) =>
        {
            Volume = value;
        });

        framerateSlider.onValueChanged.AddListener((value) =>
        {
            FramerateTarget = (int)value;
        });
    }
}
