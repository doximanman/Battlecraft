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

    [ContextMenu("Generate Loot")]
    /// <summary>
    /// generate loot according to the current loottable
    /// </summary>
    public void GenerateLoot()
    {
        chestItems.Clear();
        List<StackData> availableSlots = new(chestItems.items);
        foreach(var entry in lootTable.loot)
        {
            ItemType type = entry.type;
            int count = UnityEngine.Random.Range(entry.minCount, entry.maxCount+1);
            // minimum number of stacks to split the stack to
            int minStack = 1;
            int maxStack;
            // maximum number of stacks to split the stack to - depends on count
            if (count < 5)
                maxStack = 1;
            else if (count < 10)
                maxStack = 2;
            else if (count < 20)
                maxStack = 3;
            else
                maxStack = 4;
            // number of stacks to split to
            int stackCount = UnityEngine.Random.Range(minStack, maxStack+1);
            // choose the slots for the stacks
            StackData[] chosenSlots = availableSlots.RandomSubset(stackCount).ToArray();
            // determine the count of each sub-stack - 
            // first generate a number from 1 to (count + 1 - stackCount) (including).
            // (the reason for count + 1 - stackCount is that we want to have at least 1 for the remaining stacks)
            // then, update the stackcount to be one less, and update count to be count subtract how much was used.
            // then, apply this recursively until the last stack.
            // numbers example: assume count = 24, stackCount = 4.
            // then the first stack will have a count from 1 to 21 (count + 1 - stackCount), assume 15.
            // (notice: even if we choose 21, we have exactly enough for the remaining 3 stacks)
            // now, count is 9 and stackCount is 3.
            // then the second stack will have a count from 1 to 7.
            // etc...
            // we are left with a non-zero count, which will go to the last stack.
            int[] counts = new int[stackCount];
            for (int i = 0; i < chosenSlots.Length-1; i++)
            {
                int maxCount = count + 1 - stackCount;
                counts[i] = UnityEngine.Random.Range(1, maxCount + 1);
                count -= counts[i];
                stackCount--;
            }
            counts[^1] = count;
            for(int i = 0; i < chosenSlots.Length; i++)
            {
                chosenSlots[i].type = type;
                chosenSlots[i].count = counts[i];
            }
            // remove chosen slots from slot pool
            availableSlots.RemoveAll(stack => chosenSlots.Contains(stack));
        }
    }

    public override void Start()
    {
        base.Start();
        inventoryInteract = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryInteract>();

        if (generated) return;
        generated = true;
        GenerateLoot();
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
        bool generated = serializedData["generated"].Value<bool>();
        JObject serializedLootTable = serializedData["lootTable"] as JObject;
        this.generated = generated;
        lootTable = LootTable.Deserialize(serializedLootTable);
        // no reason to load inventory if it is generated from the loot table
        if (!generated)
            return;

        JObject serializedInventory = serializedData["inventory"] as JObject;
        chestItems = InventoryData.Deserialize(serializedInventory);
    }
}
