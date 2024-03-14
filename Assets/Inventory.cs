using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> slots = new();

    // Start is called before the first frame update
    void Start()
    {
        var inventoryHolders = GameObject.FindGameObjectsWithTag("SlotContainer");

        foreach(var holder in inventoryHolders)
        {
            getSlots(holder.transform);
        }
    }

    // returns success
    public bool AddItem(ItemType item)
    {
        // if the item is in the inventory, add it to an existing stack
        foreach(var slot in slots)
        {
            ItemStack itemStack = slot.GetStack();
            if (itemStack!=null && itemStack.Type.name == item.name)
            {
                if(itemStack.ItemCount < item.maxStack)
                {
                    itemStack.ItemCount++;
                    return true;
                }
            }
        }

        // item doesn't already exist (or all the stacks are full)
        // place it in an unused slot
        foreach(var slot in slots)
        {
            if (slot.GetStack() == null)
            {
                slot.SetItem(item);
                return true;
            }
        }

        // no slots available
        return false;
    }

    public ItemType itemToAdd;
    [ContextMenu("Add Item")]
    public void AddCurrentItem()
    {
        AddItem(itemToAdd);
    }

    private void getSlots(Transform inventoryGrid)
    {
        foreach(Transform child in inventoryGrid)
        {
            if (child.CompareTag("InventorySlot"))
            {
                slots.Add(child.GetComponent<InventorySlot>());
            }
        }
    }
}
