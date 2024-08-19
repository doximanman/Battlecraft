using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceBlock : MonoBehaviour
{
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
        InventoryLogic.OpenInventory();
    }
}
