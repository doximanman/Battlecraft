using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct FurnaceState
{
    public float minFuel;
    public float maxFuel;
    public float fuel;

    public FurnaceState(float minFuel, float maxFuel, float fuel)
    {
        this.minFuel = minFuel;
        this.maxFuel = maxFuel;
        this.fuel = fuel;
    }

    public void Set(float minFuel, float maxFuel, float fuel)
    {
        this.minFuel = minFuel;
        this.maxFuel = maxFuel;
        this.fuel = fuel;
    }
}

public class Furnace : MonoBehaviour
{
    [SerializeField] private FuelMeter fuel;
    [SerializeField] private InventorySlot fuelSlot;
    [SerializeField] private InventorySlot itemSlot;
    [SerializeField] private InventorySlot outSlot;
    [SerializeField] private Slider cookProgress;
    [SerializeField] private float fuelUsageSpeed;

    /// <summary>
    /// state of the furnace: <br></br>
    /// (minFuel, maxFuel, currentFuel)
    /// </summary>
    public FurnaceState State
    {
        get => new(fuel.MinFuel, fuel.MaxFuel, fuel.Fuel);
        set
        {
            fuel.MinFuel = value.minFuel;
            fuel.MaxFuel = value.maxFuel;
            fuel.Fuel = value.fuel;
        }
    }

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

        itemSlot.slotChangeListeners += (StackData _, StackData newStack) =>
        {
            // item slot changed - reset cooking process
            cookProgress.value = 0;
            cooking = null;

            // if nothing in the item slot don't do anything
            if (newStack == null) return;
            ItemType type = newStack.type;
            
            // if item can't be cooked don't do anything
            if (!type.cookable) return;

            // check whether cooking result can be placed in out slot
            // (can happen if there is an item from a previous cooking)
            // also, remainder must be 0. you cannot partially cook an item.
            int? canFit = outSlot.CanAccept(type.cookResult);
            if ((!canFit.HasValue) || (canFit.Value > 0)) return;

            // set the speed to be such that after cookTime seconds
            // the cooking will be done
            cookSpeed = 1.0f/newStack.type.cookTime;
            // item being cooked
            cooking = newStack.type;

        };
    }

    private ItemType cooking;
    private float cookSpeed;

    private void FixedUpdate()
    {
        fuel.Fuel -= fuelUsageSpeed * Time.fixedDeltaTime;

        if(fuel.Fuel > 0)
        {
            cookProgress.value += cookSpeed * Time.fixedDeltaTime;
            if(cookProgress.value == cookProgress.maxValue)
            {
                // all the validity checks were done 
                // before the cooking started - just add
                // the result to the slot.
                StackData cookResult = cooking.cookResult;
                outSlot.CombineFrom(cookResult);
                itemSlot.RemoveOne();
            }

        }
    }
}
