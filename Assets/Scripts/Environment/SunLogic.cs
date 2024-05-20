using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunLogic : MonoBehaviour, IBiomeListener
{
    public float plainsScale = 1;
    public float iceScale = 0.7f;
    public float desertScale = 1.3f;

    // Start is called before the first frame update
    void Start()
    {
        Logic.RegisterBiomeListener(this);
    }

    public void OnBiomeChange(Biome biome)
    {
        switch (biome)
        {
            case (Biome.PLAINS):
                transform.localScale = plainsScale * Vector3.one;
                break;
            case (Biome.ICE):
                transform.localScale = iceScale * Vector3.one;
                break;
            case (Biome.DESERT):
                transform.localScale = -desertScale * Vector3.one;
                break;
        }
    }
}
