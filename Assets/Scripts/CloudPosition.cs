using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudPosition : MonoBehaviour
{
    public float yOffset;
    public float cloudHeight;
    public float zOffset;

    private Logic logic;
    private ParticleSystem.MainModule particleControl;
    // Start is called before the first frame update
    void Start()
    {
        // where to place
        // cloud particle start point and how far the clouds
        // need to travel
        float yPos=Camera.main.ViewportToWorldPoint(new Vector3(0, yOffset, 0)).y;
        transform.position = new Vector3(Logic.maxX + 10,yPos,zOffset);

        // set startLifetime such that the clouds appear
        // all over the map.
        ParticleSystem particles=GetComponent<ParticleSystem>();
        particleControl = particles.main;

        float xBounds = Logic.maxX - Logic.minX;
        particleControl.startLifetime = 10 * xBounds;

        // 'prewarm' so clouds already appear everywhere.
        particles.Simulate(particleControl.duration);
        particles.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
