using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// simply a class with a public list field.
/// necessary for unity inspector serialization.
/// </summary>
/// <typeparam name="T"></typeparam>
[System.Serializable]
public class ListWrapper<T>
{
    public List<T> list;

    public ListWrapper()
    {
        list = new();
    }

    public ListWrapper(List<T> list)
    {
        this.list = list;
    }
}
