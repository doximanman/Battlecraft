using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public Action onSave;
    public Action onLoad;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        KeyInput.instance.onSave += (down, held, up) =>
        {
            if (down)
                Save();
        };
    }

    // load once at the start
    private bool oneTime = false;
    private void Update()
    {
        if (oneTime) return;

        Load();
        oneTime = true;
    }

    public void Save()
    {
        onSave?.Invoke();
    }

    public void Load()
    {
        onLoad?.Invoke();
    }
}
