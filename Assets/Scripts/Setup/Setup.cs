using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    void Start()
    {
        MetaLogic.UndarkenBackground();
        MetaLogic.pauseMenu.SetActive(false);

        CraftingLogic.bigCrafting.SetActive(false);
        CraftingLogic.crafting.SetActive(false);

        InventoryLogic.mainInventory.SetActive(false);
        InventoryLogic.chestInventory.SetActive(false);
        InventoryLogic.DisableSecondInventory();
    }
}
