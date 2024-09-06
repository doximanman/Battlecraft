using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using MoreLinq;
using UnityEngine.EventSystems;
using Newtonsoft.Json.Linq;

public class ChestBlock : Interactable
{
    private InventoryInteract inventoryInteract;

    [SerializeField] private InventoryData chestItems;
    [SerializeField] private ItemType chestType;
    
    // used to generate loot only once
    [SerializeField] private bool generated;
    [Tooltip("Ignored if \"generated\" is true")]
    [SerializeField] private LootTable lootTable;

    private void Awake()
    {
        if (generated) return;
        generated = true;
        GenerateLoot();
    }

    /// <summary>
    /// generate loot according to the current loottable
    /// </summary>
    public void GenerateLoot()
    {
        chestItems.Clear();
        foreach(var entry in lootTable.loot)
        {
            ItemType type = entry.type;
            int count = UnityEngine.Random.Range(entry.minCount, entry.maxCount+1);
        }
    }

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

    [Serializable]
    private class ChestData
    {
        public string serializedInventory;
        public bool generated;
        public string serializedLootTable;
    }

    public override JObject SaveInternal()
    {
        return new()
        {
            ["inventory"] = InventoryData.Serialize(chestItems),
            ["generated"] = generated,
            ["lootTable"] = LootTable.Serialize(lootTable)
        };
    }

    public override void LoadInternal(JObject serializedData)
    {
        JObject serializedInventory = serializedData["inventory"] as JObject;
        bool generated = serializedData["generated"].Value<bool>();
        JObject serializedLootTable = serializedData["lootTable"] as JObject;

        chestItems = InventoryData.Deserialize(serializedInventory);
        this.generated = generated;
        lootTable = LootTable.Deserialize(serializedLootTable);
    }
}
