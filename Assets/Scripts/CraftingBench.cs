using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingBench : MonoBehaviour,ICloseInventoryListener, IPointerDownHandler, IPointerUpHandler
{
    private Transform player;
    private InventoryInteract inventoryInteract;
    [SerializeField] private ItemType benchType;

    public float openRange = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        inventoryInteract = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryInteract>();
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
            Inventory playerInventory = inventoryInteract.mainInventory;
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
    public void OnPointerDown(PointerEventData eventData) {
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
            // open bench
            MetaLogic.EnableCraftingBench();
            MetaLogic.OpenInventory();

            MetaLogic.RegisterCloseInvListener(this);
        }
    }

    public void OnCloseInventory()
    {
        MetaLogic.DisableCraftingBench();
        var grid = MetaLogic.bigCrafting.GetComponent<CraftingGrid>();
        Inventory.MoveInventory(grid.inSlots.Flatten(), inventoryInteract.mainInventory);
    }
}
