using Codice.Client.BaseCommands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class Interactables : MonoBehaviour
{
    public static Interactables current;

    [Serializable]
    public class InteractableTypePrefab
    {
        public InteractableType type;
        public GameObject prefab;
    }

    [SerializeField] private List<InteractableTypePrefab> typePrefabs;


    public List<Interactable> interactables;

    void Awake()
    {
        current = this;
    }

    private void Start()
    {
        DataManager.instance.onSave += Save;

        DataManager.instance.onLoad += Load;
    }

    private GameObject GetPrefab(InteractableType type)
    {
        return typePrefabs.Find(x => x.type == type).prefab;
    }

    /// <summary>
    /// destroy all interactables in the game
    /// </summary>
    public void Clear()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        transform.DetachChildren();
        interactables = new();
    }

    [ContextMenu("Load From File")]
    public void Load()
    {
        Clear();

        InteractableData[] data = WorldSaver.LoadWorld();

        foreach(InteractableData interactableData in data)
        {
            GameObject typePrefab = GetPrefab(interactableData.type);
            Interactable interactable = Instantiate(typePrefab,transform).GetComponent<Interactable>();

            Vector3 position = new(interactableData.position[0], interactableData.position[1], interactableData.position[2]);
            Vector3 scale = new(interactableData.scale[0], interactableData.scale[1], interactableData.scale[2]);

            interactable.transform.position = position;
            interactable.transform.localScale = scale;

            interactables.Add(interactable);
        }
    }

    [ContextMenu("Save To File")]
    public void Save()
    {
        // create InteractableData for every Interactable
        InteractableData[] data = interactables.Select(interactable => new InteractableData(interactable)).ToArray();

        WorldSaver.SaveWorld(data);
    }

}
