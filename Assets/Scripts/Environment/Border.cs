using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Border between two biomes.
/// leftBiome to rightBiome.
/// </summary>
public class Border : MonoBehaviour
{
    [SerializeField] private Collider2D border;
    public Biome leftBiome;
    public Biome rightBiome;

    // Start is called before the first frame update
    void Start()
    {
        border = GetComponent<Collider2D>();
    }

    public float Position => border.bounds.center.x;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        float playerPosition = collision.bounds.center.x;
        float middlePosition = Position;

        bool rightOfBorder = playerPosition > middlePosition;

        Logic.Biome = rightOfBorder ? rightBiome : leftBiome;
    }
}
