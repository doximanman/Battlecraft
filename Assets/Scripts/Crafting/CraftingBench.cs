using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingBench : MonoBehaviour
{
    private Transform player;
    
    [SerializeField] private ItemType benchType;

    public float openRange = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    [SerializeField] private float holdDownTime = 0.2f;
    private float holdDownTimer = 0;
    private void Update()
    {
        if (mouseDown) holdDownTimer += Time.deltaTime;
        else holdDownTimer = 0;

        if (holdDownTimer >= holdDownTime)
        {
            ItemType bench = Instantiate(benchType);
            bench.name = "CraftingBench";
            // mouse held down for holdDownTime seconds
            Inventory playerInventory = InventoryLogic.personalInventory;
            var success = playerInventory.AddItem(bench);
            if (success == null)
            {
                // inventory full, bench cant be added
            }
            else
            {
                // remove bench from the scene, it is now inside of the player's inventory.
                Destroy(gameObject);
            }
        }


    }

    private bool mouseDown = false;
    public void OnMouseDown(){
        mouseDown = true;
    }

    public void OnMouseUp()
    {
        if (!mouseDown)
            return;
        mouseDown = false;

        // do nothing if game is paused
        if (MetaLogic.paused) return;

        if (Vector3.Distance(transform.position, player.position) < openRange)
        {
            // open bench
            CraftingLogic.EnableCraftingBench();
            InventoryLogic.OpenInventory();

            InventoryLogic.invListeners += OnInventory;
        }
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
