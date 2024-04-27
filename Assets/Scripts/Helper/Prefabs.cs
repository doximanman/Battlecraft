using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Prefabs : MonoBehaviour
{
    public static GameObject stackPrefab;

    [SerializeField] private GameObject _stackPrefab;
    // Start is called before the first frame update
    private void Start()
    {
        stackPrefab = _stackPrefab;
    }
}

