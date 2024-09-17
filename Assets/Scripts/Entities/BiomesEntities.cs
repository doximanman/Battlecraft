using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class BiomesEntities : MonoBehaviour
{
    [Serializable]
    public class EntityEntry
    {
        public EntityType type;
        public int minCount;
        public int maxCount;
        public int minCluster;
        public int maxCluster;
    }

    [Serializable]
    public class BiomeEntities
    {
        public Biome biome;
        [Tooltip("Defines for This Biome Which Enemies Will Generate in it")]
        public List<EntityEntry> biomeEntities;
    }

    [Tooltip("Defines For Each Biome Which Entities Will Generate in it")]
    public List<BiomeEntities> biomesEntities;

    private void Awake()
    {
        Entities.onFirstGenerate += GenerateEntities;
    }


    [Tooltip("Space Between Generated Entities")]
    [SerializeField] float spaceBetweenEntities;
    public void GenerateEntities()
    {
        //Entities.current.Clear();
        float xOffset = spaceBetweenEntities;
        foreach (var biomeEntities in biomesEntities)
        {
            (float minX, float maxX) = Biomes.GetBiomeBorders(biomeEntities.biome);
            foreach (var entity in biomeEntities.biomeEntities)
            {
                EntityType type = entity.type;
                int count = Logic.Random(entity.minCount, entity.maxCount + 1);
                do
                {
                    int cluster = Logic.Random(entity.minCluster, entity.maxCluster + 1);
                    if (cluster > count)
                        cluster = count;
                    // generate a random x position inside the biome, that is also outside of the camera view.
                    float position = Logic.GeneratePosition(minX,maxX);
                    for (int i = 0; i < cluster; i++)
                    {
                        // the expected value of the position moves right with i, but it still varies somewhat
                        float offset = xOffset * i + Logic.Random(-xOffset, xOffset);
                        Entities.current.SummonEntity(type, position + offset);
                    }
                    count -= cluster;
                } while (count > 0);

            }
        }
    }

}
