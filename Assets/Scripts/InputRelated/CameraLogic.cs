using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour, IBiomeListener
{
    public Color plainsBackground = new(0 / 255.0f, 175 / 255.0f, 255 / 255.0f);
    public Color iceBackground = new(49 / 255.0f, 109 / 255.0f, 164 / 255.0f);
    public Color desertBackground = new(81 / 255.0f, 214 / 255.0f, 255 / 255.0f);

    Camera thisCamera;

    // Start is called before the first frame update
    void Start()
    {
        thisCamera = GetComponent<Camera>();

        // register to biome change
        Logic.RegisterBiomeListener(this);

        // start biome
        OnBiomeChange(Logic.GetStartBiome());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBiomeChange(Biome biome)
    {
        switch (biome)
        {
            case (Biome.PLAINS):
                thisCamera.backgroundColor = plainsBackground;
                break;
            case (Biome.DESERT):
                thisCamera.backgroundColor = desertBackground;
                break;
            case (Biome.ICE):
                thisCamera.backgroundColor = iceBackground;
                break;

        }
    }

}
