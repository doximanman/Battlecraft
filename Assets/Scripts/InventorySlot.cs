using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private GameObject stackPrefab;

    public ItemStack GetStack()
    {
        if (transform.childCount == 0) return null;
        return transform.GetChild(0).GetComponent<ItemStack>();
    }

    public void SetItem(ItemStack stack)
    {
        stack.transform.SetParent(transform);
    }

    public void SetItem(ItemType item)
    {
        Assert.IsTrue(transform.childCount == 0);

        GameObject newStack=Instantiate(stackPrefab,transform);
        newStack.GetComponent<ItemStack>().Type = item;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // the dragged item
        var item = eventData.pointerDrag.GetComponent<ItemStack>();

        if(transform.childCount > 0)
        {
            // move current item into old slot
            var thisItem = transform.GetChild(0);
            thisItem.SetParent(item.oldParent);
        }
        item.oldParent=transform;
    }
}
