using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSync : MonoBehaviour
{

    /// <summary>
    /// savable objects. all must have unique names.
    /// </summary>
    [SerializeField] private List<SavableObject> savableObjects = new();

    private void OnEnable()
    {
        //DataManager.instance.onSave += Save;
        //DataManager.instance.onLoad += Load;
    }

    private void OnDisable()
    {
        //DataManager.instance.onSave -= Save;
        //DataManager.instance.onLoad -= Load;
    }

    [ContextMenu("Save As Default")]
    public void SaveDefault()
    {
        WorldData worldData = new(savableObjects);
        WorldSaver.SaveWorldDataObject(worldData, true);
    }

    [ContextMenu("Load From Default")]
    public void LoadDefault()
    {
        WorldData data = WorldSaver.LoadWorldDataObject(true);
        data.LoadInto(savableObjects);
    }

    // passive server - save to server,
    // but don't import from server while playing.

    [ContextMenu("Save To File")]
    public async void Save()
    {
        WorldData data = new(savableObjects);
        WorldSaver.SaveWorldDataObject(data);
        // try to save to server
        await WorldAPI.SaveWorldDataObject(data);
    }

    [ContextMenu("Load From File")]
    public void Load()
    {
        WorldData data = WorldSaver.LoadWorldDataObject();
        data.LoadInto(savableObjects);
    }
}
