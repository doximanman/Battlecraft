using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chest : MonoBehaviour
{
    private Transform player;
    private InventoryInteract inventoryInteract;

    [SerializeField] private InventoryData chestItems;
    [SerializeField] private ItemType chestType;

    public float openRange;

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
            if (Vector2.Distance(transform.position, player.position) > openRange)
            {
                holdDownTimer = 0;
                return;
            }
            // mouse held down for holdDownTime seconds

            // drop all items
            DroppedStacksManager.instance.Drop(chestItems.GetItems(),transform.position);

            ItemType chest = Instantiate(chestType);
            chest.name = chestType.name;
            // drop the chest
            DroppedStacksManager.instance.Drop(new StackData(chest, 1), transform.position);

            // remove chest block from the scene.
            Destroy(gameObject);
        }


    }


    private bool mouseDown = false;
    // must be implemented for OnPointerUp to activate
    public void OnMouseDown()
    {
        mouseDown = true;
    }

    public void OnMouseUp()
    {
        // make sure current still on the chest
        if (!mouseDown) return;
        mouseDown = false;

        // do nothing if game is paused
        if (MetaLogic.paused) return;

        if (Vector3.Distance(transform.position, player.position) < openRange)
        {
            // open chest
            InventoryLogic.EnableChestInventory();
            inventoryInteract.LoadSecondInventory(chestItems);
            InventoryLogic.OpenInventory();

            InventoryLogic.invListeners += OnInventory;
        }
    }

    public void OnInventory(bool open)
    {
        if (!open)
        {
            // on close inventory - save second inventory into chest item data.
            inventoryInteract.UnloadSecondInventory(chestItems);
            InventoryLogic.DisableChestInventory();
            InventoryLogic.invListeners -= OnInventory;
        }
    }

}
