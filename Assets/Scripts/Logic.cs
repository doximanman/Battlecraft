using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    private float maxX;
    private float maxY;

    // Start is called before the first frame update
    void Start()
    {
        maxX = 135;
        maxY = 3.5f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float GetMaxX()
    {
        return maxX;
    }

    public float GetMaxY()
    {
        return maxY;
    }
}
