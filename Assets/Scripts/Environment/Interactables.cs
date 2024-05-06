using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Interactables : MonoBehaviour
{
    public static Interactables current;

    public List<Interactable> interactables;

    // Start is called before the first frame update
    void Start()
    {
        current = this;
    }

}
