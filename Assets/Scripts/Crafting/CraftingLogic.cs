using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingLogic : MonoBehaviour
{
    public delegate void ToggleListener(bool on);

    public static GameObject crafting;
    public static GameObject bigCrafting;

    [InspectorName("Crafting Object")]
    [SerializeField] private GameObject _crafting;
    [InspectorName("Big Crafting Object")]
    [SerializeField] private GameObject _bigCrafting;

    // Start is called before the first frame update
    void Start()
    {
        crafting = _crafting;
        bigCrafting = _bigCrafting;

        InventoryLogic.invListeners += (on) =>
        {
            // if it is my responsibility to deal with the 
            // second inventory, I will open/close one of the
            // crafting grids.
            if (!(InventoryLogic.responsible == InventoryLogic.Responsible.CRAFTING)) return;

            if (on)
            {
                // open a crafting grid
                if (enableBigCrafting)
                {
                    // if big crafting open big crafting
                    // note: setup already done because of enableBigCrafting being true.
                    // (That means EnableCraftingBench was called)
                    bigCrafting.SetActive(true);
                }
                else
                {
                    // else open small inventory crafting grid
                    InventoryLogic.externalInventory.SetSlots(crafting.GetComponent<CraftingGrid>().inSlots.Flatten());
                    crafting.SetActive(true);
                }
            }
            else
            {
                // close the crafting grid
                if (enableBigCrafting)
                    // disable bench - closure done by the CraftingBench class.
                    DisableCraftingBench();
                else
                {
                    // close small grid.
                    // move from crafting grid to inventory whatever is in it.
                    Inventory.MoveInventory(crafting.GetComponent<CraftingGrid>().inSlots.Flatten(), InventoryLogic.personalInventory);
                }
                bigCrafting.SetActive(false);
                crafting.SetActive(false);
            }
        };
    }

    public static ToggleListener craftingListeners;
    public static bool enableBigCrafting = false;

    public static void EnableCraftingBench()
    {
        craftingListeners?.Invoke(true);
        // "I am responsible for the second inventory"
        InventoryLogic.responsible = InventoryLogic.Responsible.CRAFTING;
        enableBigCrafting = true;
        // external inventory is the inSlots of the big crafting bench.
        InventoryLogic.externalInventory.SetSlots(bigCrafting.GetComponent<SlotList>().slots);
    }

    public static void DisableCraftingBench()
    {
        craftingListeners?.Invoke(false);
        enableBigCrafting = false;
    }
}
