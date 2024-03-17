using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MetaLogic : MonoBehaviour
{
    public static GameObject externalInventory;
    public static GameObject mainInventory;
    public static GameObject darkenBackground;
    public static GameObject pauseMenu;

    public static bool paused=false;

    public static bool inventoryIsOpen=false;

    public static bool pauseMenuEnabled = false;

    private void Start()
    {
        externalInventory = GameObject.FindGameObjectWithTag("ExternalInventory");
        mainInventory = GameObject.FindGameObjectWithTag("MainInventory");
        darkenBackground = GameObject.FindGameObjectWithTag("DarkBackground");
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        darkenBackground.SetActive(false);
        pauseMenu.SetActive(false);
        CloseInventory();
        DisableSecondInventory();
    }

    public static void Pause()
    {
        Time.timeScale = 0;
        Camera.main.GetComponent<PostProcessVolume>().enabled = true;
        paused = true;
    }

    public static void Unpause()
    {
        Time.timeScale = 1;
        Camera.main.GetComponent<PostProcessVolume>().enabled = false;
        paused = false;
    }

    public static void OpenPauseMenu()
    {
        if(inventoryIsOpen) CloseInventory();

        Pause();
        pauseMenu.SetActive(true);
        darkenBackground.SetActive(true);
        pauseMenuEnabled = true;
    }

    public static void ClosePauseMenu()
    {
        Unpause();
        pauseMenu.SetActive(false);
        darkenBackground.SetActive(false);
        pauseMenuEnabled = false;
    }

    public static void QuitGame()
    {
        Application.Quit();
    }

    public static void EnableSecondInventory()
    {
        externalInventory.SetActive(true);
    }

    public static void DisableSecondInventory()
    {

        externalInventory.SetActive(false);
    }

    public static void OpenInventory()
    {
        Pause();
        mainInventory.SetActive(true);
        darkenBackground.SetActive(true);
        inventoryIsOpen = true;
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
        darkenBackground.SetActive(false);
        inventoryIsOpen = false;
        closeInvListeners.Clear();
    }


    public static void RegisterCloseInvListener(ICloseInventoryListener listener)
    {
        closeInvListeners.Add(listener);
    }

    public static void RemoveCloseInvListener(ICloseInventoryListener listener)
    {
        closeInvListeners.Remove(listener);
    }

}
