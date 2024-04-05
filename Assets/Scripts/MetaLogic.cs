using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class MetaLogic : MonoBehaviour
{
    public static Inventory externalInventory;
    public static Inventory personalInventory;
    public static GameObject mainInventory;
    public static GameObject darkenBackground;
    public static GameObject pauseMenu;
    public static GameObject darkenHotbar;
    public static GameObject crafting;

    public static double doubleClickDelay = 0.2f;

    public static bool paused=false;

    public static bool inventoryIsOpen=false;

    public static bool pauseMenuEnabled = false;

    private void Start()
    {
        personalInventory=GameObject.FindGameObjectWithTag("PersonalInventory").GetComponent<Inventory>();
        externalInventory = GameObject.FindGameObjectWithTag("ExternalInventory").GetComponent<Inventory>();
        mainInventory = GameObject.FindGameObjectWithTag("MainInventory");
        darkenBackground = GameObject.FindGameObjectWithTag("DarkBackground");
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        darkenHotbar = GameObject.FindGameObjectWithTag("DarkHotbar");
        crafting = GameObject.FindGameObjectWithTag("CraftingGrid");
        UndarkenBackground();
        pauseMenu.SetActive(false);
        CloseInventory();
        DisableSecondInventory();
    }

    public static void EnableHotbar()
    {
        darkenHotbar.GetComponent<Image>().enabled = false;
    }

    public static void DisableHotbar()
    {
        darkenHotbar.GetComponent<Image>().enabled = true;
    }

    public static void DarkenBackground()
    {
        darkenBackground.GetComponent<Image>().enabled = true;
    }

    public static void UndarkenBackground()
    {
        darkenBackground.GetComponent<Image>().enabled = false;
    }

    public static void Pause()
    {
        Time.timeScale = 0;
        Camera.main.GetComponent<PostProcessVolume>().enabled = true;
        paused = true;
        ItemStack.CancelDrag();
    }

    public static void Unpause()
    {
        Time.timeScale = 1;
        Camera.main.GetComponent<PostProcessVolume>().enabled = false;
        paused = false;
        ItemStack.CancelDrag();
    }

    static List<IPauseMenuListener> pauseMenuListeners = new();

    public static void OpenPauseMenu()
    {
        foreach(var listener in pauseMenuListeners)
        {
            listener.OnPauseMenu();
        }

        if(inventoryIsOpen) CloseInventory();

        Pause();
        pauseMenu.SetActive(true);
        DarkenBackground();
        DisableHotbar();
        pauseMenuEnabled = true;
        pauseMenuListeners.Clear();
    }

    public static void RegisterPauseMenuListener(IPauseMenuListener listener)
    {
        pauseMenuListeners.Add(listener);
    }

    public static void ClosePauseMenu()
    {
        Unpause();
        pauseMenu.SetActive(false);
        UndarkenBackground();
        EnableHotbar();
        pauseMenuEnabled = false;
    }

    public static void QuitGame()
    {
        Application.Quit();
    }

    public static void EnableSecondInventory()
    {
        externalInventory.gameObject.SetActive(true);
    }

    public static void DisableSecondInventory()
    {

        externalInventory.gameObject.SetActive(false);
    }

    public static bool IsSecondInventoryEnabled()
    {
        return externalInventory.gameObject.activeSelf;
    }

    static List<IOpenInventoryListener> openInvListeners = new();

    public static void OpenInventory()
    {
        foreach (var listener in openInvListeners)
        {
            listener.OnOpenInventory();
        }
        Pause();
        mainInventory.SetActive(true);
        // enable crafting grid only if the second inventory is closed
        if (!IsSecondInventoryEnabled())
            crafting.SetActive(true);
        DarkenBackground();
        inventoryIsOpen = true;
        openInvListeners.Clear();
    }

    public static void RegisterOpenInvListener(IOpenInventoryListener listener)
    {
        openInvListeners.Add(listener);
    }

    static List<ICloseInventoryListener> closeInvListeners = new();

    public static void CloseInventory()
    {
        foreach(var listener in closeInvListeners)
        {
            listener.OnCloseInventory();
        }

        Unpause();
        mainInventory.SetActive(false);
        crafting.SetActive(false);
        UndarkenBackground();
        inventoryIsOpen = false;
        closeInvListeners.Clear();
    }

    // does nothing when inventory is closed
    public static void RegisterCloseInvListener(ICloseInventoryListener listener)
    {
        if(inventoryIsOpen)
            closeInvListeners.Add(listener);
    }

    public static void RemoveCloseInvListener(ICloseInventoryListener listener)
    {
        closeInvListeners.Remove(listener);
    }

}
