using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class StackData
{
    public ItemType type;
    public int count;

    public StackData(ItemStack stack)
    {
        Assert.IsNotNull(stack);

        type = stack.Type;
        count = stack.ItemCount;
    }

    // unity's serializable object
    // initializes everything as type=null,count=0,
    // instead of just null
    public bool IsDefined()
    {
        return type!=null && count > 0;
    }

    public override bool Equals(object obj)
    {
        var other = obj as StackData;
        if (other == null) return false;

        return type==other.type && count== other.count;
    }

    public override int GetHashCode()
    {
        return type.GetHashCode() ^ count;
    }
}
