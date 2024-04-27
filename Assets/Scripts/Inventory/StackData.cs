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

    public StackData(ItemType type, int count)
    {
        this.type = type;
        this.count = count;
    }

    public StackData(ItemStack stack)
    {
        //Assert.IsNotNull(stack);

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

    public override string ToString()
    {
        return "Type: " + type + ", count: " + count;
    }

    // can compare null with an object and object with null
    public static bool AreEqual(object obj1, object obj2)
    {
        if (obj1 == null && obj2 == null) return true;
        if (obj1 == null) return obj2.Equals(obj1);
        if (obj2 == null) return obj1.Equals(obj2);
        return obj1.Equals(obj2);
    }

    public override bool Equals(object obj)
    {
        if (obj is not StackData other) return type == null || count == 0;

        return type==other.type && count== other.count;
    }

    public override int GetHashCode()
    {
        if (type == null) return count;

        return type.GetHashCode() ^ count;
    }
}
