using Codice.CM.Client.Differences.Merge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName ="Scriptable object/Item")]
public class ItemType : ScriptableObject
{
    public Sprite icon;
    public int maxStack = 99;
    
}
