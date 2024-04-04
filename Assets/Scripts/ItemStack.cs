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
    [HideInInspector] public static Transform originalParent;

    public static ItemStack beingDragged;

    private bool rightClick = false;
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
            rightClick = false;
            originalParent = transform.parent;
            transform.SetParent(transform.root);
            beingDragged = this;
            stopDrag = false;
        }
        else
        {

            rightClick = true;
            ItemStack newStack = Instantiate(stackPrefab, transform.parent).GetComponent<ItemStack>();
            //newStack.transform.localScale=Vector3.one;

            newStack.Type = Type;
            int originalCount = ItemCount;
            ItemCount = originalCount / 2;
            newStack.ItemCount = (originalCount + 1) / 2;

            originalParent = transform.parent;
            newStack.transform.SetParent(transform.root);

            beingDragged = newStack;
            beingDragged.GetComponent<Image>().raycastTarget = false;
            stopDrag = false;
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

        EndDrag();
    }

    private void EndDrag()
    {
        beingDragged.GetComponent<Image>().raycastTarget = true;
        if (rightClick)
        {
            // get itemstack currently inside of parent and
            // add this stack to it.
            var parentSlot = originalParent.GetComponent<InventorySlot>();
            parentSlot.CombineFrom(beingDragged);
        }
        else
        {
            if (transform.parent == transform.root)
                transform.SetParent(originalParent);
        }

    }
}
