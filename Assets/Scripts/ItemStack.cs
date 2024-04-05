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
    [SerializeField] private GameObject stackPrefab;

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
    [HideInInspector] public static InventorySlot originalSlot;

    public static ItemStack beingDragged;
    public void OnBeginDrag(PointerEventData eventData)
    {
        // disable dragging during pause menu
        if (MetaLogic.pauseMenuEnabled) return;



        // left mouse button
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // make item invisible to raycast
            // so that the OnDrag method of the slot wiil be activated
            // instead of this
            GetComponent<Image>().raycastTarget = false;

            // make stack son of root so it appears on top
            // and remove it from the slot
            originalSlot = transform.parent.GetComponent<InventorySlot>();
            transform.SetParent(transform.root);
            originalSlot.DetatchChild();

            beingDragged = this;
            stopDrag = false;
        }
        else
        {

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (stopDrag) eventData.pointerDrag = null;


        else beingDragged.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (stopDrag) return;

        beingDragged.EndDrag();
    }

    private void EndDrag()
    {

        GetComponent<Image>().raycastTarget = true;
        if (transform.parent == transform.root)
                transform.SetParent(originalSlot.transform);
    }
}
