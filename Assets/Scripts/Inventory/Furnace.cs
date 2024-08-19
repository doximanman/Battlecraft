using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Furnace : MonoBehaviour
{
    [SerializeField] private FuelMeter fuel;
    [SerializeField] private InventorySlot fuelSlot;
    [SerializeField] private InventorySlot itemSlot;
    [SerializeField] private InventorySlot outSlot;
    [SerializeField] private Slider cookProgress;
    [SerializeField] private float cookSpeed;

    private void Start()
    {
        fuelSlot.slotChangeListeners += (StackData _,StackData newStack) =>
        {
            if (newStack == null) return;
            ItemType type = newStack.type;
            // try to use the fuel item
            // if type is a fuel and the amount it gives is less than the available fuel capacity
            if (newStack.type.fuel && type.fuelStats.fuelAmount < fuel.Remainder)
            {
                fuel.Fuel += type.fuelStats.fuelAmount;
                fuelSlot.RemoveOne();
            }
        };
    }

    private void FixedUpdate()
    {
        fuel.Fuel -= cookSpeed / Time.fixedDeltaTime;
    }
}
