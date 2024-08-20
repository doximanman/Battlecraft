using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// constant properties of a furnace. they don't change.
/// </summary>
public class FurnaceProperties
{
    public float minFuel;
    public float maxFuel;

    /// <summary>
    /// copy constructor
    /// </summary>
    /// <param name="other">to copy from</param>
    public FurnaceProperties(FurnaceProperties other)
    {
        minFuel = other.minFuel;
        maxFuel = other.maxFuel;
    }

    public FurnaceProperties(float minFuel, float maxFuel)
    {
        this.minFuel = minFuel;
        this.maxFuel = maxFuel;
    }

    public void Set(float minFuel, float maxFuel)
    {
        this.minFuel = minFuel;
        this.maxFuel = maxFuel;
    }
}

public struct FurnaceItemSnapshot
{
    public SlotData fuelSlot;
    public SlotData inSlot;
    public SlotData outSlot;

    public FurnaceItemSnapshot(SlotData fuelSlot, SlotData inSlot, SlotData outSlot)
    {
        this.fuelSlot = fuelSlot;
        this.inSlot = inSlot;
        this.outSlot = outSlot;
    }

    public FurnaceItemSnapshot(FurnaceItemSnapshot other)
    {
        fuelSlot = other.fuelSlot;
        inSlot = other.inSlot;
        outSlot = other.outSlot;
    }
}

/// <summary>
/// Entire state of a furnace.
/// Fuel amount and item in each slot.
/// </summary>
public class FurnaceState
{
    public delegate void FuelChangeListener(float oldFuel, float newFuel);

    public delegate void ProgressChangeListener(float oldProgress, float newProgress);

    public delegate void ItemChangeListener(FurnaceItemSnapshot oldState,
                                                FurnaceItemSnapshot newState);


    public FurnaceProperties props;

    private float fuel;
    private float cookProgress;
    private SlotData fuelSlot;
    private SlotData inSlot;
    private SlotData outSlot;

    /// <summary>
    /// deep copy constructor
    /// </summary>
    /// <param name="other">to copy from</param>
    public FurnaceState(FurnaceState other)
    {
        props = new(other.props);
        fuel = other.fuel;
        cookProgress = other.cookProgress;
        fuelSlot = new(other.fuelSlot);
        inSlot = new(other.inSlot);
        outSlot = new(other.outSlot);
    }

    public FurnaceState(
        FurnaceProperties props,
        float fuel,
        float cookProgress,
        SlotData fuelSlot,
        SlotData inSlot,
        SlotData outSlot
        )
    {
        this.props = props;
        this.fuel = fuel;
        this.cookProgress = cookProgress;
        this.fuelSlot = fuelSlot;
        this.inSlot = inSlot;
        this.outSlot = outSlot;
    }

    // seperation between fuel changes and item changes
    // to make it more lightweight
    // (otherwise you had to check every time fuel changes whether
    // the items changed, which is a comparison of complex objects,
    // and fuel changes a lot (every fixedupdate)).

    /// <summary>
    /// listen to fuel amount changes.
    /// </summary>
    public FuelChangeListener fuelChangeListener;
    // higher priority delegate, for example for UI.
    public FuelChangeListener fuelChangeListenerHighPriority;
    /// <summary>
    /// listen to any change in the items in the slots.
    /// not notified of other changes (like fuel).
    /// </summary>
    public ItemChangeListener itemChangeListener;
    public ItemChangeListener itemChangeListenerHighPriority;
    /// <summary>
    /// listen to cook progress changes
    /// </summary>
    public ProgressChangeListener progressChangeListener;
    public ProgressChangeListener progressChangeListenerHighPriority;

    public float Fuel
    {
        get => fuel;
        set
        {
            float oldFuel = fuel;
            fuel = Mathf.Clamp(value,props.minFuel,props.maxFuel);
            fuelChangeListenerHighPriority?.Invoke(oldFuel, fuel);
            fuelChangeListener?.Invoke(oldFuel,fuel);
        }
    }

    public float CookProgress
    {
        get => cookProgress;
        set
        {
            // progress is 0 to 1
            float oldCookProgress = cookProgress;
            cookProgress = Mathf.Clamp(value,0,1);
            progressChangeListenerHighPriority?.Invoke(oldCookProgress, cookProgress);
            progressChangeListener?.Invoke(oldCookProgress,cookProgress);
        }
    }

    public SlotData FuelItem
    {
        get => fuelSlot;
        set
        {
            FurnaceItemSnapshot oldState = new(fuelSlot, inSlot, outSlot);
            FurnaceItemSnapshot newState = new(value, inSlot, outSlot);
            fuelSlot = value;
            itemChangeListenerHighPriority?.Invoke(oldState, newState);
            itemChangeListener?.Invoke(oldState, newState);
        }
    }

    public SlotData InItem
    {
        get => inSlot;
        set
        {
            FurnaceItemSnapshot oldState = new(fuelSlot, inSlot, outSlot);
            FurnaceItemSnapshot newState = new(fuelSlot, value, outSlot);
            inSlot = value;
            itemChangeListenerHighPriority?.Invoke(oldState, newState);
            itemChangeListener?.Invoke(oldState, newState);
        }
    }

    public SlotData OutItem
    {
        get => outSlot;
        set
        {
            FurnaceItemSnapshot oldState = new(fuelSlot, inSlot, outSlot);
            FurnaceItemSnapshot newState = new(fuelSlot, inSlot, value);
            outSlot = value;
            itemChangeListenerHighPriority?.Invoke(oldState, newState);
            itemChangeListener?.Invoke(oldState, newState);
        }
    }

    public override string ToString()
    {
        return "Fuel: " + fuel + " cookProgress: " + cookProgress + " fuelSlot: " + fuelSlot + " itemSlot: " + inSlot + " outSlot: " + outSlot;
    }
}
