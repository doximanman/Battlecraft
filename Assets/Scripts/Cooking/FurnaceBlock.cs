using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
using UnityEngine;

public class FurnaceBlock : MonoBehaviour
{
    /// <summary>
    /// properties defining the furnace: <br />
    /// (minFuel, maxFuel)
    /// </summary>
    public FurnaceProperties properties;

    /// <summary>
    /// State of the furnace: <br />
    /// (fuel, inItem, outItem)
    /// </summary>
    public FurnaceState state;

    // for debugging
    public static int id = 0;
    private int thisId;

    private ItemType cooking = null;
    private void Start()
    {
        // initialize with default values
        properties = new(FurnaceLogic.defaultProperties);
        state = new(FurnaceLogic.defaultState);
        thisId = id++;

        state.itemChangeListener += (FurnaceItemSnapshot _, FurnaceItemSnapshot newItems) =>
        {
            SlotData fuelSlot = newItems.fuelSlot;
            SlotData inSlot = newItems.inSlot;
            SlotData outSlot = newItems.outSlot;

            // consume fuel if possible
            if(fuelSlot.stack!=null && fuelSlot.stack.type.fuel)
            {
                StackData fuelStack = fuelSlot.stack;
                FuelStats fuelStats = fuelSlot.stack.type.fuelStats;
                // 'possible' means adding the new fuel won't exceed
                // the maximum fuel amount of the furnace.
                float fuelRemainder = properties.maxFuel - state.Fuel;
                // maximum amount that it is possible to add of this fuel type
                int maxToAdd = Mathf.FloorToInt(fuelRemainder / fuelStats.fuelAmount);
                // add as many as possible as long it is less than the maximum possible amount
                int toAdd = Mathf.Min(maxToAdd, fuelStack.count);

                // add fuel and remove from stack
                float fuelToAdd = toAdd * fuelStats.fuelAmount;
                if (fuelToAdd > 0)
                {
                    // new stack after removal
                    StackData newFuelStack = new(fuelStack.type, fuelStack.count - toAdd);
                    fuelSlot.stack = newFuelStack;
                    // start timer of fuel usage
                    if (state.Fuel == 0)
                        fuelTimer = 0;
                    // update state
                    state.Fuel += fuelToAdd;
                    state.FuelItem = fuelSlot;
                }
            }

            // start cooking item
            // or keep cooking, if it's the same item
            if(inSlot.stack!=null && inSlot.stack.type.cookable)
            {
                StackData itemStack = inSlot.stack;
                // if it is the same type of the type that is currently
                // cooking - keep cooking it and don't change anything.
                // otherwise stop the cooking.
                if (itemStack.type != cooking)
                {
                    // if outslot can accept the cooking result start cooking
                    if(outSlot.CanAccept(itemStack.type.cookResult))
                    {
                        cookTimer = 0;
                        cooking = itemStack.type;
                        cookSpeed = 1.0f/itemStack.type.cookTime;
                        state.CookProgress = 0;
                    }
                    else
                    {
                        // outslot can't accept - stop cooking.
                        cooking = null;
                        cookSpeed = 0;
                        state.CookProgress = 0;
                    }
                }
            }
            // inslot is empty or not cookable - stop cooking
            else
            {
                if(cooking != null)
                {
                    cooking = null;
                    cookSpeed = 0;
                    state.CookProgress = 0;
                }
            }
        };

        state.progressChangeListener += (float _, float progress) =>
        {
            // done cooking
            // note: all the other listeners ensure that if
            // progress is 1, necessarily something is being cooked,
            // and the outslot can accept it.
            if (Mathf.Approximately(progress, 1))
            {
                // reset cook progress
                state.CookProgress = 0;
                // remove from inSlot and add to outSlot
                SlotData outSlot = state.OutItem;
                SlotData inSlot = state.InItem;
                inSlot.Remove(1);
                outSlot.Add(cooking.cookResult);
                state.InItem = inSlot;
                state.OutItem = outSlot;
            }
        };
    }

    private bool mouseDown = false;
    private void OnMouseDown()
    {
        mouseDown = true;
    }

    private void OnMouseUp()
    {
        // if mouse wasnt pressed down on the block, ignore
        if (!mouseDown) return;
        mouseDown = false;

        // if paused ignore
        if (MetaLogic.paused) return;

        FurnaceLogic.EnableFurnace();
        FurnaceLogic.furnaceListeners += OnFurnace;
        InventoryLogic.OpenInventory(false);
    }

    public void OnFurnace(bool open)
    {
        if (open)
        {
            FurnaceLogic.SetProps(properties);
            FurnaceLogic.LoadState(state);
        }
        else
        {
            FurnaceLogic.UnloadState();
            FurnaceLogic.DisableFurnace();
            FurnaceLogic.furnaceListeners -= OnFurnace;
        }
    }

    private float cookSpeed;

    private float fuelTimer = 0;
    private float cookTimer = 0;
    private void Update()
    {
        fuelTimer += Time.deltaTime;
        // if fuel update rate is x,
        // then every 1/x seconds an update should be done.
        if(fuelTimer >= 1.0f / FurnaceLogic.fuelUpdateRate)
        {
            // decrease fuel
            state.Fuel -= FurnaceLogic.fuelUsageSpeed * FurnaceLogic.fuelUpdateRate;
            fuelTimer = 0;
        }

        if (cookSpeed == 0)
        {
            cookTimer = 0;
            return;
        }

        cookTimer+= Time.deltaTime;
        if(cookTimer >= 1.0f / FurnaceLogic.fuelUpdateRate)
        {
            // increase cook progress
            if(state.Fuel > 0)
                state.CookProgress += cookSpeed * FurnaceLogic.fuelUpdateRate;
            cookTimer = 0;
        }
    }


    //private int counter = 0; // debugging
    private void FixedUpdate()
    {
        

        // debugging
        /*counter++;
        if(counter == 20)
        {
            Debug.Log("Id " + thisId + ": " + state);
            counter = 0;
        }*/
    }
}
