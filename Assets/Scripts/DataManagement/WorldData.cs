using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WorldData
{
    [Serializable]
    public class Entry
    {
        public string name;
        public JObject data;
    }

    public List<Entry> savableObjects;

    private WorldData() { }

    public WorldData(IEnumerable<SavableObject> savableObjects)
    {
        this.savableObjects = new();
        foreach (var obj in savableObjects)
            this.savableObjects.Add(new Entry()
            {
                name = obj.SavableName,
                data = obj.Save()
            });
    }

    private JObject GetDataOf(string key)
    {
        return savableObjects.Find(x => x.name == key).data;
    }


    public void LoadInto(IEnumerable<SavableObject> savableObjects)
    {
        foreach(var obj in savableObjects)
        {
            obj.Load(GetDataOf(obj.SavableName));
        }
    }

    public static JObject Save(WorldData data)
    {
        IEnumerable<JObject> serializedEntires = data.savableObjects.Select(
            entry => new JObject()
            {
                ["name"] = entry.name,
                ["data"] = entry.data
            });
        JArray serializedArray = JArray.FromObject(serializedEntires);
        return new()
        {
            ["entries"] = serializedArray
        };
    }

    public static WorldData Load(JObject serialized)
    {
        IEnumerable<JObject> serializedEntires = serialized["entries"].ToObject<IEnumerable<JObject>>();
        IEnumerable<Entry> savableObjectList = serializedEntires.Select(serialized =>
        {
            return new Entry()
            {
                name = serialized["name"].ToString(),
                data = serialized["data"] as JObject,
            };
        });
        return new()
        {
            savableObjects = savableObjectList.ToList(),
        };
    }
}
