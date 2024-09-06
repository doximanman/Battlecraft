using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class InventoryData
{
    [SerializeField] private List<StackData> items;

    // default number of items
    public InventoryData(int capacity)
    {
        items = new();
        for(int i = 0; i < capacity; i++)
        {
            items.Add(null);
        }
    }

    public InventoryData(Inventory inventory) : this(inventory.slots.Count)
    {
        LoadFrom(inventory);
    }

    public IEnumerable<StackData> GetItems()
    {
        return items;
    }

    public void Fix()
    {
        // unity inserts empty stacks to the list instead of
        // null. replace them.
        // (note: probably not relevant anymore)
        for(int i = 0;i < items.Count;i++)
        {
            if (items[i] != null && (items[i].type == null || items[i].count == 0))
                items[i] = null;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i] = null;
        }
    }

    #region Serialization
    // cannot save an itemstack 

    public static JObject Serialize(InventoryData data)
    {
        // convert every stack in the inventorydata to a json
        // and create an array of those jsons
        JArray stackDatas = JArray.FromObject(
            data.items.Select(stack => StackData.Serialize(stack)));
        return new()
        {
            ["stacks"] = stackDatas
        };
    }

    public static InventoryData Deserialize(JObject serialized)
    {
        // get the array of jsons from the serialized object
        // and convert each json to its stackdata
        JArray stackDatas = serialized["stacks"] as JArray;
        List<StackData> itemsList = new(stackDatas.Select(stackData => StackData.Deserialize(stackData as JObject)));
        InventoryData invData = new(itemsList.Count)
        {
            items = itemsList
        };
        return invData;
    }

    public static InventoryData Deserialize(string serialized)
    {
        return Deserialize(JObject.Parse(serialized));
    }
    #endregion

    public void LoadFrom(Inventory inventory)
    {
        Clear();
        int numOfSlots = Math.Min(inventory.slots.Count, items.Count);
        for (int i = 0; i < numOfSlots; i++)
        {
            var stack = inventory.slots[i].GetStack();
            if (stack == null) items[i] = null;

            // save all the items
            else items[i] = new StackData(stack);
        }
    }

    public void LoadTo(Inventory inventory)
    {
        // nulls all the items in the slots
        inventory.Clear();
        int numOfSlots = Math.Min(inventory.slots.Count, items.Count);
        for (int i = 0; i < numOfSlots; i++)
        {
            // deep copy all the items
            if (items[i] != null && items[i].IsDefined())
                inventory.slots[i].SetItem(items[i]);
        }
    }

    public InventoryData Copy()
    {
        InventoryData copy = new(items.Count);
        for(int i=0;i<items.Count; i++)
        {
            copy.items[i] = items[i];
        }
        return copy;
    }

    public override string ToString()
    {
        return string.Join(",", items);
    }

    public override bool Equals(object obj)
    {
        if (obj is not InventoryData other) return false;
        if (items == null) return other.items == null;
        if (other.items.Count != items.Count) return false;

        return items.SequenceEqual(other.items);
    }

    public override int GetHashCode()
    {
        if (items == null) return 0;
        return items.GetHashCode();
    }
}
