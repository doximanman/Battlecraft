using Codice.Client.Commands;
using System;
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
            if (type != value)
            {
                type = value;
                GetComponent<Image>().sprite = value.icon;
                name = type.name;
                ItemCount = 1;
            }
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
    public static bool rightClick = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        // disable dragging during pause menu
        if (MetaLogic.pauseMenuEnabled) return;

        // cancel drag on pause
        MetaLogic.pauseListeners += (_) => CancelDrag();

        // left mouse button or there is only one item in the slot
        if (eventData.button == PointerEventData.InputButton.Left || ItemCount == 1)
        {
            rightClick = false;
            // make item invisible to raycast
            // so that the OnDrag method of the slot wiil be activated
            // instead of this
            GetComponent<Image>().raycastTarget = false;

            beingDragged = this;
            stopDrag = false;

            // make stack son of root so it appears on top
            // and remove it from the slot
            originalSlot = transform.parent.GetComponent<InventorySlot>();
            transform.SetParent(transform.root);
            originalSlot.DetatchStack();

        }
        // right mouse button
        else
        {
            rightClick = true;
            originalSlot = transform.parent.GetComponent<InventorySlot>();
            // create new stack that is half of the current stack
            ItemStack newStack = Instantiate(Prefabs.stackPrefab, transform.parent).GetComponent<ItemStack>();
            var rectTran = newStack.GetComponent<RectTransform>();
            rectTran.sizeDelta = GetComponent<RectTransform>().sizeDelta;
            newStack.Type = Type;
            int originalCount = ItemCount;
            newStack.ItemCount = (originalCount + 1) / 2;
            newStack.GetComponent<Image>().raycastTarget = false;
            newStack.transform.SetParent(transform.root);

            beingDragged = newStack;
            stopDrag = false;
            originalSlot.RemoveSome(ItemCount-originalCount / 2);

            // do the same as left mouse button but for the new stack


        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (stopDrag) eventData.pointerDrag = null;


        else beingDragged.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (stopDrag && beingDragged != null)
        {
            originalSlot = null;
            beingDragged.GetComponent<Image>().raycastTarget = true;
            beingDragged = null;
            return;
        }
        if (beingDragged == null)
        {
            originalSlot = null;
            return;
        };

        beingDragged.EndDrag();
    }

    private void EndDrag()
    {
        GetComponent<Image>().raycastTarget = true;

        // OnDrop of a slot had already handled the required logic
        if (stopDrag)
        {
            originalSlot = null;
            beingDragged = null;
            return;
        };

        // not on a slot - throw the item.
        DroppedStacksManager.instance.Drop(new(beingDragged.Type, beingDragged.ItemCount));
        Destroy(beingDragged.gameObject);
        beingDragged = null;


        originalSlot = null;
    }
}
