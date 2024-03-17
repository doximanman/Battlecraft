using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
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

    // Start is called before the first frame update
    void Start()
    {
        // deselected color same as normal slot color
        deselectedColor=slotPrefab.GetComponent<Image>().color;
        
        // start the game with the first slot selected
        SelectedSlot = slots[0];
    }
}
