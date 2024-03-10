using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour, IBiomeListener
{
    public readonly Color plainsBackground = new(0 / 255.0f, 175 / 255.0f, 255 / 255.0f);
    public readonly Color iceBackground = new(49 / 255.0f, 109 / 255.0f, 164 / 255.0f);
    public readonly Color desertBackground = new(81 / 255.0f, 214 / 255.0f, 255 / 255.0f);


    Logic logic;
    Camera thisCamera;

    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
        thisCamera = GetComponent<Camera>();

        // register to biome change
        logic.registerBiomeListener(this);

        // start biome
        OnBiomeChange(logic.GetStartBiome());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnBiomeChange(string biome)
    {
        switch (biome)
        {
            case (Logic.plainsBiome):
                thisCamera.backgroundColor = plainsBackground;
                break;
            case (Logic.desertBiome):
                thisCamera.backgroundColor = desertBackground;
                break;
            case (Logic.iceBiome):
                thisCamera.backgroundColor = iceBackground;
                break;

        }
    }

}
