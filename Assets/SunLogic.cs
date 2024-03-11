using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunLogic : MonoBehaviour, IBiomeListener
{
    public float plainsScale = 1;
    public float iceScale = 0.7f;
    public float desertScale = 1.3f;

    private Logic logic;

    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();

        logic.registerBiomeListener(this);
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
                transform.localScale = plainsScale * Vector3.one;
                break;
            case (Logic.iceBiome):
                transform.localScale = iceScale * Vector3.one;
                break;
            case (Logic.desertBiome):
                transform.localScale = -desertScale * Vector3.one;
                break;
        }
    }
}
