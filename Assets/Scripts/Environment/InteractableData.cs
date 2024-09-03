using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum InteractableType { 
    ICE_TREE1,
    ICE_TREE2,
    ICE_TREE3,
    LAVENDAR,
    ORANGE_BUSH,
    PLAINS_TREE,
    PLAINS_TREE2,
    CHEST,
    FURNACE,
    CRAFTING_BENCH
}

[Serializable]
public class InteractableData
{
    public InteractableType type;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
    public string internalData;

    public InteractableData(Interactable interactable)
    {
        type = interactable.type;
        position = interactable.transform.position;
        scale = interactable.transform.localScale;
        rotation = interactable.transform.rotation;
        internalData = interactable.SaveInternal();
    }

    public void LoadInto(Interactable interactable)
    {
        interactable.type = type;
        interactable.transform.SetPositionAndRotation(position, rotation);
        interactable.transform.localScale = scale;
        interactable.LoadInternal(internalData);
    }
}
