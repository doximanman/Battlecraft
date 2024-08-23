using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setup : MonoBehaviour
{
    [SerializeField] private GameObject settingsTab;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject bigCrafting;
    [SerializeField] private GameObject crafting;
    [SerializeField] private GameObject mainInventory;
    [SerializeField] private GameObject chestInventory;

    void Start()
    {
        //MenuLogic.settingsTab.gameObject.SetActive(false);
        settingsTab.SetActive(false);

        MetaLogic.UndarkenBackground();
        //MetaLogic.pauseMenu.SetActive(false);
        pauseMenu.SetActive(false);

        //CraftingLogic.bigCrafting.SetActive(false);
        //CraftingLogic.crafting.SetActive(false);
        bigCrafting.SetActive(false);
        crafting.SetActive(false);

        //InventoryLogic.mainInventory.SetActive(false);
        //InventoryLogic.chestInventory.SetActive(false);
        mainInventory.SetActive(false);
        chestInventory.SetActive(false);
        InventoryLogic.DisableSecondInventory();


    }
}
