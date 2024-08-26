using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChestBlock : Interactable
{
    private InventoryInteract inventoryInteract;

    [SerializeField] private InventoryData chestItems;
    [SerializeField] private ItemType chestType;

    public override void Start()
    {
        base.Start();
        inventoryInteract = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryInteract>();
    }

    public override void OnInteract()
    {
        // open chest
        InventoryLogic.EnableChestInventory();
        inventoryInteract.LoadSecondInventory(chestItems);
        InventoryLogic.OpenInventory();

        InventoryLogic.invListeners += OnInventory;
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

    public override void OnFinishChopping()
    {
        // drop all items
        DroppedStacksManager.instance.Drop(chestItems.GetItems(), transform.position);

        ItemType chest = Instantiate(chestType);
        chest.name = chestType.name;
        // drop the chest
        DroppedStacksManager.instance.Drop(new StackData(chest, 1), transform.position);

        // remove chest block from the scene.
        Destroy(gameObject);
    }

    public override string SerializeData()
    {
        return JsonUtility.ToJson(chestItems);
    }

    public override void DeserializeData(string data)
    {
        if (data != "")
            chestItems = JsonUtility.FromJson<InventoryData>(data);
    }
}
