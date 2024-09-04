using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// provides the ability to access all defined types using their name
/// </summary>
public class ItemTypes : MonoBehaviour
{
    [SerializeField] List<ItemType> typesList = new();
    private static readonly Dictionary<string, ItemType> types = new();

    private void Awake()
    {
        foreach (var type in typesList)
            types[type.name] = type;
    }

    public static ItemType GetByName(string name)
    {
        if (name == "None")
            return null;
        return types[name];
    }
}
