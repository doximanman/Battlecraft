using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Entities : MonoBehaviour
{
    public static Entities current;
    public static Action onFirstGenerate;

    [Serializable]
    public class EntityTypePrefab
    {
        public EntityType type;
        public GameObject prefab;
    }

    [SerializeField] private List<EntityTypePrefab> typePrefabs;

    [SerializeField] private bool generated;
    public bool Generated
    {
        get => generated;
        set
        {
            if (value == false)
                onFirstGenerate?.Invoke();
            generated = true;
        }
    }
    public List<Entity> entities;


    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        if (Generated) return;
        Generated = true;
        onFirstGenerate?.Invoke();
    }

    private GameObject GetPrefab(EntityType type)
    {
        return typePrefabs.Find(x => x.type == type).prefab;
    }

    [ContextMenu("Clear")]
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

    [SerializeField] private float summonYOffset;
    /// <summary>
    /// summon entity at position x
    /// </summary>
    /// <param name="type">type to summon</param>
    public void SummonEntity(EntityType type, float x)
    {
        float? groundHeightMaybe = Logic.GetGroundHeightAt(x);
        // do not spawn if no ground!
        if(!groundHeightMaybe.HasValue) return;
        float groundHeight = groundHeightMaybe.Value;
        Vector3 summonPosition = new(x, groundHeight + summonYOffset,0);
        GameObject newEntity = Instantiate(GetPrefab(type),summonPosition, Quaternion.identity,transform);
        newEntity.name = GetPrefab(type).name;
    }
    [SerializeField] private EntityType typeToSummon;
    [SerializeField] private float locationToSummonAt;
    [SerializeField]
    [ContextMenu("Summon Entity")]
    private void SummonEntityInspector()
    {
        SummonEntity(typeToSummon, locationToSummonAt);
    }

    public EntityData[] GetData()
    {
        return entities.Select(entity => new EntityData(entity)).ToArray();
    }

    public void LoadData(EntityData[] data)
    {
        Clear();

        foreach (EntityData entityData in data)
        {
            GameObject typePrefab = GetPrefab(entityData.type);
            Entity entity = Instantiate(typePrefab, transform).GetComponent<Entity>();
            entity.name = typePrefab.name;
            entityData.LoadInto(entity);
        }
    }
}
