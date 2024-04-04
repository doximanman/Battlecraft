using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class CraftingGrid : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> inSlots;
    [SerializeField] private InventorySlot outSlot;

    [SerializeField] private List<CraftingRecipe> recipes;

    // Start is called before the first frame update
    void Start()
    {
        CraftingRecipe currentRecipe = null;

        foreach (InventorySlot slot in inSlots)
        {
            slot.slotChangeListeners += (newStack) =>
            {
                currentRecipe = CheckRecipes();
                ApplyRecipe(currentRecipe);
            };
        }
        outSlot.slotChangeListeners += (newStack) =>
        {
            if (newStack != null || currentRecipe == null) return;
            Craft(currentRecipe);
            currentRecipe = CheckRecipes();
            ApplyRecipe(currentRecipe);
        };
    }

    public void ApplyRecipe(CraftingRecipe recipe)
    {
        if (recipe == null)
            outSlot.RemoveItem();
        else
            outSlot.SetItem(recipe.outItem);
    }

    // logical only
    public bool CheckRecipe(CraftingRecipe recipe)
    {
        // convert slot list into stackdata list
        var inData = inSlots.Select(slot => slot.GetStack() == null ? null : new StackData(slot.GetStack()));

        // check if the recipe can be crafted
        return recipe.CanCraft(inData);
    }

    // logical only
    public CraftingRecipe CheckRecipes()
    {
        foreach (var recipe in recipes)
        {
            if (CheckRecipe(recipe))
                return recipe;
        }
        return null;
    }


    public void Craft(CraftingRecipe recipe)
    {
        for (int i = 0; i < inSlots.Count; i++)
        {
            if (inSlots[i].GetStack() != null)
            {
                inSlots[i].RemoveSome(recipe.inItems[i].count);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
