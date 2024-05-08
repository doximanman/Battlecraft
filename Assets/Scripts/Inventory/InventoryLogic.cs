using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryLogic : MonoBehaviour
{

    /// <summary>
    /// defines who has the responsibility to deal with
    /// the second inventory. for example, if it is INVENTORY
    /// then this class will open a chest (because that is
    /// its job). If it is CRAFTING then the crafting logic class
    /// will open some crafting grid, and this class will not do 
    /// anything.
    /// </summary>
    public enum Responsible { INVENTORY, CRAFTING };
    public static Responsible responsible;

    public delegate void ToggleListener(bool on);

    public static GameObject chestInventory;
    public static GameObject mainInventory;
    public static Inventory personalInventory;
    public static Inventory externalInventory;
    public static Hotbar hotbar;
    public static GameObject darkenHotbar;

    [InspectorName("Chest Inventory Object")]
    [SerializeField] private GameObject _chestInventory;
    [InspectorName("Main Inventory Object")]
    [SerializeField] private GameObject _mainInventory;
    [InspectorName("Personal Inventory")]
    [SerializeField] private Inventory _personalInventory;
    [InspectorName("External Inventory")]
    [SerializeField] private Inventory _externalInventory;
    [InspectorName("Hotbar")]
    [SerializeField] private Hotbar _hotbar;
    [InspectorName("Darken Hotbar Object")]
    [SerializeField] private GameObject _darkenHotbar;

    private void Start()
    {
        personalInventory = _personalInventory;
        externalInventory = _externalInventory;
        hotbar = _hotbar;
        chestInventory = _chestInventory;
        mainInventory = _mainInventory;
        darkenHotbar = _darkenHotbar;

        responsible = Responsible.CRAFTING;

        KeyInput.instance.onInventory += (down,_,_) =>
        {
            if (down)
            {
                // when pressing the inventory button, open it.
                // only if the pause menu is not enabled.
                if (MetaLogic.pauseMenuEnabled) return;
                if (inventoryIsOpen) CloseInventory();
                else OpenInventory();
            }
        };

        MetaLogic.pauseMenuListeners += (on) =>
        {
            // when pause menu is opened, close inventory.
            // and disable hotbar
            if (on)
            {
                if (inventoryIsOpen) CloseInventory();
                DisableHotbar();
            }
            // when pause menu is closed, enable hotbar.
            else
            {
                EnableHotbar();
            }
        };
    }

    public static void EnableHotbar()
    {
        darkenHotbar.GetComponent<Image>().enabled = false;
    }

    public static void DisableHotbar()
    {
        darkenHotbar.GetComponent<Image>().enabled = true;
    }

    public static ToggleListener chestListeners;
    public static bool enableChestInventory = true;

    public static void EnableChestInventory()
    {
        chestListeners?.Invoke(true);
        responsible = Responsible.INVENTORY;
        enableChestInventory = true;
        externalInventory.SetSlots(chestInventory.GetComponent<SlotList>().slots);
    }

    public static void DisableChestInventory()
    {
        chestListeners?.Invoke(false);
        responsible = Responsible.CRAFTING;
        enableChestInventory = false;
        //externalInventory.SetSlots(crafting.GetComponent<SlotList>().slots);
    }

    public static ToggleListener secondInventoryListeners;
    public static bool secondInventoryEnabled = false;

    public static void EnableSecondInventory()
    {
        secondInventoryListeners?.Invoke(true);
        secondInventoryEnabled = true;
        externalInventory.gameObject.SetActive(true);
    }

    public static void DisableSecondInventory()
    {
        secondInventoryListeners?.Invoke(false);
        secondInventoryEnabled = false;
        externalInventory.gameObject.SetActive(false);
    }

    public static bool inventoryIsOpen = false;
    public static ToggleListener invListeners;

    public static void OpenInventory()
    {
        invListeners?.Invoke(true);

        MetaLogic.Pause();
        mainInventory.SetActive(true);
        if (responsible == Responsible.INVENTORY && enableChestInventory)
            chestInventory.SetActive(true);

        EnableSecondInventory();
        MetaLogic.DarkenBackground();
        inventoryIsOpen = true;
    }

    public static void CloseInventory()
    {
        invListeners?.Invoke(false);

        MetaLogic.Unpause();
        // move items from crafting grid to inventory
        // only necessary because the inventory crafting grid
        // doesn't have its own class like the crafting
        // bench
        //if (crafting.activeSelf)
        //Inventory.MoveInventory(crafting.GetComponent<CraftingGrid>().inSlots.Flatten(),personalInventory);
        mainInventory.SetActive(false);
        //crafting.SetActive(false);
        //bigCrafting.SetActive(false);
        chestInventory.SetActive(false);
        DisableSecondInventory();
        MetaLogic.UndarkenBackground();
        inventoryIsOpen = false;
        //closeInvListeners.Clear();
    }
}
