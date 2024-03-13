using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform oldParent;


    public void OnBeginDrag(PointerEventData eventData)
    {
        // make item invisible to raycast
        // so that the OnDrag method of the slot wiil be activated
        // instead of this
        GetComponent<Image>().raycastTarget = false;

        oldParent = transform.parent;
        transform.SetParent(transform.root);

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = true;
        transform.SetParent(oldParent);
    }


}
