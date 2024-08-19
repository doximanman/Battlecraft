using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceBlock : MonoBehaviour
{
    public bool initialized = false;
    /// <summary>
    /// state of the furnace: <br />
    /// (minFuel, maxFuel, currentFuel)
    /// </summary>
    public FurnaceState state;

    private bool mouseDown = false;
    private void OnMouseDown()
    {
        mouseDown = true;
    }

    private void OnMouseUp()
    {
        // if mouse wasnt pressed down on the block, ignore
        if (!mouseDown) return;
        mouseDown = false;

        // if paused ignore
        if (MetaLogic.paused) return;

        FurnaceLogic.EnableFurnace();
        FurnaceLogic.furnaceListeners += OnFurnace;
        InventoryLogic.OpenInventory();
    }

    public void OnFurnace(bool open)
    {
        if (open)
        {
            if (!initialized)
            {
                state = FurnaceLogic.defaultValues;
                initialized = true;
            }
            FurnaceLogic.LoadState(state);
        }
        else
        {
            state = FurnaceLogic.GetState();
            FurnaceLogic.DisableFurnace();
            FurnaceLogic.furnaceListeners -= OnFurnace;
        }
    }
}
