using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInterations : MonoBehaviour
{
    public Inventory inventory;
    public float pickupRange;
    public Vector2 maxThrowSpeed;
    public Vector2 minThrowSpeed;

    void Start()
    {
        DroppedStacksManager.instance.RegisterCloseListener(gameObject, pickupRange, (droppedStack) =>
        {
            // try to add to inventory
            int remainder=inventory.AddItems(droppedStack.Stack.type, droppedStack.Stack.count);
            droppedStack.Stack.count = remainder;
        });

        PlayerControl control=GetComponent<PlayerControl>();
        System.Random rand = new();

        DroppedStacksManager.instance.dropPosition = () => transform.position;
        DroppedStacksManager.instance.dropVelocity = () =>
        {
            Direction facing = control.Facing;
            float xSpeed = GenerateFloat(rand, minThrowSpeed.x, maxThrowSpeed.x);
            if (xSpeed < 0 && facing == Direction.RIGHT
                || xSpeed > 0 && facing == Direction.LEFT)
                xSpeed = -xSpeed;
            return new(xSpeed,GenerateFloat(rand, minThrowSpeed.y, maxThrowSpeed.y));
        };
    }

    private float GenerateFloat(System.Random rand, float min, float max) => min + (max - min) * (float)rand.NextDouble();

    /// <summary>
    /// Drop the contents of a slot
    /// </summary>
    /// <param name="slot">Slot to drop from</param>
    /// <returns>True on success, false on failure</returns>
    public bool DropSlot(InventorySlot slot,bool throwItem=true)
    {
        if (!throwItem)
        {
            System.Random rand = new();
            // if don't throw - drop it with a random x velocity 
            // according to minThrowSpeed
            float xSpeed = GenerateFloat(rand,-minThrowSpeed.x, minThrowSpeed.x);
            Vector2 velocity = new(xSpeed, 0);
            DroppedStacksManager.instance.dropVelocity = () => velocity;
        }

        // slot must be of this inventory
        if (!inventory.slots.Contains(slot)) return false;

        // drop the item
        if(slot.TryGetStack(out ItemStack stack))
        {
            DroppedStacksManager.instance.Drop(new(stack));
        }

        // remove the stack from the slot
        slot.RemoveItem();
        return true;
    }

    /// <summary>
    /// Drop entire inventory
    /// </summary>
    /// <returns>True if all items dropped, false if any item failed to drop.</returns>
    [ContextMenu("Drop Inventory")]
    public bool DropInventory(bool throwItems=true)
    {
        bool result=true;
        // drop items from all slots
        foreach(var slot in inventory)
        {
            bool success=DropSlot(slot,throwItems);
            result = result && success;
        }

        // result is true if all the items dropped properly.
        return result;
    }
}
