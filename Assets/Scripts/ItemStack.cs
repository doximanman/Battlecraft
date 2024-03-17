using Codice.Client.Commands;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemStack : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{


    private ItemType type;
    public ItemType Type
    {
        get
        {
            return type;
        }
        set
        {

            type = value;
            GetComponent<Image>().sprite = value.icon;
            name = type.name;
            ItemCount = 1;
        }
    }

    private int itemCount;
    public int ItemCount
    {
        get { return itemCount; }
        set
        {
            itemCount = value;
            // update count on screen
            var text = transform.GetChild(0);
            if (itemCount == 1)
            {
                // don't show count if it is 1
                text.gameObject.SetActive(false);
            }
            else
            {
                text.gameObject.SetActive(true);
                text.GetComponent<TMP_Text>().text = itemCount.ToString();
            }
        }
    }

    public static void CancelDrag()
    {
        if (beingDragged != null) beingDragged.EndDrag();
        stopDrag = true;
        beingDragged = null;
    }

    public static bool stopDrag = false;
    [HideInInspector] public Transform originalParent;

    private static ItemStack beingDragged;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // disable dragging during pause menu
        if (MetaLogic.pauseMenuEnabled) return;

        // make item invisible to raycast
        // so that the OnDrag method of the slot wiil be activated
        // instead of this
        GetComponent<Image>().raycastTarget = false;
        originalParent = transform.parent;
        transform.SetParent(transform.root);
        beingDragged = this;
        stopDrag = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (stopDrag) eventData.pointerDrag = null;


        else transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (stopDrag) return;

        EndDrag();
    }

    private void EndDrag()
    {
        GetComponent<Image>().raycastTarget = true;
        if (transform.parent == transform.root)
            transform.SetParent(originalParent);
    }
}
