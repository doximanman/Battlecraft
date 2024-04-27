using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable object/Crafting Recipes")]
public class CraftingRecipes : ScriptableObject
{
    public List<CraftingRecipe> craftingRecipes;

    public IEnumerator<CraftingRecipe> GetEnumerator() => craftingRecipes.GetEnumerator();

}
