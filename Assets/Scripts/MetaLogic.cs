using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MetaLogic : MonoBehaviour
{
    public static GameObject externalInventory;

    public static bool paused=false;

    public static bool inventoryIsOpen=false;

    public static bool pauseMenu = false;

    private void Start()
    {
        externalInventory = GameObject.FindGameObjectWithTag("ExternalInventory");
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
        GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<Canvas>().enabled = true;
        pauseMenu = true;
    }

    public static void ClosePauseMenu()
    {
        Unpause();
        GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<Canvas>().enabled = false;
        pauseMenu = false;
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
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<Canvas>().enabled = true;
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
        GameObject.FindGameObjectWithTag("Inventory").GetComponent<Canvas>().enabled = false;
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
