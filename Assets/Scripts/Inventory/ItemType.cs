using Codice.CM.Client.Differences.Merge;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

[Serializable]
public class Stats
{
    public float range;
    public float damage;
}

[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class ItemType : ScriptableObject
{
    public Sprite icon;
    public int maxStack = 99;
    public bool swingable = false;
    public Stats stats;

    public InventoryData invData;

    public override string ToString()
    {
        if (invData == null)
        {
            return "name: " + name + " maxStack: " + maxStack;
        }
        return "name: "+name+" icon. maxStack: " + maxStack + " invData: "+invData;
    }

    // can compare null with an object and object with null
    public static bool AreEqual(object obj1, object obj2)
    {
        if (obj1 == null && obj2 == null) return true;
        if (obj1 == null) return obj2.Equals(obj1);
        if (obj2 == null) return obj1.Equals(obj2);
        return obj1.Equals(obj2);
    }

    public override bool Equals(object other)
    {
        // 'as' returns null if the types aren't equal
        var newOther = other as ItemType;

        if (newOther == null) return icon == null;

        return newOther.icon == icon && newOther.maxStack == maxStack && newOther.name == name && invData.Equals(newOther.invData);
    }

    public override int GetHashCode()
    {

        if (icon == null) return maxStack;

        if (invData == null) return icon.GetHashCode() ^ maxStack;

        else return icon.GetHashCode() ^ maxStack ^ invData.GetHashCode();
    }

}


[CustomEditor(typeof(ItemType))]
public class ItemTypeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var item = target as ItemType;

        EditorGUI.BeginChangeCheck();

        item.icon = EditorGUILayout.ObjectField("Icon",item.icon, typeof(Sprite), false) as Sprite;
        item.maxStack = EditorGUILayout.IntField("Max Stack",item.maxStack);
        item.swingable = EditorGUILayout.Toggle("Is Swingable (as weapon)",item.swingable);

        if (item.swingable)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stats"),new GUIContent("Weapon Stats"));
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
}