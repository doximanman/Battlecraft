using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    private float maxX;
    private float maxY;

    public List<string> canJumpFrom=new List<string>();

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

    // preferably use canJumpOn(collider)
    public bool canJumpOn(string tag)
    {
        return canJumpFrom.Contains(tag);
    }

    public bool canJumpOn(Collider2D collider)
    {
        return canJumpFrom.Contains(collider.tag) || canJumpOn(collider.transform.parent);
    }

    private bool canJumpOn(Transform transform)
    {
        if (transform == null) return false;

        return canJumpFrom.Contains(transform.tag) || canJumpOn(transform.parent);
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
