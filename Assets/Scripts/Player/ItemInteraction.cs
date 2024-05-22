using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public float pickupRange;

    void Start()
    {
        DroppedStacksManager.instance.RegisterCloseListener(gameObject, pickupRange, (droppedStack) =>
        {
            // try to add to inventory
            int remainder=InventoryLogic.personalInventory.AddItems(droppedStack.Stack.type, droppedStack.Stack.count);
            droppedStack.Stack.count = remainder;
        });

        DroppedStacksManager.instance.dropPosition = () => transform.position;
    }
}
