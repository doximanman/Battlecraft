using Newtonsoft.Json.Linq;
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
    public float[] position;
    public float[] rotation;
    public float health;

    private EntityData() { }

    public EntityData(Entity entity)
    {
        type = entity.type;

        Vector3 position = entity.transform.position;
        this.position = new float[3];
        this.position[0] = position.x;
        this.position[1] = position.y;
        this.position[2] = position.z;

        Quaternion rotation = entity.transform.rotation;
        this.rotation = new float[4];
        this.rotation[0] = rotation.x;
        this.rotation[1] = rotation.y;
        this.rotation[2] = rotation.z;
        this.rotation[3] = rotation.w;

        health = entity.Health;
    }

    public void LoadInto(Entity entity)
    {
        entity.type = type;

        Vector3 newPosition = new(position[0], position[1], position[2]);
        Quaternion newRotation = new(rotation[0], rotation[1], rotation[2], rotation[3]);
        entity.transform.SetPositionAndRotation(newPosition, newRotation);

        entity.Health = health;
    }

    public static JObject Save(EntityData entityData)
    {
        return new()
        {
            ["type"] = Enum.GetName(typeof(EntityType), entityData.type),
            ["position"] = JArray.FromObject(entityData.position),
            ["rotation"] = JArray.FromObject(entityData.rotation),
            ["health"] = entityData.health,
        };
    }

    public static EntityData Load(JObject serialized)
    {
        string typeName = serialized["type"].ToString();
        EntityType entityType = (EntityType) Enum.Parse(typeof(EntityType), typeName);
        float[] entityPosition = serialized["position"].ToObject<float[]>();
        float[] entityRotation = serialized["rotation"].ToObject<float[]>();
        float entityHealth = serialized["health"].Value<float>();
        return new()
        {
            type = entityType,
            position = entityPosition,
            rotation = entityRotation,
            health = entityHealth,
        };
    }
}
