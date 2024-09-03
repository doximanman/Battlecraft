using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Linq;

public class InteractableSaver : SavableObject
{
    [Serializable]
    private class InteractableDataList
    {
        public InteractableData[] list;
    }

    private Interactables interactables;

    private void Awake()
    {
        interactables = GetComponent<Interactables>();
    }

    public override string SavableObjectName => "Interactables";

    public override string SerializeObject()
    {
        InteractableData[] dataArray = interactables.GetData();
        InteractableDataList data = new() { list = dataArray };
        return JsonUtility.ToJson(data);
    }

    public override void DeserializeObject(string serializedObject)
    {
        InteractableDataList preData = JsonUtility.FromJson<InteractableDataList>(serializedObject);
        InteractableData[] data = preData.list;
        interactables.LoadData(data);
    }
}
