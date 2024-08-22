using Codice.CM.Client.Differences.Merge;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using System.Text;

[Serializable]
public class WeaponStats
{
    public float range;
    public float damage;
    public float stun;

    public override string ToString()
    {
        return "Range: " + range + " Damage: " + damage + " Stun: " + stun;
    }
}

[Serializable]
public class FoodStats
{
    public float saturation;
    public float heal;

    public override string ToString()
    {
        return "Saturation: " + saturation + " Heal: "+heal;
    }
}

[Serializable]
public class FuelStats
{
    public float fuelAmount;

    public override string ToString()
    {
        return "Fuel: "+fuelAmount;
    }
}

[Serializable]
public class PlacableProps
{
    public float maxHeightDifference;
    public float maxPlaceDistance;
    public float placeYOffset;
    public GameObject blockPrefab;

    public PlacableProps(float maxHeightDifference, float maxPlaceDistance,float placeYOffset, GameObject blockPrefab)
    {
        this.maxHeightDifference = maxHeightDifference;
        this.maxPlaceDistance = maxPlaceDistance;
        this.placeYOffset = placeYOffset;
        this.blockPrefab = blockPrefab;
    }
}

[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class ItemType : ScriptableObject
{
    public Sprite icon;
    public int maxStack = 99;

    public bool swingable = false;
    public WeaponStats stats;

    public bool food = false;
    public FoodStats foodStats;

    public bool fuel = false;
    public FuelStats fuelStats;

    public bool cookable = false;
    public StackData cookResult;
    public float cookTime;

    public bool placable = false;
    public PlacableProps placableProps;

    public override string ToString()
    {
        StringBuilder builder = new();
        builder.Append("Name: " + name + " Max Stack: " + maxStack);
        if (swingable)
            builder.Append(" Swingable. Stats: " + stats);
        if (food)
            builder.Append(" Food Stats: " + foodStats);
        if (fuel)
            builder.Append(" Fuel: " + fuelStats);
        return builder.ToString();
    }

    // can compare null with an object and object with null
    public static bool AreEqual(object obj1, object obj2)
    {
        if (obj1 == null && obj2 == null) return true;
        if (obj1 == null) return obj2.Equals(obj1);
        if (obj2 == null) return obj1.Equals(obj2);
        return obj1.Equals(obj2);
    }

    public static bool operator==(ItemType obj1, object obj2)
    {
        return AreEqual(obj1, obj2);
    }

    public static bool operator!=(ItemType obj1, object obj2)
    {
        return !AreEqual(obj1, obj2);
    }

    public override bool Equals(object other)
    {
        // 'as' returns null if the types aren't equal
        var newOther = other as ItemType;

        if (newOther == null) return icon == null;

        return newOther.icon == icon && newOther.maxStack == maxStack && newOther.name == name;
    }

    public override int GetHashCode()
    {

        if (icon == null) return maxStack;

        else return icon.GetHashCode() ^ maxStack;
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
        item.swingable = EditorGUILayout.Toggle("Swingable",item.swingable);

        if (item.swingable)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stats"), new GUIContent("Weapon Stats"));
            EditorGUI.indentLevel--;
        }

        item.food = EditorGUILayout.Toggle("Food", item.food);

        if (item.food)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("foodStats"), new GUIContent("Food Stats"));
            EditorGUI.indentLevel--;
        }

        item.fuel = EditorGUILayout.Toggle("Fuel", item.fuel);

        if (item.fuel)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fuelStats"), new GUIContent("Fuel Stats"));
            EditorGUI.indentLevel--;
        }

        item.cookable = EditorGUILayout.Toggle("Cookable", item.cookable);

        if (item.cookable)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cookResult"), new GUIContent("Cook Result"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cookTime"), new GUIContent("Cook Time"));
            EditorGUI.indentLevel--;
        }

        item.placable = EditorGUILayout.Toggle("Placable", item.placable);

        if (item.placable)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("placableProps"), new GUIContent("Placable Properties"));
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
}