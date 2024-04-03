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

    private void notifyChange()
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
        notifyChange();
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
        notifyChange();
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
        notifyChange();
    }

    // only 1 item of that type
    public void RemoveOne()
    {
        if (GetStack() != null)
        {
            if (GetStack().ItemCount == 1) RemoveItem();
            else GetStack().ItemCount--;
            notifyChange();
        }
    }

    // destroy item object
    public void RemoveItem()
    {
        if (GetStack() != null) Destroy(GetStack().gameObject);
        stack = null;
        notifyChange();
    }

    // dont destroy item object
    public void DetatchChild()
    {
        transform.DetachChildren();
        stack = null;
        notifyChange();
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

    // from original to this
    public void MoveFrom(ItemStack item, InventorySlot original)
    {
        if (original == this) return;
        if (stack != null)
        {
            // if its the same itemtype, try to combine stacks
            if (stack.Type.Equals(item.Type))
            {
                var combinedStacks = CombineStacks(new StackData(item));
                if (combinedStacks[1] == null)
                {
                    // stacks can be combined with no remainder
                    // combine and remove old item
                    original.RemoveItem();
                    SetItem(combinedStacks[0]);
                }
                else
                {
                    // stacks can be combined, but there is a remainder
                    // remainder remains in original slot
                    original.SetItem(combinedStacks[1]);
                    SetItem(combinedStacks[0]);
                }
            }
            else
            {
                // move current item into old slot
                // and the dragged item into this slot
                // only if both slots can accept items
                if (canAcceptItems && original.canAcceptItems)
                {
                    original.DetatchChild();
                    original.SetItem(stack);
                    DetatchChild();
                    SetItem(item);
                }
            }
        }
        else
        {
            // move the item to this slot
            original.DetatchChild();
            original.notifyChange();
            SetItem(item);
        }
    }

    // disables dragging operation
    public void DoubleClick()
    {
        ItemStack.CancelDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (ItemStack.stopDrag || eventData.pointerDrag == null || !canAcceptItems) return;

        // the dragged item
        var item = eventData.pointerDrag.GetComponent<ItemStack>();
        if (!item) return;
        MoveFrom(item, item.originalParent.GetComponent<InventorySlot>());
    }



}
