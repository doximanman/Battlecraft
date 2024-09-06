using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SavableObject : MonoBehaviour
{
    public abstract string SavableName { get; }
    public abstract JObject Save();
    public abstract void Load(JObject serializedObject);
}
