using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingBench : Interactable
{
    
    [SerializeField] private ItemType benchType;

    public override void OnFinishChopping()
    {

        ItemType bench = Instantiate(benchType);
        bench.name = benchType.name;
        // mouse held down for holdDownTime seconds
        DroppedStacksManager.instance.Drop(new StackData(bench, 1), transform.position);

        Destroy(gameObject);
    }

    public override void OnInteract()
    {

        CraftingLogic.EnableCraftingBench();
        InventoryLogic.OpenInventory();

        InventoryLogic.invListeners += OnInventory;
    }

    public void OnInventory(bool open)
    {
        if (!open)
        {
            // on close inventory
            var grid = CraftingLogic.bigCrafting.GetComponent<CraftingGrid>();
            // move from grid to inventory
            Inventory.MoveInventory(grid.inSlots.Flatten(), InventoryLogic.personalInventory);
            // remove this listener
            InventoryLogic.invListeners -= OnInventory;
        }
    }
}
