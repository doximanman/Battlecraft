using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SunMovement : MonoBehaviour
{
    public float sunVelocity;

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + sunVelocity * Time.deltaTime * Vector3.right;

    }
}
