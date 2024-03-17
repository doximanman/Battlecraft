using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Video;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new();

    // returns the slot item was added in
    // adds 1 of the item
    public InventorySlot AddItem(ItemType item)
    {
        // if the item is in the inventory, add it to an existing stack
        foreach (var slot in slots)
        {
            ItemStack itemStack = slot.GetStack();
            if (itemStack != null && itemStack.Type.name == item.name)
            {
                if (itemStack.ItemCount < item.maxStack)
                {
                    itemStack.ItemCount++;
                    return slot;
                }
            }
        }

        // item doesn't already exist (or all the stacks are full)
        // place it in an unused slot
        foreach (var slot in slots)
        {
            if (slot.GetStack() == null)
            {
                slot.SetItem(item);
                return slot;
            }
        }

        // no slots available
        return null;
    }

    // returns the count of items left
    // 0 on successs, > 0 if there was a remainder
    public int AddItems(ItemType type,int count)
    {
        for(int i = 0; i < count; i++)
        {
            var success=AddItem(type);
            if (success==null) return count-i;
        }
        return 0;
    }

    // adds wherever available.
    // returns the remainder
    public ItemStack AddStack(ItemStack stack)
    {
        ItemType item = stack.Type;
        // if the item is in the inventory, add it into existing stacks
        int remainder = stack.ItemCount;
        foreach (var slot in slots)
        {
            ItemStack itemStack = slot.GetStack();
            if (itemStack != null && itemStack.Type.name == item.name)
            {
                if (itemStack.ItemCount < item.maxStack)
                {
                    int added = Mathf.Min(item.maxStack-itemStack.ItemCount,remainder);
                    itemStack.ItemCount+=added;
                    remainder -= added;
                    if (remainder == 0) return null;
                }
            }
        }

        // add the remainder to any empty slots
        foreach(var slot in slots)
        {
            if(slot.GetStack() == null)
            {
                slot.SetItem(item);
                slot.GetStack().ItemCount = stack.ItemCount;
                return null;
            }
        }
        stack.ItemCount = remainder;
        return stack;
    }

    public bool HasSpace(ItemType item)
    {
        foreach(var slot in slots)
        {
            // empty slot or existing slot with non full stack
            if (slot.GetStack() == null) return true;
            if (slot.GetStack().Type == item) return slot.GetStack().ItemCount < item.maxStack;
        }
        return false;
    }

    public void MoveItem(InventorySlot slot, Inventory to)
    {
        ItemStack item = slot.GetStack();
        if (item != null)
        {
            slot.RemoveItem();

            ItemType type = item.Type;
            int itemCount = item.ItemCount;
            int remainder = to.AddItems(type, itemCount);
            if (remainder > 0)
            {
                slot.SetItem(type);
                slot.GetStack().ItemCount = remainder;
            }
        }
    }

    public void Clear()
    {
        for(int i = 0; i < slots.Count; i++) {
            slots[i].RemoveItem();
        }
    }

    public InventorySlot slot;
    public int addCount;
    public ItemType itemToAdd;
    [ContextMenu("Add Item")]
    public void AddCurrentItem()
    {
        if (addCount == 0) return;
        if (slot == null)
        {
            // add anywhere
            AddItems(itemToAdd, addCount);
        }
        else
        {
            if (!slot.HasItem())
            {
                slot.SetItem(itemToAdd);
                ItemStack stack = slot.GetStack();
                stack.ItemCount = (stack.ItemCount-1 + addCount) % (stack.Type.maxStack+1);
            }
            else if (slot.GetItemType().name == itemToAdd.name)
            {
                // combine stacks
                ItemStack stack = slot.GetStack();
                stack.ItemCount = (stack.ItemCount + addCount) % (stack.Type.maxStack + 1);
            }
            else
            {
                // slot already has an item
                return;
            }
        }
    }
}
