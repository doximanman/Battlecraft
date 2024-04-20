using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftingBench : MonoBehaviour,ICloseInventoryListener, IPointerDownHandler, IPointerUpHandler
{
    private Transform player;
    private InventoryInteract inventoryInteract;


    public float openRange = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        inventoryInteract=GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryInteract>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData) {}

    public void OnPointerUp(PointerEventData eventData)
    {

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
    }
}
