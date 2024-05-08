using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudLogic : MonoBehaviour, IBiomeListener
{
    public Material plainsClouds;
    public Material iceClouds;
    public Material desertClouds;

    private ParticleSystem particles;
    private ParticleSystemRenderer particleRenderer;


    // Start is called before the first frame update
    void Start()
    {

        Logic.RegisterBiomeListener(this);

        particles=GetComponent<ParticleSystem>();
        particleRenderer = particles.GetComponent<ParticleSystemRenderer>();
    }

    public void OnBiomeChange(Biome biome)
    {
        switch (biome)
        {
            case (Biome.PLAINS):
                particleRenderer.material=plainsClouds;
                break;
            case (Biome.ICE):
                particleRenderer.material = iceClouds;
                break;
            case (Biome.DESERT):
                particleRenderer.material = desertClouds;
                break;
        }
    }
}
