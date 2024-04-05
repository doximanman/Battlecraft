using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private GameObject stackPrefab;

    [SerializeField] private bool canAcceptItems;

    // initialized on SetItem
    // removed on RemoveItem
    private ItemStack stack;

    public delegate void InventorySlotChange(StackData newData);
    public InventorySlotChange slotChangeListeners;


    public ItemStack GetStack()
    {
        return stack;
    }

    private void NotifyChange()
    {
        if (slotChangeListeners != null)
            if (stack == null)
                slotChangeListeners(null);
            else slotChangeListeners(new(stack));
    }

    // combines the stacks, doesn't move them.
    // first item is this slot, second item is remainder.
    // remainder is null if no remainder.
    public StackData[] CombineStacks(StackData stack)
    {
        var thisStackMaybeNull = GetStack();
        if (thisStackMaybeNull == null)
        {
            return new[] { stack, null };
        }

        var thisStack = new StackData(thisStackMaybeNull);
        // there is a stack - find how many
        // items can move into this stack.
        int combinedCounts = thisStack.count + stack.count;
        if (combinedCounts > stack.type.maxStack)
        {
            // too many items for one stack
            // update stack count and return remainder
            // remainder is the updated passed parameter
            int remainder = combinedCounts - stack.type.maxStack;
            thisStack.count = stack.type.maxStack;
            stack.count = remainder;
            return new[] { thisStack, stack };
        }
        // items can combine without remainder
        thisStack.count += stack.count;
        return new[] { thisStack, null };
    }

    // no remainder
    // original stack is destroyed
    public void CombineFrom(ItemStack stack)
    {
        int newCount = Mathf.Min(stack.Type.maxStack, stack.ItemCount + this.stack.ItemCount);
        stack.ItemCount = newCount;
        Destroy(stack.gameObject);
    }

    // create new stack
    public void SetItem(StackData stack)
    {
        if (stack == null || !stack.IsDefined())
        {
            RemoveItem();
            return;
        }

        if (this.stack == null)
        {
            GameObject newStack = Instantiate(stackPrefab, transform);
            this.stack = newStack.GetComponent<ItemStack>();
        }
        this.stack.Type = stack.type;
        this.stack.ItemCount = stack.count;
        NotifyChange();
    }

    // predefined stack
    public void SetItem(ItemStack stack)
    {
        if (stack == null)
        {
            RemoveItem();
            return;
        }

        stack.transform.SetParent(transform);
        this.stack = stack;
        NotifyChange();
    }

    // only 1 item of that type
    public void SetItem(ItemType item)
    {
        if (item == null)
        {
            RemoveItem();
            return;
        }

        if (this.stack == null)
        {
            GameObject newStack = Instantiate(stackPrefab, transform);
            newStack.GetComponent<ItemStack>().Type = item;
            this.stack = newStack.GetComponent<ItemStack>();
        }
        else
        {
            this.stack.Type = item;
            this.stack.ItemCount = 1;
        }
        NotifyChange();
    }

    // only 1 item of that type
    public void RemoveOne()
    {
        if (GetStack() != null)
        {
            if (GetStack().ItemCount == 1) RemoveItem();
            else GetStack().ItemCount--;
            NotifyChange();
        }
    }

    // remove count of that type
    public void RemoveSome(int count)
    {
        for (int i = 0; i < count; i++)
        {
            RemoveOne();
        }
    }

    // destroy item object
    public void RemoveItem()
    {
        if (GetStack() != null) Destroy(GetStack().gameObject);
        stack = null;
        NotifyChange();
    }

    // dont destroy item object
    public void DetatchChild()
    {
        transform.DetachChildren();
        stack = null;
        NotifyChange();
    }



    public bool HasItem()
    {
        return GetStack() != null;
    }

    public ItemType GetItemType()
    {
        if (GetStack() == null) return null;
        return GetStack().Type;
    }

    // disables dragging operation
    public void DoubleClick()
    {
        ItemStack.CancelDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (ItemStack.stopDrag || eventData.pointerDrag == null || !canAcceptItems) return;

        var item = ItemStack.beingDragged;
        var originalSlot = ItemStack.originalSlot;
        // left click
        // if no item here - set it to be the stack.
        if(GetStack()==null)
            SetItem(item);
        else
        {
            // otherwise try to combine
            if (item.Type.Equals(GetStack().Type))
            {
                var combinedStacks=CombineStacks(new StackData(item));
                // there is no remainder
                if (combinedStacks[1] == null)
                {
                    GetStack().ItemCount = combinedStacks[0].count;
                    Destroy(item.gameObject);
                }
                // there is a remainder
                else
                {
                    originalSlot.SetItem(combinedStacks[1]);
                    // setting the item could change the inventory
                    combinedStacks = CombineStacks(new StackData(item));
                    SetItem(combinedStacks[0]);
                    Destroy(item.gameObject);
                }
            }
            // otherwise swap
            else if(canAcceptItems && originalSlot.canAcceptItems)
            {
                var thisStack = GetStack();
                DetatchChild();
                originalSlot.SetItem(thisStack);
                SetItem(item);
            }
        }
    }



}
