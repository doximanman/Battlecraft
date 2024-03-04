using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject playerObject;
    public float smoothTime;
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

        transform.position = Vector3.SmoothDamp(transform.position, transform.position + xOffset * Vector3.right,ref currentVelocity,smoothTime);

        lastPosition=playerObject.transform.position;
    }
}
