using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SavableObject : MonoBehaviour
{
    public abstract string SavableObjectName { get; }
    public abstract string SerializeObject();
    public abstract void DeserializeObject(string serializedObject);
}
