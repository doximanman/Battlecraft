using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SavableObject : MonoBehaviour
{
    public abstract string SavableName { get; }
    public abstract string Save();
    public abstract void Load(string serializedObject);
}
