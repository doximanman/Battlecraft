using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Video;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new();

    public void SetSlots(IEnumerable<InventorySlot> slots)
    {
        this.slots = new(slots);
    }

    // returns the slot item was added in3
    // adds 1 of the item
    public InventorySlot AddItem(ItemType item)
    {
        // if the item is in the inventory, add it to an existing stack
        foreach (var slot in slots)
        {
            ItemStack itemStack = slot.GetStack();
            if (itemStack != null && itemStack.Type.Equals(item))
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
    public StackData AddStack(ItemStack stack)
    {
        ItemType item = stack.Type;
        // if the item is in the inventory, add it into existing stacks
        int remainder = stack.ItemCount;
        foreach (var slot in slots)
        {
            ItemStack itemStack = slot.GetStack();
            if (itemStack != null && itemStack.Type.Equals(item))
            {
                if (itemStack.ItemCount < item.maxStack)
                {
                    int added = Mathf.Min(item.maxStack-itemStack.ItemCount,remainder);
                    itemStack.ItemCount+=added;
                    remainder -= added;
                    if (remainder == 0)
                    {
                        Destroy(stack.gameObject);
                        return null;
                    };
                }
            }
        }

        // add the remainder to any empty slots
        foreach(var slot in slots)
        {
            if(slot.GetStack()==null)
            {
                slot.SetItem(stack);
                return null;
            }
        }
        return new StackData(stack.Type,remainder) ;
    }

    public bool HasSpace(ItemType item)
    {
        foreach(var slot in slots)
        {
            // empty slot or existing slot with non full stack
            if (slot.GetStack() == null) return true;
            if (slot.GetStack().Type.Equals(item)) return slot.GetStack().ItemCount < item.maxStack;
        }
        return false;
    }

    public static void MoveItem(InventorySlot slot, Inventory to)
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

    public static void MoveInventory(IEnumerable<InventorySlot> from,Inventory to)
    {
        foreach(var slot in from)
        {
            MoveItem(slot, to);
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

        // unity stuff (see "Fix")
        if (itemToAdd.name == "Chest")
            itemToAdd.invData.Fix();

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
            else if (slot.GetItemType()==itemToAdd)
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

    public override string ToString()
    {
        StringBuilder str=  new StringBuilder();
        for(int i = 0; i < slots.Count; i++)
        {
            str.Append(i+", " + (slots[i].GetStack() == null ? "empty" : slots[i].GetStack().Type.name + "Count: " + slots[i].GetStack().ItemCount) + ";;");
        }
        return str.ToString();
    }
}
