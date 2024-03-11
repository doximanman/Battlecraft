using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudLogic : MonoBehaviour, IBiomeListener
{
    public Material plainsClouds;
    public Material iceClouds;
    public Material desertClouds;


    private Logic logic;
    private ParticleSystem particles;
    private ParticleSystemRenderer particleRenderer;


    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();

        logic.registerBiomeListener(this);

        particles=GetComponent<ParticleSystem>();
        particleRenderer = particles.GetComponent<ParticleSystemRenderer>();
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
                particleRenderer.material=plainsClouds;
                break;
            case (Logic.iceBiome):
                particleRenderer.material = iceClouds;
                break;
            case (Logic.desertBiome):
                particleRenderer.material = desertClouds;
                break;
        }
    }
}
