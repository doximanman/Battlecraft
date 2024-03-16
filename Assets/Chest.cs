using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, ICloseInventoryListener
{
    public Transform player;
    public InventoryInteract inventoryInteract;

    [SerializeField] private InventoryData chestItems;

    public float openRange = 0.5f;

    private void OnMouseDown()
    {
        // do nothing if game is paused
        if (MetaLogic.paused) return;

        if(Vector3.Distance(transform.position, player.position) < openRange)
        {
            // open chest
            inventoryInteract.LoadSecondInventory(chestItems);
            MetaLogic.EnableSecondInventory();
            MetaLogic.OpenInventory();

            MetaLogic.RegisterCloseInvListener(this);
        }
    }
    
    public void OnCloseInventory()
    {
        inventoryInteract.UnloadSecondInventory(chestItems);
        MetaLogic.DisableSecondInventory();
    }
    
}
