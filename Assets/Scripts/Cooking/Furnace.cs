using Codice.CM.Common.Tree;
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
    [SerializeField] private float fuelUsageSpeed;

    private FurnaceProperties props;

    /// <summary>
    /// properties of the furnace: <br></br>
    /// (minFuel, maxFuel)
    /// </summary>
    public FurnaceProperties Props
    {
        get => props;
        set
        {
            fuel.MinFuel = value.minFuel;
            fuel.MaxFuel = value.maxFuel;
            props = value;
        }
    }

    private FurnaceState state = null;
    /// <summary>
    /// loads the state into the furnace UI
    /// </summary>
    /// <param name="state">state to load from</param>
    public void LoadState(FurnaceState state)
    {
        // unregister from old state
        if(this.state != null)
        {
            this.state.fuelChangeListenerHighPriority -= OnFuelChange;
            this.state.itemChangeListenerHighPriority -= OnItemChange;
            this.state.progressChangeListenerHighPriority -= OnProgressChange;
        }

        // register to new state
        // set values
        this.state = state;
        fuel.Fuel = state.Fuel;
        fuelSlot.SetItem(state.FuelItem.stack);
        itemSlot.SetItem(state.InItem.stack);
        outSlot.SetItem(state.OutItem.stack);

        // register listeners
        state.fuelChangeListenerHighPriority += OnFuelChange;
        state.itemChangeListenerHighPriority += OnItemChange;
        state.progressChangeListenerHighPriority += OnProgressChange;
    }

    public void UnloadState()
    {
        if(state != null)
        {
            state.fuelChangeListener -= OnFuelChange;
            state.itemChangeListener -= OnItemChange;
            state.progressChangeListener -= OnProgressChange;
        }
        state = null;
    }

    public void OnFuelChange(float oldFuel, float newFuel)
    {
        if (fuel.Fuel != newFuel)
            fuel.Fuel = newFuel;
    }

    public void OnProgressChange(float oldProgress, float newProgress)
    {
        if (cookProgress.value != newProgress)
            cookProgress.value = newProgress;
    }

    public void OnItemChange(FurnaceItemSnapshot oldState, FurnaceItemSnapshot newState)
    {
        SlotData fuelItem = newState.fuelSlot;
        SlotData inItem = newState.inSlot;
        SlotData outItem = newState.outSlot;

        // update everything
        if(!fuelSlot.HasExact(fuelItem.stack))
            fuelSlot.SetItem(fuelItem.stack);
        if(!itemSlot.HasExact(inItem.stack))
            itemSlot.SetItem(inItem.stack);
        if(!outSlot.HasExact(outItem.stack))
            outSlot.SetItem(outItem.stack);
    }

    private void Start()
    {

        fuelSlot.slotChangeListeners += (StackData _,StackData _) =>
        {
            // set state's fuel item to have this value.
            state.FuelItem = fuelSlot.ToData();
            /*if (newStack == null) return;
            ItemType type = newStack.type;
            // try to use the fuel item
            // if type is a fuel and the amount it gives is less than the available fuel capacity
            if (newStack.type.fuel && type.fuelStats.fuelAmount < fuel.Remainder)
            {
                fuel.Fuel += type.fuelStats.fuelAmount;
                fuelSlot.RemoveOne();
            }*/
        };

        itemSlot.slotChangeListeners += (StackData _, StackData _) =>
        {
            state.InItem = itemSlot.ToData();
            /*// item slot changed - reset cooking process
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
            cooking = newStack.type;*/
        };

        outSlot.slotChangeListeners += (StackData _, StackData _) =>
        {
            state.OutItem = outSlot.ToData();
        };
    }

    private ItemType cooking;
    private float cookSpeed;

    private void FixedUpdate()
    {
        /**/
    }
}
