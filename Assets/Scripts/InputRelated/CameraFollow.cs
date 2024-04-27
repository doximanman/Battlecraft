using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject playerObject;
    public float xSmoothTime;
    public float ySmoothTime;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition=playerObject.transform.position;
    }

    void Update()
    {
        float xOffset=playerObject.transform.position.x - lastPosition.x;

        transform.position = Vector3.SmoothDamp(transform.position, transform.position + xOffset * Vector3.right,ref currentVelocity,xSmoothTime);

        float yOffset=playerObject.transform.position.y - lastPosition.y;

        transform.position=Vector3.SmoothDamp(transform.position,transform.position+yOffset*Vector3.up,ref currentVelocity,ySmoothTime);

        lastPosition=playerObject.transform.position;
    }
}
