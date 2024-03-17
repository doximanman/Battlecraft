using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private GameObject stackPrefab;

    // initialized on SetItem
    // removed on RemoveItem
    private ItemStack stack;

    public ItemStack GetStack()
    {
        return stack;
    }

    // combines stack into current stack
    // returns remainder
    // null if no remainder
    public ItemStack CombineStacks(ItemStack stack)
    {
        var thisStack = GetStack();
        if (thisStack==null)
        {
            // if no stack - use new stack
            SetItem(stack);
            return null;
        }
        // there is a stack - find how many items
        // need to move into this stack and move them.
        int combinedCounts = thisStack.ItemCount + stack.ItemCount;
        if(combinedCounts > stack.Type.maxStack)
        {
            // too many items for one stack
            // update stack count and return remainder
            // remainder is the updated passed parameter
            int remainder=combinedCounts - stack.Type.maxStack;
            thisStack.ItemCount=stack.Type.maxStack;
            stack.ItemCount = remainder;
            return stack;
        }
        // items can combine without remainder
        thisStack.ItemCount = combinedCounts;
        return null;
    }

    // create new stack
    public void SetItem(StackData stack)
    {
        if (stack == null || !stack.IsDefined())
        {
            RemoveItem();
            return;
        }

        GameObject newStack = Instantiate(stackPrefab, transform);
        this.stack=newStack.GetComponent<ItemStack>();
        this.stack.Type = stack.type;
        this.stack.ItemCount = stack.count;
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
    }

    // only 1 item of that type
    public void SetItem(ItemType item)
    {
        if (item == null)
        {
            RemoveItem();
            return;
        }

        GameObject newStack = Instantiate(stackPrefab, transform);
        newStack.GetComponent<ItemStack>().Type = item;
        this.stack = newStack.GetComponent<ItemStack>();
    }

    // only 1 item of that type
    public void RemoveOne()
    {
        if(GetStack()!=null)
        {
            if (GetStack().ItemCount == 1) RemoveItem();
            else GetStack().ItemCount--;
        }
    }

    // destroy item object
    public void RemoveItem()
    {
        if (GetStack() != null) Destroy(GetStack().gameObject);
        stack = null;
    }

    // dont destroy item object
    public void DetatchChild()
    {
        transform.DetachChildren();
        stack = null;
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
        if (stack!=null)
        {
            // if its the same itemtype, try to combine stacks
            if (stack.Type.Equals(item.Type))
            {
                var remainder = CombineStacks(item);
                if (remainder == null)
                {
                    // stacks were combined with no remainder
                    // old object discarded
                    original.RemoveItem();
                }
                else
                {
                    // stacks were combined, but there is a remainder
                    // remainder remains in original slot
                    original.SetItem(remainder);
                }
            }
            else
            {
                // move current item into old slot
                // and the dragged item into this slot
                original.DetatchChild();
                original.SetItem(stack);
                DetatchChild();
                SetItem(item);
            }
        }
        else
        {
            // move the item to this slot
            original.DetatchChild();
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
        if (ItemStack.stopDrag || eventData.pointerDrag == null) return;

        // the dragged item
        var item = eventData.pointerDrag.GetComponent<ItemStack>();
        if(!item) return;
        MoveFrom(item,item.originalParent.GetComponent<InventorySlot>());
    }



}
