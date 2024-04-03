using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptable object/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public StackData[] inItems = new StackData[4];
    public StackData outItem;

    public override bool Equals(object other)
    {
        CraftingRecipe otherRecipe = other as CraftingRecipe;
        if(otherRecipe == null) return false;

        return inItems.SequenceEqual(otherRecipe.inItems);
    }

    public override int GetHashCode()
    {
        return (
            inItems[0].GetHashCode() +
            inItems[1].GetHashCode() +
            inItems[2].GetHashCode() +
            inItems[3].GetHashCode()
            ) ^outItem.GetHashCode();
    }
}
