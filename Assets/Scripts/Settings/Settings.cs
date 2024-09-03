using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum DifficultyType { EASY, NORMAL, HARD };
public static class DifficultyMethods
{
    public static float GetMultiplier(this DifficultyType difficulty)
    {
        return difficulty switch
        {
            DifficultyType.EASY => 0.5f,
            DifficultyType.HARD => 1.5f,
            _ => 1.0f,
        };
    }
}

public class Settings : MonoBehaviour
{
    public static Settings current;

    #region Difficulty
    [SerializeField] DifficultyDropdown difficultyDropdown;
    private DifficultyType difficulty;
    public DifficultyType Difficulty
    {
        get => difficulty;
        set
        {
            difficulty = value;
            PlayerPrefs.SetInt("Difficulty", (int)difficulty);
        }
    }
    #endregion

    #region Volume
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
    #endregion

    #region Framerate Target
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
    #endregion


    private void Awake()
    {
        current = this;
    }

    private void OnEnable()
    {
        // set slider if volume changed externally
        volumeSlider.SetValueWithoutNotify(volume);
        framerateSlider.SetValueWithoutNotify(framerateTarget);
        difficultyDropdown.SetValueWithoutNotify(difficulty);
    }

    // Start is called before the first frame update
    void Start()
    {
        // no vsync (otherwise framerate target couldn't work)
        QualitySettings.vSyncCount = 0;
        if (!sliderAffectThisScene)
        {
            // set framerate target manually in case it is not 
            Application.targetFrameRate = _framerateTarget;
        }

        if (PlayerPrefs.HasKey("Volume"))
            Volume = PlayerPrefs.GetFloat("Volume");
        if (PlayerPrefs.HasKey("FrameRate"))
            FramerateTarget = PlayerPrefs.GetInt("FrameRate");
        if (PlayerPrefs.HasKey("Difficulty"))
            Difficulty = (DifficultyType)PlayerPrefs.GetInt("Difficulty");
            

        volumeSlider.onValueChanged.AddListener((value) =>
        {
            Volume = value;
        });

        framerateSlider.onValueChanged.AddListener((value) =>
        {
            FramerateTarget = (int)value;
        });

        difficultyDropdown.AddValueChangeListener((value) =>
        {
            Difficulty = value;
        });
    }
}
