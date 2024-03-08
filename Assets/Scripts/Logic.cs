using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    private float maxX;
    private float maxY;

    private List<string> canJumpFrom=new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        maxX = 135;
        maxY = 3.5f;

        canJumpFrom.Add("Ground");
        canJumpFrom.Add("Obsticles");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool canJumpOn(string tag)
    {
        return canJumpFrom.Contains(tag);
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
