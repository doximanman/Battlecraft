using Codice.CM.Client.Differences.Merge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="Scriptable object/Item")]
public class ItemType : ScriptableObject
{
    private static int maxId=0;
    private readonly int id;

    public Sprite icon;
    public int maxStack = 99;

    ItemType()
    {
        maxId++;
        id = maxId;
    }

    public override bool Equals(object other)
    {
        if(other == null) return false;
        if(other.GetType() != GetType()) return false;

        var newOther=other as ItemType;

        return newOther.icon == icon && newOther.maxStack == maxStack && newOther.name == name;
    }

    public override int GetHashCode()
    {
        return id;
    }

}
