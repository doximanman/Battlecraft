using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Logic between inventories
public class InventoryInteract : MonoBehaviour
{
    public Inventory mainInventory;
    public Inventory secondInventory;



    private GraphicRaycaster raycaster;
    private EventSystem eventSystem;
    // Start is called before the first frame update
    void Start()
    {
        var inventory = GameObject.FindGameObjectWithTag("Inventory");
        raycaster= inventory.GetComponent<GraphicRaycaster>();
        eventSystem=GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
    }



    [SerializeField] private float doubleClickDelay = 0.5f;
    private float lastClicked;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.realtimeSinceStartup - lastClicked < doubleClickDelay)
            {
                // double click
                // raycast to check which slot was clicked
                //r = new Ray(Input.mousePosition + Vector3.back, Vector3.forward);
                var eventData = new PointerEventData(eventSystem)
                {
                    position = Input.mousePosition
                };
                List<RaycastResult> hit = new();
                raycaster.Raycast(eventData, hit);
                foreach(var collider in hit)
                {
                    if (collider.gameObject.GetComponent<InventorySlot>() != null)
                    {
                        var slot = collider.gameObject.GetComponent<InventorySlot>();
                        InventorySlotDoubleClick(slot);
                    }
                }
            }
            else
            {
                // single click
                lastClicked = Time.realtimeSinceStartup;
            }
        }
    }
    public void InventorySlotDoubleClick(InventorySlot slot)
    {
        slot.DoubleClick();

        if (!mainInventory.isActiveAndEnabled || !secondInventory.isActiveAndEnabled) return;

        Inventory origin = BelongsTo(slot);
        if (origin == null) return;
        Inventory notOrigin=origin==mainInventory ? secondInventory : mainInventory;
        origin.MoveItem(slot, notOrigin);
    }

    public Inventory BelongsTo(InventorySlot slot)
    {
        if (mainInventory.slots.Contains(slot)) return mainInventory; 
        if (secondInventory.slots.Contains(slot)) return secondInventory;
        return null;
    }
}
