using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class LootTable
{
    [Serializable]
    public class LootTableEntry
    {
        public ItemType type;
        public int minCount;
        public int maxCount;
    }

    public List<LootTableEntry> loot;

    public static JObject Serialize(LootTable lootTable)
    {
        // convert every entry into a json
        // and save an array of those jsons
        JArray entryArray = JArray.FromObject(
            lootTable.loot.Select(entry => new JObject()
            {
                ["type"] = entry.type == null ? "None" : entry.type.name,
                ["minCount"] = entry.minCount,
                ["maxCount"] = entry.maxCount
            }));
        return new JObject()
        {
            ["lootTable"] = entryArray
        };
    }

    public static LootTable Deserialize(JObject serialized)
    {
        JArray entryArray = serialized["lootTable"] as JArray;
        List < LootTableEntry > entryList= new();
        foreach (JObject entryData in entryArray.Cast<JObject>())
        {
            string typeName = entryData["type"].ToString();
            int entryMinCount = entryData["minCount"].Value<int>();
            int entryMaxCount = entryData["maxCount"].Value<int>();
            LootTableEntry entry = new()
            {
                type = ItemTypes.GetByName(typeName),
                minCount = entryMinCount,
                maxCount = entryMaxCount
            };
            entryList.Add(entry);
        }
        return new LootTable()
        {
            loot = entryList
        };
    }

    public static LootTable Deserialize(string serialized)
    {
        return Deserialize(JObject.Parse(serialized));
    }
}
