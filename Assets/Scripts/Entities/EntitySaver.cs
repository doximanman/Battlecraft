

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EntitySaver : SavableObject
{
    private Entities entities;

    private void Awake()
    {
        entities = GetComponent<Entities>();
    }

    public override string SavableName => "Entities";

    public override JObject Save()
    {
        // serialize every entity.
        // entity -> entitydata -> JObject
        IEnumerable<JObject> serializedEntities = entities.entities.Select(
            entity => EntityData.Save(new EntityData(entity)));
        JArray entityArray = JArray.FromObject(serializedEntities);
        return new()
        {
            ["entities"] = entityArray
        };
    }

    public override void Load(JObject serializedObject)
    {
        // get jobject array
        IEnumerable<JObject> serializedEntities = serializedObject["entities"].ToObject<IEnumerable<JObject>>();
        // convert each jobject to entitydata
        EntityData[] entityDatas = serializedEntities.Select(serialized => EntityData.Load(serialized)).ToArray();
        // load
        entities.LoadData(entityDatas);
    }
}
