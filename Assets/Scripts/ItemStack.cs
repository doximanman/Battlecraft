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
        get { return type; }
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
            var text = transform.GetChild(0);
            if (itemCount == 1)
            {
                text.gameObject.SetActive(false);
            }
            else
            {
                text.gameObject.SetActive(true);
                text.GetComponent<TMP_Text>().text=itemCount.ToString();
            }
        }
    }

    [HideInInspector] public Transform oldParent;

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

    public void Update()
    {
        Debug.Log(name + " :" + itemCount);
    }
}
