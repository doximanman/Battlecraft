using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public Vector3 position;
    public Quaternion rotation;
    public string inventorySerialized;
    public Vector2 velocity;
    public float health;
    public float food;

    public PlayerData(Player player)
    {
        position = player.transform.position;
        rotation = player.transform.rotation;
        // convert current inventory into inventory data and serialize it
        inventorySerialized = InventoryData.Serialize(new InventoryData(InventoryLogic.personalInventory));
        velocity = player.GetComponent<Rigidbody2D>().velocity;
        StatManager playerStats = player.GetComponent<StatManager>();
        health = playerStats.GetStat(StatManager.Health).Value;
        food = playerStats.GetStat(StatManager.Food).Value;
    }

    public void LoadInto(Player player)
    {
        player.transform.SetPositionAndRotation(position, rotation);
        // convert serialized string to inventorydata and load it into the current inventory.
        InventoryData deserializedData = InventoryData.Deserialize(inventorySerialized);
        deserializedData.LoadTo(InventoryLogic.personalInventory);
        player.GetComponent<Rigidbody2D>().velocity = velocity;
        StatManager playerStats = player.GetComponent <StatManager>();
        playerStats.GetStat(StatManager.Health ).Value = health;
        playerStats.GetStat(StatManager.Food ).Value = food;
    }
}
