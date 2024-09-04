using log4net.Core;
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

    /// <summary>
    /// empty stack.
    /// meaning null type and 0 count.
    /// </summary>
    public StackData()
    {
        type = null;
        count = 0;
    }

    /// <summary>
    /// copy constructor
    /// </summary>
    /// <param name="other">to copy from</param>
    public StackData(StackData other)
    {
        type = other.type;
        count = other.count;
    }

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

    // ItemType cannot be serialized.
    // Therefore, save the name of it instead when serializing,
    // and restore it by the global dictionary.
    #region Serialization
    [Serializable]
    private class SavableStackData
    {
        public string typeName;
        public int count;
    }

    public static string Serialize(StackData stack)
    {
        SavableStackData data;
        // a null stack is serialized the same as a stack with null type and 0 count.
        if (stack != null) data = new(){
                typeName = stack.type == null ? "None" : stack.type.name,
                count = stack.count
            };
        else data = new(){
                typeName = "None",
                count = 0
            };

        return JsonUtility.ToJson(data);
    }

    public static StackData Deserialize(string serialized)
    {
        SavableStackData data = JsonUtility.FromJson<SavableStackData>(serialized);
        StackData result = new()
        {
            type = ItemTypes.GetByName(data.typeName),
            count = data.count
        };
        return result;
    }
    #endregion

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

    public static bool operator==(StackData obj1, object obj2)
    {
        return AreEqual(obj1, obj2);
    }

    public static bool operator!=(StackData obj1, object obj2)
    {
        return !AreEqual(obj1, obj2);
    }

    public override int GetHashCode()
    {
        if (type == null) return count;

        return type.GetHashCode() ^ count;
    }
}
