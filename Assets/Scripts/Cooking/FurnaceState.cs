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


/// <summary>
/// Entire state of a furnace.
/// Fuel amount and item in each slot.
/// </summary>
public class FurnaceState
{
    public enum StateChangeType {FUEL,COOK_PROGRESS,FUEL_SLOT,IN_SLOT,OUT_SLOT};

    public delegate void StateChangeListener(StateChangeType type);

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

    /// <summary>
    /// Get notified when state changes.
    /// Receive what was changed in function parameter.
    /// By having a reference to the state, the listener
    /// has access to the new values.
    /// </summary>
    public StateChangeListener stateChangeListener;
    /// <summary>
    /// Higher priority delegate, called before normal delegate.
    /// </summary>
    public StateChangeListener stateChangeListenerHighPriority;

    public float Fuel
    {
        get => fuel;
        set
        {
            fuel = Mathf.Clamp(value,props.minFuel,props.maxFuel);
            stateChangeListenerHighPriority?.Invoke(StateChangeType.FUEL);
            stateChangeListener?.Invoke(StateChangeType.FUEL);
        }
    }

    public float CookProgress
    {
        get => cookProgress;
        set
        {
            // progress is 0 to 1
            cookProgress = Mathf.Clamp(value,0,1);
            stateChangeListenerHighPriority?.Invoke(StateChangeType.COOK_PROGRESS);
            stateChangeListener?.Invoke(StateChangeType.COOK_PROGRESS);
        }
    }

    public SlotData FuelSlot
    {
        get => fuelSlot;
        set
        {
            fuelSlot = value;
            stateChangeListenerHighPriority?.Invoke(StateChangeType.FUEL_SLOT);
            stateChangeListener?.Invoke(StateChangeType.FUEL_SLOT);
        }
    }

    public SlotData InSlot
    {
        get => inSlot;
        set
        {
            inSlot = value;
            stateChangeListenerHighPriority?.Invoke(StateChangeType.IN_SLOT);
            stateChangeListener?.Invoke(StateChangeType.IN_SLOT);
        }
    }

    public SlotData OutSlot
    {
        get => outSlot;
        set
        {
            outSlot = value;
            stateChangeListenerHighPriority?.Invoke(StateChangeType.OUT_SLOT);
            stateChangeListener?.Invoke(StateChangeType.OUT_SLOT);
        }
    }

    public override string ToString()
    {
        return "Fuel: " + fuel + " cookProgress: " + cookProgress + " fuelSlot: " + fuelSlot + " itemSlot: " + inSlot + " outSlot: " + outSlot;
    }
}
