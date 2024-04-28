using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button backButton;

    public float Volume
    {
        get { return volumeSlider.value; }
        set { volumeSlider.value = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
