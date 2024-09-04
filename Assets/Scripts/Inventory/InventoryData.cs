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

    private void Clear()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i] = null;
        }
    }

    #region Serialization
    // cannot save an itemstack 

    [Serializable]
    private class StringList {
        public string[] list;
    }

    public static string Serialize(InventoryData data)
    {
        // serialize every item in the items array
        string[] serializedArray = data.items.Select(stack => StackData.Serialize(stack)).ToArray();
        // save it in a serializable object (a list/array by itself is not serializable,
        // it requires a wrapper class).
        StringList list = new() { list = serializedArray };
        // serialize using jsonutility.
        return JsonUtility.ToJson(list);
    }

    public static InventoryData Deserialize(string serialized)
    {
        // deserialize into array of serialized stacks
        string[] serializedArray = JsonUtility.FromJson<StringList>(serialized).list;
        // create the inventory data with its items being the deserialized stacks
        InventoryData data = new(serializedArray.Length)
        {
            // deserialize every stack
            items = new(serializedArray.Select(stack => StackData.Deserialize(stack)).ToArray())
        };
        return data;
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
