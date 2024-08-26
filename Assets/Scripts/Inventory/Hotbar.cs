using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public static Hotbar instance;

    [SerializeField] private GameObject slotPrefab;

    public List<InventorySlot> slots;
    [SerializeField] private Color selectedColor;
    private Color deselectedColor;

    private InventorySlot selectedSlot;
    public InventorySlot SelectedSlot
    {
        get { return selectedSlot; }
        set
        {
            // deselect old slot
            if (selectedSlot != null)
                selectedSlot.GetComponent<Image>().color = deselectedColor;

            // select new slot
            selectedSlot = value;
            selectedSlot.GetComponent<Image>().color = selectedColor;
        }
    }

    private int selectedIndex;
    public int SelectedIndex { get { return selectedIndex; } set
        {
            Assert.IsTrue(0 <= value && value <= slots.Count);

            selectedIndex = value;
            SelectedSlot = slots[selectedIndex];
        } }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        // deselected color same as normal slot color
        deselectedColor=slotPrefab.GetComponent<Image>().color;
        
        // start the game with the first slot selected
        SelectedSlot = slots[0];

        KeyInput.instance.onNum += (num) => SelectedIndex = num;
    }

    public delegate void ItemNotifier(ItemStack stack);

    /// <summary>
    /// Get notified when an item is used.
    /// If the used item needs to change
    /// its count, update the given stack.
    /// </summary>
    public ItemNotifier OnItemUse;

    private void Update()
    {
        // if paused don't do anything
        if (MetaLogic.paused || InventoryLogic.inventoryIsOpen || MetaLogic.mouseDownOnBlock) return;
        
        // if selected slot has a stack and mouse is pressed
        if(selectedSlot != null
            && Input.GetMouseButtonDown(0)
            && selectedSlot.TryGetStack(out ItemStack stack))
        {
            OnItemUse?.Invoke(stack);
            /*if(stackData.count == 0)
            {
                selectedSlot.RemoveItem();
            }
            else if(stackData.count != stack.ItemCount)
            {
                stack.ItemCount = stackData.count;
            }*/
        }
    }
}
