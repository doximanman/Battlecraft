using System;
using System.Collections;
using System.Collections.Generic;
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
        items = new List<StackData>(capacity);
        Clear();
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
}
