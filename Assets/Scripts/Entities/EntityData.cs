using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum EntityType { 
    CHICKEN,
    COW,
    SHEEP,
    SLIME,
    WOLF,
    DESERT_WOLF,
    SNAKE,
    DEER,
    ICE_SLIME
}

[Serializable]
public class EntityData
{
    public EntityType type;
    public Vector3 position;
    public Quaternion rotation;
    public float health;

    public EntityData(Entity entity)
    {
        type = entity.type;
        position = entity.transform.position;
        rotation = entity.transform.rotation;
        health = entity.Health;
    }

    public void LoadInto(Entity entity)
    {
        entity.type = type;
        entity.transform.SetPositionAndRotation(position, rotation);
        entity.Health = health;
    }
}
