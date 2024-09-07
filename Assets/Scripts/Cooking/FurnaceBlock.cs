using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceBlock : Interactable
{
    /// <summary>
    /// properties defining the furnace: <br />
    /// (minFuel, maxFuel)
    /// </summary>
    public FurnaceProperties properties = null;

    /// <summary>
    /// State of the furnace: <br />
    /// (fuel, inItem, outItem)
    /// </summary>
    public FurnaceState state = null;

    [SerializeField] private ItemType furnaceType;

    private ItemType cooking = null;
    public override void Start()
    {
        base.Start();

        if (state == null || properties == null)
        {
            // uninitialized - initialize with default values
            properties = new(FurnaceLogic.defaultProperties);
            state = new(FurnaceLogic.defaultState);
        }

        state.stateChangeListener += (FurnaceState.StateChangeType type) =>
        {
            SlotData fuelSlot = state.FuelSlot;

            switch (type)
            {
                // when only fuel is updated, no need to do the entire cook check
                case FurnaceState.StateChangeType.FUEL:
                    UpdateFuel();
                    break;
                case FurnaceState.StateChangeType.FUEL_SLOT:
                    UpdateFuel();
                    break;
                default:
                    UpdateCook();
                    break;
            }
        };
    }

    /// <summary>
    /// try to consume fuel if possible.<br/>
    /// updates according to current state.
    /// </summary>
    public void UpdateFuel()
    {
        if (state.FuelSlot.stack == null)
            return;

        StackData fuelStack = state.FuelSlot.stack;
        FuelStats fuelStats = state.FuelSlot.stack.type.fuelStats;
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
            state.FuelSlot.stack = newFuelStack;
            // start timer of fuel usage
            if (state.Fuel == 0)
                fuelTimer = 0;
            // update state
            state.Fuel += fuelToAdd;
            state.FuelSlot = state.FuelSlot;
        }
    }

    /// <summary>
    /// Checks if done cooking or starts new cooking and does
    /// consequent operations
    /// </summary>
    public void UpdateCook()
    {
        SlotData inSlot = state.InSlot;
        SlotData outSlot = state.OutSlot;

        // inslot has cookable item
        if (inSlot.stack != null && inSlot.stack.type.cookable)
        {
            StackData itemStack = inSlot.stack;
            // if it is the same type of the type that is currently
            // cooking - keep cooking it and don't change anything.
            // otherwise stop the cooking.
            if (itemStack.type != cooking)
            {
                // if outslot can accept the cooking result start cooking
                if (outSlot.CanAccept(itemStack.type.cookResult))
                {
                    cookTimer = 0;
                    cooking = itemStack.type;
                    cookSpeed = 1.0f / itemStack.type.cookTime;
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
            if (cooking != null)
            {
                cooking = null;
                cookSpeed = 0;
                state.CookProgress = 0;
            }
        }

        // check if done cooking

        float progress = state.CookProgress;

        // done cooking
        // note: all the other listeners ensure that if
        // progress is 1, necessarily something is being cooked,
        // and the outslot can accept it.
        if (Mathf.Approximately(progress, 1))
        {
            // reset cook progress
            state.CookProgress = 0;
            // remove from inSlot and add to outSlot
            inSlot.Remove(1);
            outSlot.Add(cooking.cookResult);
            state.InSlot = inSlot;
            state.OutSlot = outSlot;
        }
    }

    public override void OnInteract()
    {
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

    public override void OnFinishChopping()
    {
        // drop all items
        DroppedStacksManager.instance.Drop(new List<StackData>(){state.FuelSlot.stack,
                                                            state.InSlot.stack,
                                                            state.OutSlot.stack }, transform.position);

        // create itemtype to be dropped when destroyed
        ItemType furnace = Instantiate(furnaceType);
        furnace.name = furnaceType.name;
        // drop the furnace
        DroppedStacksManager.instance.Drop(new StackData(furnace, 1), transform.position);

        // remove chest block from the scene.
        Destroy(gameObject);
    }

    public override JObject SaveInternal()
    {
        return new()
        {
            ["props"] = FurnaceProperties.Serialize(properties),
            ["state"] = FurnaceState.Serialize(state),
            ["cooking"] = cooking == null ? "None" : cooking.name,
        };
    }

    public override void LoadInternal(JObject data)
    {
        properties = FurnaceProperties.Deserialize(data["props"] as JObject);
        state = FurnaceState.Deserialize(data["state"] as JObject);
        cooking = ItemTypes.GetByName(data["cooking"].ToString());
        cookSpeed = 1.0f / cooking.cookTime;
    }

    private float cookSpeed;

    private float fuelTimer = 0;
    private float cookTimer = 0;
    private void Update()
    {
        // otherwise - fuel check and cook progress
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
            if (state.Fuel > 0)
                state.CookProgress += cookSpeed * FurnaceLogic.fuelUpdateRate;
            else
                // reset progress when fuel runs out
                state.CookProgress = 0;
            cookTimer = 0;
        }
    }
}
