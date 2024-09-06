using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;

public class InteractableSaver : SavableObject
{
    private Interactables interactables;

    private void Awake()
    {
        interactables = GetComponent<Interactables>();
    }

    public override string SavableName => "Interactables";

    public override JObject Save()
    {
        // convert each interactable into an interactabledata
        // and then convert each interactabledata into a json
        IEnumerable<JObject> arr = interactables.interactables.Select(
            interactable => InteractableData.Save(new InteractableData(interactable)));
        // and then store the array
        JArray serializedArray = JArray.FromObject(arr);
        return new()
        {
            ["interactables"] = serializedArray
        };
    }

    public override void Load(JObject serializedObject)
    {
        // get jobject array from serialized object
        IEnumerable<JObject> arr = serializedObject["interactables"].ToObject<IEnumerable<JObject>>();
        // convert all the jobjects in the array into interactabledata
        InteractableData[] data = arr.Select(serialized => InteractableData.Load(serialized)).ToArray();
        // load the data
        interactables.LoadData(data);
    }
}
