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
        public string data;

        public Entry(string name, string data)
        {
            this.name = name;
            this.data = data;
        }
    }

    public List<Entry> savableObjects;

    private string GetDataOf(string key)
    {
        return savableObjects.Find(x => x.name == key).data;
    }

    public WorldData(IEnumerable<SavableObject> savableObjects)
    {
        this.savableObjects = new();
        foreach(var obj in savableObjects)
            this.savableObjects.Add(new Entry(obj.SavableName, obj.Save()));
    }

    public void LoadInto(IEnumerable<SavableObject> savableObjects)
    {
        foreach(var obj in savableObjects)
        {
            obj.Load(GetDataOf(obj.SavableName));
        }
    }
}
