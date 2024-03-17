using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chest : MonoBehaviour, ICloseInventoryListener, IPointerDownHandler, IPointerUpHandler
{
    private Transform player;
    private InventoryInteract inventoryInteract;

    [SerializeField] private InventoryData chestItems;
    [SerializeField] private ItemType chestType;

    public float openRange = 0.5f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        inventoryInteract = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryInteract>();
    }

    public void SetItems(InventoryData chestItems)
    {
        this.chestItems = chestItems;
    }

    public InventoryData GetItems(InventoryData chestItems)
    {
        return chestItems;
    }

    [SerializeField] private float holdDownTime = 0.2f;
    private float holdDownTimer = 0;
    private void Update()
    {
        if (mouseDown) holdDownTimer += Time.deltaTime;
        else holdDownTimer = 0;

        if(holdDownTimer >= holdDownTime)
        {
            ItemType chest = Instantiate(chestType);
            chest.name = "Chest";
            chest.invData = chestItems;
            // mouse held down for holdDownTime seconds
            Inventory playerInventory = inventoryInteract.mainInventory;
            var success=playerInventory.AddItem(chest);
            if (success == null)
            {
                // inventory full, chest cant be added
            }
            else
            {
                // remove chest from the scene, it is now inside of the player's inventory.
                Destroy(gameObject);
            }
        }


    }


    private bool mouseDown = false;
    // must be implemented for OnPointerUp to activate
    public void OnPointerDown(PointerEventData eventData)
    {
        mouseDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mouseDown = false;
        // make sure current still on the chest
        if (eventData.pointerCurrentRaycast.gameObject != gameObject) return;

        // do nothing if game is paused
        if (MetaLogic.paused) return;

        if (Vector3.Distance(transform.position, player.position) < openRange)
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
