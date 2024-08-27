using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Entities : MonoBehaviour
{
    public static Entities current;

    [Serializable]
    public class EntityTypePrefab
    {
        public EntityType type;
        public GameObject prefab;
    }

    [SerializeField] private List<EntityTypePrefab> typePrefabs;

    public List<Entity> entities;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        DataManager.instance.onSave += Save;
        DataManager.instance.onLoad += Load;
    }

    private GameObject GetPrefab(EntityType type)
    {
        return typePrefabs.Find(x => x.type == type).prefab;
    }

    /// <summary>
    /// destroy all entities in the game
    /// </summary>
    public void Clear()
    {
        // destroy the children!!
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        transform.DetachChildren();
        entities = new();
    }

    [ContextMenu("Load From File")]
    public void Load()
    {
        Clear();

        EntityData[] data = EntitySaver.LoadEntities();

        foreach (EntityData entityData in data)
        {
            GameObject typePrefab = GetPrefab(entityData.type);
            Entity entity = Instantiate(typePrefab, transform).GetComponent<Entity>();
            entity.name = typePrefab.name;
            entityData.LoadInto(entity);
        }
    }

    [ContextMenu("Save To File")]
    public void Save()
    {
        // create EntityData for every Entity
        EntityData[] data = entities.Select(entity => new EntityData(entity)).ToArray();

        EntitySaver.SaveEntities(data);
    }

    [ContextMenu("Load Default Entities")]
    public void LoadDefault()
    {
        Clear();

        EntityData[] data = EntitySaver.LoadEntities(true);

        foreach (EntityData entityData in data)
        {
            GameObject typePrefab = GetPrefab(entityData.type);
            Entity entity = Instantiate(typePrefab, transform).GetComponent<Entity>();
            entity.name = typePrefab.name;
            entityData.LoadInto(entity);
        }
    }

    [ContextMenu("Save Default Entities")]
    public void SaveDefault()
    {
        // create EntityData for every Entity
        EntityData[] data = entities.Select(entity => new EntityData(entity)).ToArray();

        EntitySaver.SaveEntities(data,true);
    }
}
