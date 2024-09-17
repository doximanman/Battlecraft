using Newtonsoft.Json.Linq;
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
    public float[] position;
    public float[] scale;
    public float[] rotation;
    public JObject internalData;

    private InteractableData() { }

    public InteractableData(Interactable interactable)
    {
        type = interactable.type;

        Vector3 position = interactable.transform.position;
        this.position = new float[3];
        this.position[0] = position.x;
        this.position[1] = position.y;
        this.position[2] = position.z;

        Vector3 scale = interactable.transform.localScale;
        this.scale = new float[3];
        this.scale[0] = scale.x;
        this.scale[1] = scale.y;
        this.scale[2] = scale.z;

        Quaternion rotation = interactable.transform.rotation;
        this.rotation = new float[4];
        this.rotation[0] = rotation.x;
        this.rotation[1] = rotation.y;
        this.rotation[2] = rotation.z;
        this.rotation[3] = rotation.w;

        internalData = interactable.SaveInternal();
    }

    public void LoadInto(Interactable interactable)
    {
        interactable.type = type;

        Vector3 newPosition = new(position[0], position[1], position[2]);
        Quaternion newRotation = new(rotation[0], rotation[1], rotation[2], rotation[3]);
        interactable.transform.SetPositionAndRotation(newPosition, newRotation);

        Vector3 newScale = new(scale[0], scale[1], scale[2]);
        interactable.transform.localScale = newScale;

        interactable.LoadInternal(internalData);
    }

    public static JObject Save(InteractableData interactableData)
    {
        return new()
        {
            ["type"] = interactableData.type.ToString(),
            ["position"] = JArray.FromObject(interactableData.position),
            ["scale"] = JArray.FromObject(interactableData.scale),
            ["rotation"] = JArray.FromObject(interactableData.rotation),
            ["internal"] = interactableData.internalData,
        };
    }

    public static InteractableData Load(JObject serialized)
    {
        string typeName = serialized["type"].ToString();
        InteractableType interactableType = (InteractableType) Enum.Parse(typeof(InteractableType), typeName);
        float[] positionArray = serialized["position"].ToObject<float[]>();
        float[] scaleArray = serialized["scale"].ToObject<float[]>();
        float[] rotationArray = serialized["rotation"].ToObject<float[]>();
        JObject interactableInternalData = serialized["internal"] as JObject;
        return new()
        {
            type = interactableType,
            position = positionArray,
            scale = scaleArray,
            rotation = rotationArray,
            internalData = interactableInternalData,
        };
    }
}
