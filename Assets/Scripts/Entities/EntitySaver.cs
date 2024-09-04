

using System;
using System.IO;
using UnityEngine;

public class EntitySaver : SavableObject
{
    private Entities entities;

    private void Awake()
    {
        entities = GetComponent<Entities>();
    }

    [Serializable]
    public class EntityDataList
    {
        public EntityData[] list;
    }

    public override string SavableName => "Entities";

    public override string Save()
    {
        EntityData[] preData = entities.GetData();
        EntityDataList data = new() { list = preData };
        return JsonUtility.ToJson(data);
    }

    public override void Load(string serializedObject)
    {
        EntityDataList preData = JsonUtility.FromJson<EntityDataList>(serializedObject);
        EntityData[] data = preData.list;
        entities.LoadData(data);
    }
}
