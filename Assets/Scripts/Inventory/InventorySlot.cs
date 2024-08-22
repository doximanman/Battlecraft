using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public bool canAcceptItems;
    public FilterType filterType;
    public List<ItemType> filter;

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
        if (this.stack == null)
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
    public void SetItem(StackData stack, bool notify = true)
    {
        if (stack == null)
        {
            RemoveItem(notify);
            return;
        }

        if (this.stack == null)
        {
            GameObject newStack = Instantiate(Prefabs.stackPrefab, transform);
            this.stack = newStack.GetComponent<ItemStack>();
        }
        this.stack.Type = stack.type;
        this.stack.ItemCount = stack.count;
        if(notify)
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
    public void RemoveItem(bool notify = true)
    {
        if (GetStack() != null) Destroy(GetStack().gameObject);
        stack = null;
        if(notify)
            NotifyChange();
    }

    // dont destroy item object
    public void DetatchStack()
    {
        foreach (Transform t in transform)
            if (t.TryGetComponent<ItemStack>(out ItemStack _))
                t.SetParent(null);

        //transform.DetachChildren();
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

    // general, whether the slot can in general accept this type,
    // doesn't take into account current slot state.
    public bool CanAccept(ItemType type)
    {
        if (!canAcceptItems) return false;

        return filterType switch
        {
            FilterType.WHITE_LIST => filter.Contains(type),
            FilterType.BLACK_LIST => !filter.Contains(type),
            _ => false,
        };
    }

    // whether the slot can fit adding the stack to it
    // also does the normal filter check
    // returns null if it cannot fit, the remainder of combining the stacks if it can.
    public int? CanAccept(StackData stack)
    {
        if (TryGetStack(out ItemStack currentStack))
        {
            // there is a stack in the slot currently - check whether the stack
            // can be combined into the stack already inside the slot.
            // i.e. check types, if they are the same return remainder of combining.
            if (currentStack.Type == stack.type)
            {
                int combinedCount = stack.count + currentStack.ItemCount;
                if (combinedCount > stack.type.maxStack)
                    return combinedCount - stack.type.maxStack;
                return 0;
            }
            // items not of the same type, cannot fit
            return null;
        }
        // slot is empty - return whether the slot accepts the type.
        // 0 means no remainder - because the entire stack can be added with no remainder.
        else return CanAccept(stack.type)? 0 : null;
    }

    /// <summary>
    /// whether the slot has this stack, at the exact amount. <br/>
    /// meaning, same type and count.
    /// </summary>
    /// <param name="stack">to compare to</param>
    /// <returns>true if stack has this stack at the exact amount, false otherwise.</returns>
    public bool HasExact(StackData stack)
    {
        // if stack is empty return if this slot is empty
        if(stack == null)
            return GetStack() == null;

        // otherwise, stack is not empty, therefore if this slot is empty
        // it doesn't contain the stack.
        if (GetStack() == null) return false;

        ItemStack currentStack = GetStack();
        return currentStack.Type == stack.type && currentStack.ItemCount == stack.count;
    }

    /// <summary>
    /// get SlotData representation of the slot
    /// </summary>
    /// <returns>slot data with all properties of the slot</returns>
    public SlotData ToData()
    {
        // slot data with the properties of this slot
        SlotData data = new(null,canAcceptItems,filterType,filter);

        // appropriate stack - if null then null,
        // otherwise the existing stack.
        if (GetStack() != null)
            data.stack = new(GetStack());
        return data;
    }
    public void OnDrop(PointerEventData eventData)
    {
        var item = ItemStack.beingDragged;
        var originalSlot = ItemStack.originalSlot;
        bool thisCanAccept = CanAccept(item.Type);

        // if drag is stopped or nothing was dragged
        if (ItemStack.stopDrag || eventData.pointerDrag == null || !thisCanAccept) return;

        bool originalCanAccept = originalSlot.CanAccept(item.Type);

        // notify EndDrag that the dragging operation was handled (by this function)
        ItemStack.stopDrag = true;

        // left click

        // if no item here - set it to be the stack.
        if (GetStack() == null)
            SetItem(item);
        else
        {
            // otherwise try to combine
            if (item.Type == GetStack().Type)
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
                    if (originalCanAccept)
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

                    if (!originalCanAccept)
                    {
                        // cant accept items - try to place it in the inventory
                        InventoryLogic.personalInventory.AddItems(combinedStacks[1].type, combinedStacks[1].count);
                    }


                }
            }
            // otherwise swap only if the original slot is empty now
            else if (thisCanAccept && originalCanAccept && originalSlot.GetStack() == null)
            {
                var thisStack = GetStack();
                DetatchStack();
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

[CustomEditor(typeof(InventorySlot))]
public class SlotEditor : Editor
{
    SerializedProperty canAcceptItems;
    SerializedProperty filterType;
    SerializedProperty filter;

    public override VisualElement CreateInspectorGUI()
    {
        var returnValue = base.CreateInspectorGUI();
        canAcceptItems = serializedObject.FindProperty("canAcceptItems");
        filterType = serializedObject.FindProperty("filterType");
        filter = serializedObject.FindProperty("filter");
        return returnValue;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(canAcceptItems);

        if (canAcceptItems.boolValue)
        {
            EditorGUILayout.PropertyField(filterType);
            EditorGUILayout.PropertyField(filter);
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
