using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chest : MonoBehaviour, ICloseInventoryListener, IPointerDownHandler
{
    private Transform player;
    private InventoryInteract inventoryInteract;

    [SerializeField] private InventoryData chestItems;

    public float openRange = 0.5f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        inventoryInteract = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryInteract>();
    }

    public void OnPointerDown(PointerEventData eventData)
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
