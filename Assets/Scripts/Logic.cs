using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    // constant variables for biome names
    // so that if someone needs a biome's name
    // they don't have to use a hardcoded string
    public const string plainsBiome = "plains";
    public const string iceBiome = "ice";
    public const string desertBiome = "desert";

    // map bounds
    public const float minX = -470;
    public const float minY = -20;
    public const float maxX = 470;
    public const float maxY = 20;

    private string startBiome = plainsBiome;

    private readonly List<string> canJumpFrom = new();

    // biome change event - game objects can listen to.
    private readonly List<IBiomeListener> biomeListeners = new();


    private string biome;
    public string Biome
    {
        get
        {
            return biome;
        }
        set
        {
            biome = value;
            foreach (var listener in biomeListeners)
            {
                listener.OnBiomeChange(biome);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        canJumpFrom.Add("Ground");
        canJumpFrom.Add("Obsticles");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RegisterBiomeListener(IBiomeListener listener)
    {
        biomeListeners.Add(listener);
    }

    // preferably use canJumpOn(collider)
    public bool CanJumpOn(string tag)
    {
        return canJumpFrom.Contains(tag);
    }

    public bool CanJumpOn(Collider2D collider)
    {
        return canJumpFrom.Contains(collider.tag) || CanJumpOn(collider.transform.parent);
    }

    private bool CanJumpOn(Transform transform)
    {
        if (transform == null) return false;

        return canJumpFrom.Contains(transform.tag) || CanJumpOn(transform.parent);
    }

    public string GetStartBiome()
    {
        return startBiome;
    }

    // gets the ground height of x position positionX
    public static IEnumerable<float> GetGroundHeight(float positionX)
    {
        float BIG_HEIGHT = maxY - minY;

        Vector2 start = new(positionX, maxY);
        var result = Physics2D.RaycastAll(start, Vector2.down, BIG_HEIGHT);

        foreach (var hit in result)
        {
            if (hit.collider.CompareTag("Ground"))
                yield return hit.point.y;
        }
    }
}
