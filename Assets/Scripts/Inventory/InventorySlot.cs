using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class InventorySlot : MonoBehaviour, IDropHandler
{

    public bool canAcceptItems;

    // initialized on SetItem
    // removed on RemoveItem
    private ItemStack stack;

    public delegate void InventorySlotChange(StackData oldData,StackData newData);
    public InventorySlotChange slotChangeListeners;


    public ItemStack GetStack()
    {
        return stack;
    }

    public bool TryGetStack(out ItemStack stack)
    {
        stack = this.stack;
        if (this.stack == null || this.stack.Equals(null))
            return false;
        else
            return true;
    }

    private StackData oldStack;
    private void NotifyChange()
    {
        if (slotChangeListeners != null)
        {
            StackData notifyOldStack = oldStack ?? null;
            StackData notifyNewStack = stack == null ? null : new StackData(stack);
            slotChangeListeners(notifyOldStack, notifyNewStack);
        }
        if (stack == null) oldStack = null;
        else oldStack = new StackData(stack);
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
        this.stack.ItemCount = newCount;
        Destroy(stack.gameObject);
    }

    public void CombineFrom(StackData stack)
    {
        int newCount = Mathf.Min(stack.type.maxStack, stack.count + this.stack.ItemCount);
        this.stack.ItemCount = newCount;
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
            GameObject newStack = Instantiate(Prefabs.stackPrefab, transform);
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
            GameObject newStack = Instantiate(Prefabs.stackPrefab, transform);
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

    public void AddOne(bool notifyChange = true)
    {
        if (GetStack() != null)
        {
            if (GetStack().ItemCount == GetStack().Type.maxStack) return;
            GetStack().ItemCount++;
            if (notifyChange) NotifyChange();
        }
    }

    public void AddSome(int count)
    {
        for(int i=0;i<count; i++)
        {
            AddOne(false);
        }
        if(count > 0) NotifyChange();
    }

    // only 1 item of that type
    public void RemoveOne(bool notifyChange = true)
    {
        if (GetStack() != null)
        {
            if (GetStack().ItemCount == 1) RemoveItem();
            else GetStack().ItemCount--;
            if(notifyChange) NotifyChange();
        }
    }

    // remove count of that type
    public void RemoveSome(int count)
    {
        for (int i = 0; i < count; i++)
        {
            RemoveOne(false);
        }
        if(count > 0) NotifyChange() ;
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

        // notify EndDrag that the dragging operation already happened
        ItemStack.stopDrag = true;

        var item = ItemStack.beingDragged;
        var originalSlot = ItemStack.originalSlot;
        // left click

        // if no item here - set it to be the stack.
        if (GetStack() == null)
            SetItem(item);
        else
        {
            // otherwise try to combine
            if (item.Type.Equals(GetStack().Type))
            {
                var combinedStacks = CombineStacks(new StackData(item));
                // there is no remainder
                if (combinedStacks[1] == null)
                {
                    GetStack().ItemCount = combinedStacks[0].count;
                    Destroy(item.gameObject);
                }
                // there is a remainder
                else
                {
                    if (originalSlot.canAcceptItems)
                    {
                        // right click - add the remainder to the existing amount in the slot
                        if (ItemStack.rightClick)
                            originalSlot.CombineFrom(combinedStacks[1]);
                        // left click - the slot is empty so set the item to be the entire stack
                        else
                            originalSlot.SetItem(combinedStacks[1]);
                    }

                    // setting the item could change the inventory
                    combinedStacks = CombineStacks(new StackData(item));
                    SetItem(combinedStacks[0]);
                    Destroy(item.gameObject);

                    if (!originalSlot.canAcceptItems)
                    {
                        // cant accept items - try to place it in the inventory
                        InventoryLogic.personalInventory.AddItems(combinedStacks[1].type, combinedStacks[1].count);
                    }


                }
            }
            // otherwise swap only if the original slot is empty now
            else if (canAcceptItems && originalSlot.canAcceptItems && originalSlot.GetStack()==null)
            {
                var thisStack = GetStack();
                DetatchChild();
                originalSlot.SetItem(thisStack);
                SetItem(item);
            }
            // else just try to put it in the inventory of the player
            else
            {
                InventoryLogic.personalInventory.AddStack(item);
            }
        }

    }



}
