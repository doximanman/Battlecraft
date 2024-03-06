using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudPosition : MonoBehaviour
{
    public float yOffset;
    public float cloudHeight;
    public float zOffset;

    private Logic logic;
    // Start is called before the first frame update
    void Start()
    {
        logic=GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();

        float yPos=Camera.main.ViewportToWorldPoint(new Vector3(0, yOffset, 0)).y;
        transform.position = new Vector3(logic.GetMaxX() + 10,yPos,zOffset);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
