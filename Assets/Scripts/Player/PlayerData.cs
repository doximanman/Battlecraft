using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public float[] position;
    public float[] rotation;
    public JObject inventorySerialized;
    public float[] velocity;
    public float health;
    public float food;

    public PlayerData(Player player)
    {
        Vector3 position = player.transform.position;

        this.position = new float[3];
        this.position[0] = position.x;
        this.position[1] = position.y;
        this.position[2] = position.z;

        Quaternion rotation = player.transform.rotation;

        this.rotation = new float[4];
        this.rotation[0] = rotation.x;
        this.rotation[1] = rotation.y;
        this.rotation[2] = rotation.z;
        this.rotation[3] = rotation.w;

        // convert current inventory into inventory data and serialize it
        inventorySerialized = InventoryData.Serialize(new InventoryData(InventoryLogic.personalInventory));

        Vector2 velocity = player.GetComponent<Rigidbody2D>().velocity;

        this.velocity = new float[2];
        this.velocity[0] = velocity.x;
        this.velocity[1] = velocity.y;

        StatManager playerStats = player.GetComponent<StatManager>();
        health = playerStats.GetStat(StatManager.Health).Value;
        food = playerStats.GetStat(StatManager.Food).Value;
    }

    private PlayerData() { }

    public void LoadInto(Player player)
    {
        Vector3 newPosition = new(position[0],position[1], position[2]);
        Quaternion newRotation = new(rotation[0], rotation[1], rotation[2], rotation[3]);
        player.transform.SetPositionAndRotation(newPosition, newRotation);
        // convert serialized string to inventorydata and load it into the current inventory.
        InventoryData deserializedData = InventoryData.Deserialize(inventorySerialized);
        deserializedData.LoadTo(InventoryLogic.personalInventory);
        Vector2 newVelocity = new(velocity[0], velocity[1]);
        player.GetComponent<Rigidbody2D>().velocity = newVelocity;
        StatManager playerStats = player.GetComponent <StatManager>();
        playerStats.GetStat(StatManager.Health ).Value = health;
        playerStats.GetStat(StatManager.Food ).Value = food;
    }

    public static JObject Save(PlayerData playerData)
    {
        return new()
        {
            ["position"] = JArray.FromObject(playerData.position),
            ["rotation"] = JArray.FromObject(playerData.rotation),
            ["inventory"] = playerData.inventorySerialized,
            ["velocity"] = JArray.FromObject(playerData.velocity),
            ["health"] = playerData.health,
            ["food"] = playerData.food,
        };
    }

    public static PlayerData Load(JObject serialized)
    {
        return new()
        {
            position = (serialized["position"] as JArray).ToObject<float[]>(),
            rotation = (serialized["rotation"] as JArray).ToObject<float[]>(),
            inventorySerialized = serialized["inventory"] as JObject,
            velocity = (serialized["velocity"] as JArray).ToObject<float[]>(),
            health = serialized["health"].Value<float>(),
            food = serialized["food"].Value<float>()
        };
    }
}
