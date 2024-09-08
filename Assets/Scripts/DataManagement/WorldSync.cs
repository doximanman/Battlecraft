using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorldSync : MonoBehaviour
{

    /// <summary>
    /// savable objects. all must have unique names.
    /// </summary>
    [SerializeField] private List<SavableObject> savableObjects = new();

    // comment "onenable" and "ondisable" to make changes to default world,
    // and then do "save as default" from the inspector
    private void OnEnable()
    {
        DataManager.instance.onSave += Save;
        DataManager.instance.onLoad += Load;
    }

    private void OnDisable()
    {
        DataManager.instance.onSave -= Save;
        DataManager.instance.onLoad -= Load;
    }

    [ContextMenu("Reset World")]
    public void ResetWorld()
    {
        if (!MetaLogic.paused)
            MetaLogic.Pause();
        KeyInput.instance.DisableKeys();

        static void Exit()
        {
            KeyInput.instance.EnableKeys();
            if (MetaLogic.pauseMenuEnabled)
                MetaLogic.ClosePauseMenu();
            else if (MetaLogic.paused)
                MetaLogic.Unpause();
        }
        Modal.Ask("Reset World",
            "Are you sure you want to reset the world?<br>Your current save will be lost!",
            "Reset",
            "Cancel",
            async () => {
                await LoadDefault();
                Exit();
            },
            () => {
                Exit();
            });
    }

    [ContextMenu("Save As Default")]
    public async Task SaveDefault()
    {
        WorldData worldData = new(savableObjects);
        await WorldSaver.SaveWorldDataObjectAsync(worldData, true);
    }

    [ContextMenu("Load From Default")]
    public async Task LoadDefault()
    {
        WorldData data = await WorldSaver.LoadWorldDataObjectAsync(true);
        data.LoadInto(savableObjects);
    }

    // passive server - save to server,
    // but don't import from server while playing.

    [ContextMenu("Save To File")]
    public async void Save()
    {
        WorldData data = new(savableObjects);
        await WorldSaver.SaveWorldDataObjectAsync(data);
        // try to save to server
        await WorldAPI.SaveWorldDataObject(data);
    }

    [ContextMenu("Load From File")]
    public async void Load()
    {
        WorldData data = await WorldSaver.LoadWorldDataObjectAsync();
        data.LoadInto(savableObjects);
    }
}
