using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderHandler : MonoBehaviour
{

    private Logic logic;
    private Collider2D border;

    // Start is called before the first frame update
    void Start()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();
        border = GetComponent<Collider2D>();

    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (!collision.CompareTag("Player")) return;

        float playerPosition = collision.bounds.center.x;
        float middlePosition = border.bounds.center.x;

        switch (tag)
        {
            case ("PlainsToDesert"):
                logic.Biome = playerPosition < middlePosition ? Biome.PLAINS : Biome.DESERT;
                break;
            case ("IceToPlains"):
                logic.Biome = playerPosition < middlePosition ? Biome.ICE : Biome.DESERT;
                break;
        }
    }
}
