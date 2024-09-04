using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

/// <summary>
/// serializes the player for saving. <br/>
/// must be attatched to the player object.
/// </summary>
public class PlayerSaver : SavableObject
{
    private Player player;
    private void Awake()
    {
        player = GetComponent<Player>();
    }

    public override string SavableName => "Player";

    public override string Save()
    {
        PlayerData data = new(player);
        return JsonUtility.ToJson(data);
    }

    public override void Load(string serializedObject)
    {
        PlayerData data = JsonUtility.FromJson<PlayerData>(serializedObject);
        data.LoadInto(player);
    }
}
