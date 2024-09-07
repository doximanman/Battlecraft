using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum FilterType { BLACK_LIST, WHITE_LIST };

[Serializable]
public class SlotData
{
    public StackData stack;
    public bool canAcceptItems;
    public FilterType filterType;
    public List<ItemType> filter;

    /// <summary>
    /// empty stack, can accept items, no filter. <br/>
    /// (no filter = empty blacklist)
    /// </summary>
    public SlotData()
    {
        stack = null;
        canAcceptItems = true;
        filterType = FilterType.BLACK_LIST;
        filter = new();
    }

    /// <summary>
    /// deep copy constructor
    /// </summary>
    /// <param name="other">to copy from</param>
    public SlotData(SlotData other)
    {
        stack = new(other.stack);
        canAcceptItems = other.canAcceptItems;
        filterType = other.filterType;
        filter = new(other.filter);
    }

    /// <summary>
    /// create a new stack with the same slot properties
    /// but with a different stack
    /// </summary>
    /// <param name="stack">new stack</param>
    /// <param name="other">slot data to copy properties from</param>
    public SlotData(StackData stack,SlotData other)
    {
        this.stack = stack;
        canAcceptItems = other.canAcceptItems;
        filterType = other.filterType;
        filter = new(other.filter);
    }

    public SlotData(StackData stack, bool canAcceptItems, FilterType filterType, List<ItemType> filter)
    {
        this.stack = stack;
        this.canAcceptItems = canAcceptItems;
        this.filterType = filterType;
        this.filter = filter;
    }

    /// <summary>
    /// try to add stack <br/>
    /// doesn't add the stack on failure
    /// </summary>
    /// <param name="stack">stack to add</param>
    /// <returns>if addition was successful</returns>
    public bool Add(StackData stack)
    {
        // empty stack can always be added
        if (stack == null) return true;

        // if the slot is empty simply add the stack as is.
        if (this.stack == null)
        {
            this.stack = new(stack);
            return true;
        }
        
        // otherwise slot is not empty, can only 
        // add if the types are the same.
        if(this.stack.type == stack.type)
        {
            // can only add if the combined count of both stacks
            // is less than the maximum stack
            int combinedCounts = this.stack.count + stack.count;
            if (combinedCounts > stack.type.maxStack)
                return false;
            this.stack.count = combinedCounts;
            return true;
        }

        // both stacks aren't of the same type - can't add.
        return false;
    }

    public void Remove(int count)
    {
        if (stack == null) return;

        // subtract the count, not below 0
        stack.count = Mathf.Max(stack.count - count, 0);

        // nullify stack if count is 0
        if (stack.count == 0)
            stack.type = null;
    }

    public bool CanAccept(StackData stack)
    {
        // if the stack is null then it can be accepted.
        // any slot can be empty.
        if (stack == null) return true;

        // if slot's stack is null (meaning the slot is empty),
        // return whether the stack passes the filter.
        if(this.stack == null)
            if (filterType == FilterType.BLACK_LIST)
                return !filter.Contains(stack.type);
            else
                return filter.Contains(stack.type);

        // otherwise the slot isn't empty, return
        // whether the types are the same and combining doesn't overflow the maxcount.
        if (this.stack.type != stack.type) return false;
        int combinedCount = this.stack.count + stack.count;
        if (combinedCount > stack.type.maxStack) return false;

        return true;

    }

    public bool CanAccept(ItemType type)
    {
        // type is null - slot can accept.
        if (type == null) return true;

        // slot is empty - filter check.
        if(stack == null)
            if(filterType == FilterType.BLACK_LIST)
                return !filter.Contains(type);
            else return filter.Contains(type);

        // slot is not empty - types match and
        // count is below max.
        return stack.type == type && stack.count < stack.type.maxStack;
    }

    public static JObject Serialize(SlotData slotData)
    {
        return new()
        {
            ["stack"] = StackData.Serialize(slotData.stack),
            ["canAcceptItems"] = slotData.canAcceptItems,
            ["filterType"] = Enum.GetName(typeof(FilterType), slotData.filterType),
            ["filter"] = JArray.FromObject(slotData.filter.Select(type => type.name)),
        };
    }

    public static SlotData Deserialize(JObject serialized)
    {
        StackData slotStack = StackData.Deserialize(serialized["stack"] as JObject);
        bool slotCanAcceptItems = serialized["canAcceptItems"].Value<bool>();
        FilterType slotFilterType = (FilterType) Enum.Parse(typeof(FilterType), serialized["filterType"].ToString());
        string[] typeNames = serialized["filter"].ToObject<string[]>();
        List<ItemType> slotFilter = typeNames.Select(typeName => ItemTypes.GetByName(typeName)).ToList();
        return new()
        {
            stack = slotStack,
            canAcceptItems = slotCanAcceptItems,
            filterType = slotFilterType,
            filter = slotFilter,
        };
    }

    // can compare null with an object and object with null
    public static bool AreEqual(object obj1, object obj2)
    {
        if (obj1 == null && obj2 == null) return true;
        if (obj1 == null) return obj2.Equals(obj1);
        if (obj2 == null) return obj1.Equals(obj2);
        return obj1.Equals(obj2);
    }

    public static bool operator==(SlotData obj1,object obj2)
    {
        return AreEqual(obj1, obj2);
    }

    public static bool operator!=(SlotData obj1,object obj2)
    {
        return !AreEqual(obj1, obj2);
    }

    public override bool Equals(object obj)
    {
        SlotData other = obj as SlotData;
        if (other == null) return stack == null;

        return stack == other.stack
            && canAcceptItems == other.canAcceptItems
            && filterType == other.filterType
            && filter.SequenceEqual(other.filter);
    }

    public override int GetHashCode()
    {
        return stack.GetHashCode() ^ canAcceptItems.GetHashCode() ^ filterType.GetHashCode();
    }

    public override string ToString()
    {
        return "Stack: " + stack?.ToString() + " canAcceptItems: " + canAcceptItems + " filterType: " + filterType;
    }
}
