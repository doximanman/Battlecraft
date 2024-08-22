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
            this.state.stateChangeListenerHighPriority -= OnStateChange;
        }

        // register to new state
        // set values
        this.state = state;
        fuel.Fuel = state.Fuel;
        cookProgress.value = state.CookProgress;
        fuelSlot.SetItem(state.FuelSlot.stack);
        itemSlot.SetItem(state.InSlot.stack);
        outSlot.SetItem(state.OutSlot.stack);

        // register listeners
        state.stateChangeListenerHighPriority += OnStateChange;
    }

    public void UnloadState()
    {
        if(state != null)
        {
            state.stateChangeListenerHighPriority -= OnStateChange;
        }
        state = null;
    }

    public void OnStateChange(FurnaceState.StateChangeType type)
    {
        switch (type)
        {
            case FurnaceState.StateChangeType.FUEL:
                OnFuelChange();
                break;
            case FurnaceState.StateChangeType.COOK_PROGRESS:
                OnProgressChange();
                break;
            default:
                OnItemChange();
                break;
        }
    }

    public void OnFuelChange()
    {
        float newFuel = state.Fuel;

        if (fuel.Fuel != newFuel)
            fuel.Fuel = newFuel;
    }

    public void OnProgressChange()
    {
        float newProgress = state.CookProgress;

        if (cookProgress.value != newProgress)
            cookProgress.value = newProgress;
    }

    public void OnItemChange()
    {
        SlotData fuelItem = state.FuelSlot;
        SlotData inItem = state.InSlot;
        SlotData outItem = state.OutSlot;

        // update everything
        if(!fuelSlot.HasExact(fuelItem.stack))
            fuelSlot.SetItem(fuelItem.stack,false);
        if(!itemSlot.HasExact(inItem.stack))
            itemSlot.SetItem(inItem.stack, false);
        if(!outSlot.HasExact(outItem.stack))
            outSlot.SetItem(outItem.stack, false);
    }

    private void Start()
    {

        fuelSlot.slotChangeListeners += (StackData _,StackData _) =>
        {
            // set state's fuel item to have this value.
            state.FuelSlot = fuelSlot.ToData();
        };

        itemSlot.slotChangeListeners += (StackData _, StackData _) =>
        {
            state.InSlot = itemSlot.ToData();
        };

        outSlot.slotChangeListeners += (StackData _, StackData _) =>
        {
            state.OutSlot = outSlot.ToData();
        };
    }
}
