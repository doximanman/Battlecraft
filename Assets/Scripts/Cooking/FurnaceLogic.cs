using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FurnaceLogic : MonoBehaviour
{
    private static Furnace furnace;

    [InspectorName("Furnace")]
    [SerializeField] private Furnace _furnace;

    private static bool enableFurnace = false;

    private void Start()
    {
        furnace = _furnace;

        furnace.gameObject.SetActive(false);

        InventoryLogic.invListeners += (on) =>
        {
            // it is my responsibility to handle the second inventory
            if (InventoryLogic.responsible != InventoryLogic.Responsible.FURNACE){
                if (enableFurnace)
                    DisableFurnace();
            }
            if (on && enableFurnace)
            {
                furnace.gameObject.SetActive(true);
            }
            if(!on && onlyOnce)
            {
                DisableFurnace();
            }
        };
    }

    private static bool onlyOnce;
    // onlyOnce disables the furnace after opening once
    // saves the caller from having to add an invListener
    public static void EnableFurnace(bool onlyOnce = true)
    {
        enableFurnace = true;
        InventoryLogic.responsible = InventoryLogic.Responsible.FURNACE;
        FurnaceLogic.onlyOnce = onlyOnce;
    }

    public static void DisableFurnace()
    {
        // disable furnace from being enabled next time the listener runs
        // and change the inventory responsibility to the default one
        enableFurnace = false;
        onlyOnce = false;
        furnace.gameObject.SetActive(false);
        InventoryLogic.responsible = InventoryLogic.defaultResponsible;
    }
}
