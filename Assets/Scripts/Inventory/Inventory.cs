using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Inventory : MonoBehaviour, IEnumerable<InventorySlot>
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
        if(item == null) return null;

        // if the item is in the inventory, add it to an existing stack
        foreach (var slot in slots)
        {
            ItemStack itemStack = slot.GetStack();
            // stack isn't null and is equal to this type
            if (itemStack != null && itemStack.Type == item)
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
            // slot is empty and it can accept this item
            if (slot.GetStack() == null && slot.CanAccept(item))
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
        /*for(int i = 0; i < count; i++)
        {
            var success=AddItem(type);
            if (success==null) return count-i;
        }
        return 0;*/

        // add to existing stacks
        foreach(var slot in slots)
        {
            ItemStack stack = slot.GetStack();
            // how much of the stack is left
            if (stack != null && stack.Type == type)
            {
                int combinedCounts = stack.ItemCount + count;
                int remainder = 0;
                if(combinedCounts <= type.maxStack)
                {
                    // combined counts can fit in one stack
                    // add everything
                    stack.ItemCount = combinedCounts;
                    return 0;
                }
                else
                {
                    // more items in combined count than maximum count
                    remainder = combinedCounts - type.maxStack;
                    stack.ItemCount = type.maxStack;
                }
                // set count to be the amount left in the stack
                count = remainder;
                if (count == 0)
                    return 0;
            }
        }

        // add to free slots
        foreach(var slot in slots)
        {
            if(slot.GetStack() == null && slot.CanAccept(type))
            {
                // if count is more than maximum stack count of the type
                if (count > type.maxStack)
                {
                    // add the stack to this slot and subtract from count
                    StackData newStack = new(type, type.maxStack);
                    slot.SetItem(newStack);
                    count -= type.maxStack;
                }
                else
                {
                    // add the stack to this slot and return
                    StackData newStack = new(type, count);
                    slot.SetItem(newStack);
                    return 0;
                }
            }
        }

        return count;
    }

    /// <summary>
    /// Adds all the items to the inventory.
    /// </summary>
    /// <param name="stacks">Collection of items</param>
    /// <returns>Collection of remainder</returns>
    public IEnumerable<StackData> AddItems(IEnumerable<StackData> stacks)
    {
        var stackList=new List<StackData>(stacks);
        int i=0;
        for(; i<stackList.Count;i++)
        {
            int remainder = AddItems(stackList[i].type, stackList[i].count);
            if (remainder > 0) yield return new(stackList[i].type, remainder);
        }
        for (; i < stackList.Count; i++)
        {
            yield return stackList[i];
        }
        yield return null;
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
            if (itemStack != null && slot.CanAccept(stack.Type))
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
            if(slot.GetStack()==null && slot.CanAccept(stack.Type))
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
            if (slot.CanAccept(item))
            {
                // empty slot or existing slot with non full stack
                if (slot.GetStack() == null) return true;
                if (slot.GetStack().Type == item) return slot.GetStack().ItemCount < item.maxStack;
            }
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

    IEnumerator<InventorySlot> IEnumerable<InventorySlot>.GetEnumerator()
    {
        return slots.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return slots.GetEnumerator();
    }

    #region Inspector
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
    #endregion
    public override string ToString()
    {
        StringBuilder str=  new();
        for(int i = 0; i < slots.Count; i++)
        {
            str.Append(i+", " + (slots[i].GetStack() == null ? "empty" : slots[i].GetStack().Type.name + "Count: " + slots[i].GetStack().ItemCount) + ";;");
        }
        return str.ToString();
    }

}
