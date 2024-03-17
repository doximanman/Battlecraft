using Codice.CM.Client.Differences.Merge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class ItemType : ScriptableObject
{
    public Sprite icon;
    public int maxStack = 99;

    public InventoryData invData;

    public override bool Equals(object other)
    {
        // 'as' returns null if the types aren't equal
        var newOther = other as ItemType;

        if (newOther == null) return false;



        return newOther.icon == icon && newOther.maxStack == maxStack && newOther.name == name && invData.Equals(newOther.invData);
    }

    public override int GetHashCode()
    {

        if (icon == null) return maxStack;

        if (invData == null) return icon.GetHashCode() ^ maxStack;

        else return icon.GetHashCode() ^ maxStack ^ invData.GetHashCode();
    }

}
