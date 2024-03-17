using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using static UnityEditor.Experimental.GraphView.Port;

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

    private void Clear()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i] = null;
        }
    }

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

    public override bool Equals(object obj)
    {
        var other= obj as InventoryData;
        if (other == null) return false;
        if (other.items.Count != items.Count) return false;

        return items.SequenceEqual(other.items);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
