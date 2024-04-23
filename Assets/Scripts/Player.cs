using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static GameObject current;

    private void Start()
    {
        current = gameObject;
    }
}
