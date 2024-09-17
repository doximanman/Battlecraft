using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    private GameObject GetPrefab(InteractableType type)
    {
        return typePrefabs.Find(x => x.type == type).prefab;
    }

    /// <summary>
    /// destroy all interactables in the game
    /// </summary>
    public void Clear()
    {
        // destroy the children!!
        while(transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        transform.DetachChildren();
        interactables = new();
    }

    public InteractableData[] GetData()
    {
        return interactables.Select(interactable => new InteractableData(interactable)).ToArray();
    }

    public void LoadData(InteractableData[] data)
    {
        Clear();

        foreach (InteractableData interactableData in data)
        {
            GameObject typePrefab = GetPrefab(interactableData.type);
            Interactable interactable = Instantiate(typePrefab, transform).GetComponent<Interactable>();
            interactable.name = typePrefab.name;
            interactableData.LoadInto(interactable);
        }
    }
}
