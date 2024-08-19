using Codice.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FurnaceLogic : MonoBehaviour
{
    private static Furnace furnace;

    [InspectorName("Furnace")]
    [SerializeField] private Furnace _furnace;

    // default values for the fields.
    // an uninitialized furnace will have these values at first.
    [InspectorName("Default Minimum Fuel")]
    [SerializeField] private float _defaultMinFuel;
    [InspectorName("Default Maximum Fuel")]
    [SerializeField] private float _defaultMaxFuel;
    [InspectorName("Default Fuel")]
    [SerializeField] private float _defaultFuel;

    public static FurnaceState defaultValues;

    public static bool enableFurnace = false;

    public delegate void FurnaceListener(bool open);
    /// <summary>
    /// notify when furnace is opened/closed
    /// </summary>
    public static FurnaceListener furnaceListeners;

    private void Start()
    {
        furnace = _furnace;
        defaultValues = new(_defaultMinFuel,_defaultMaxFuel,_defaultFuel);

        furnace.gameObject.SetActive(false);

        InventoryLogic.invListeners += (on) =>
        {
            // it is my responsibility to handle the second inventory
            if (InventoryLogic.responsible != InventoryLogic.Responsible.FURNACE) return;

            // if the furnace is enabled - notify when the inventory opens and closes
            if (enableFurnace && on)
            {
                furnaceListeners?.Invoke(true);
                furnace.gameObject.SetActive(true);
                InventoryLogic.externalInventory.SetSlots(furnace.GetComponent<SlotList>().slots);
            }
            else
            {
                furnaceListeners?.Invoke(false);
                furnace.gameObject.SetActive(false);
            }
        };
    }

    public static void LoadState(FurnaceState state)
    {
        furnace.State = state;
    }

    public static FurnaceState GetState()
    {
        return furnace.State;
    }

    public static void EnableFurnace()
    {
        enableFurnace = true;
        InventoryLogic.responsible = InventoryLogic.Responsible.FURNACE;
    }

    public static void DisableFurnace()
    {
        enableFurnace = false;
        InventoryLogic.responsible = InventoryLogic.defaultResponsible;
    }
}
