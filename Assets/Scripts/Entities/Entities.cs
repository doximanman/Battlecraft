using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entities : MonoBehaviour
{
    public static Entities instance;

    private void Awake()
    {
        instance = this;
    }
    
}
