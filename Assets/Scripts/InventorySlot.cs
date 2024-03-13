using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        // the dragged item
        var item = eventData.pointerDrag.GetComponent<InventoryItem>();

        if(transform.childCount > 0)
        {
            // move current item into old slot
            var thisItem = transform.GetChild(0);
            thisItem.SetParent(item.oldParent);
        }
        item.oldParent=transform;
    }
}
