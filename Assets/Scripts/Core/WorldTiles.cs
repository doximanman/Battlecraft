using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTiles : MonoBehaviour
{
    [SerializeReference] private List<Tilemap> tilemaps;

    public static Vector2 tileSize;

    private void Start()
    {
        tileSize = tilemaps[0].cellSize;
    }
}
