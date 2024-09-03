using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public Vector3 position;
    public Quaternion rotation;
    public InventoryData inventory;
    public Vector2 velocity;
    public float health;
    public float food;

    public PlayerData(Player player)
    {
        position = player.transform.position;
        rotation = player.transform.rotation;
        inventory = new InventoryData(InventoryLogic.personalInventory);
        velocity = player.GetComponent<Rigidbody2D>().velocity;
        StatManager playerStats = player.GetComponent<StatManager>();
        health = playerStats.GetStat(StatManager.Health).Value;
        food = playerStats.GetStat(StatManager.Food).Value;
    }

    public void LoadInto(Player player)
    {
        player.transform.SetPositionAndRotation(position, rotation);
        inventory.LoadTo(InventoryLogic.personalInventory);
        player.GetComponent<Rigidbody2D>().velocity = velocity;
        StatManager playerStats = player.GetComponent <StatManager>();
        playerStats.GetStat(StatManager.Health ).Value = health;
        playerStats.GetStat(StatManager.Food ).Value = food;
    }
}
