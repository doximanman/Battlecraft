using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Assertions;


[CreateAssetMenu(menuName = "Scriptable object/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public StackData[] inItems = new StackData[4];
    public StackData outItem;


    public bool CanCraft(IEnumerable<StackData> items)
    {
        Assert.IsTrue(items.Count() == 4);

        for(int i = 0; i < 4; i++)
        {
            StackData inItem = inItems[i], item = items.ElementAt(i);
            
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
            bool hasItems = ItemType.AreEqual(inItem.type,item.type) && inItem.count <= item.count;
            if (!hasItems) return false;
        }
        return true;
    }

    public override bool Equals(object other)
    {
        CraftingRecipe otherRecipe = other as CraftingRecipe;
        if (otherRecipe == null) return false;

        return inItems.SequenceEqual(otherRecipe.inItems);
    }

    public override int GetHashCode()
    {
        int x, y, z, w;
        x = inItems[0] == null ? 0 : inItems[0].GetHashCode();
        y = inItems[1] == null ? 0 : inItems[1].GetHashCode();
        z = inItems[2] == null ? 0 : inItems[2].GetHashCode();
        w = inItems[3] == null ? 0 : inItems[3].GetHashCode();

        return (x + y + z + w) ^ outItem.GetHashCode();
    }
}
