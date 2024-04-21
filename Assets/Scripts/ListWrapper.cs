using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// necessary for unity serialization
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
