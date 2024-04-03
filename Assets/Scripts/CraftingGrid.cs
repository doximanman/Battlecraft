using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class CraftingGrid : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> inSlots;
    [SerializeField] private InventorySlot outSlot;

    [SerializeField] private List<CraftingRecipe> recipes;

    // Start is called before the first frame update
    void Start()
    {
        foreach (InventorySlot slot in inSlots)
        {
            slot.slotChangeListeners += (newStack) => { checkRecipes(); };
        }
        oldStack = outSlot.GetStack();
    }

    public void checkRecipes()
    {
        foreach (var recipe in recipes)
        {
            if (recipe.CanCraft(inSlots.Select(slot => slot.GetStack() == null ? null : new StackData(slot.GetStack()))))
            {
                outSlot.SetItem(recipe.outItem);
            }
            else outSlot.RemoveItem();
        }
    }

    private ItemStack oldStack;
    // Update is called once per frame
    void Update()
    {
        if (!StackData.AreEqual(oldStack, outSlot.GetStack()))
        {
            Debug.Log("Stack changed from " + oldStack + " to "+outSlot.GetStack());
            oldStack = outSlot.GetStack();
        }
    }
}
