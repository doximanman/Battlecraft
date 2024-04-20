using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Assertions;


[CreateAssetMenu(menuName = "Scriptable object/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public Matrix<StackData> inItems;
    public StackData outItem;


    public bool CanCraft(IEnumerable<IEnumerable<StackData>> items)
    {
        if (!inItems.ContainedBy(items))
            return false;



        for(int i = 0; i < inItems.Count(); i++)
        {
            for (int j = 0; j < inItems[i].Count; j++)
            {
                StackData inItem = inItems[i][j], item = items.ElementAt(i).ElementAt(j);

                // if both are null, they are equal
                // if only one is null, use Equals
                if (inItem == null)
                {
                    if (item == null) continue;
                    if (item.Equals(inItem)) continue;
                    else return false;
                }
                if (item == null)
                {
                    if (inItem.Equals(item)) continue;
                    else return false;
                }

                // same type and enough items in the slot
                bool hasItems = ItemType.AreEqual(inItem.type, item.type) && inItem.count <= item.count;
                if (!hasItems) return false;
            }
            // rest must be null
            for(int j = inItems[i].Count; j < items.ElementAt(i).Count(); j++)
            {
                // stackdata so either null or equal to null
                if (!(items.ElementAt(i).ElementAt(j) == null || items.ElementAt(i).ElementAt(j).Equals(null)))
                    return false;
            }
        }
        // rest must be null
        for(int i = inItems.Count(); i < items.Count(); i++)
        {
            // its a list so either null or list of nulls
            if (items.ElementAt(i) != null)
            {
                // not null - list of nulls.
                for (int j = 0; j < items.ElementAt(i).Count(); j++)
                    // stackdata so either null or equal to null
                    if (!(items.ElementAt(i).ElementAt(j) == null || !items.ElementAt(i).ElementAt(j).Equals(null)))
                        return false;
            }
        }
        return true;
    }

    public int HowManyCraft(IEnumerable<IEnumerable<StackData>> items)
    {
        int result = 0;
        while (CanCraft(items))
        {
            for(int i = 0; i < inItems.Count(); i++)
            {
                for (int j = 0; j < inItems[i].Count(); j++)
                {
                    var item = items.ElementAt(i).ElementAt(j);
                    var recipeItem = inItems[i][j];
                    if (item == null || item.Equals(null)) continue;
                    else
                    {
                        item.count -= recipeItem.count;
                        result++;
                    }
                }
            }
        }
        // cant craft more than however many would give a max stack
        return Mathf.Min(result,outItem.type.maxStack/outItem.count);
    }
    public override bool Equals(object other)
    {
        CraftingRecipe otherRecipe = other as CraftingRecipe;
        if (otherRecipe == null) return this;
        if(inItems==null) return otherRecipe.inItems == null;

        return inItems.Equals(otherRecipe.inItems);
    }

    public override int GetHashCode()
    {
        int sum=0;
        for(int i = 0; i < inItems.Count(); i++)
        {
            sum += inItems[i] == null ? 0 : inItems[i].GetHashCode();
        }

        return sum ^ outItem.GetHashCode();
    }
}
