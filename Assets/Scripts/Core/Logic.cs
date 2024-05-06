using PlasticGui.Configuration.CloudEdition.Welcome;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

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

    private readonly string startBiome = plainsBiome;

    private static readonly List<string> canJumpFrom = new();

    [InspectorName("Collision Detection")]
    [SerializeField] private float _collisionDetection;
    public static float collisionDetection = 0.05f;

    // biome change event - game objects can listen to.
    private readonly List<IBiomeListener> biomeListeners = new();

    [InspectorName("Ground Collider")]
    [SerializeField] private CompositeCollider2D _ground;

    private static CompositeCollider2D ground;

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

        ground = _ground;
        collisionDetection = _collisionDetection;
        wallCloseDistance = _wallCloseDistance;
    }

    public void RegisterBiomeListener(IBiomeListener listener)
    {
        biomeListeners.Add(listener);
    }

    // preferably use canJumpOn(collider)
    public static bool IsGround(string tag)
    {
        return canJumpFrom.Contains(tag);
    }

    public static bool IsGround(Collider2D collider)
    {
        return canJumpFrom.Contains(collider.tag) || IsGround(collider.transform.parent);
    }

    private static bool IsGround(Transform transform)
    {
        if (transform == null) return false;

        return canJumpFrom.Contains(transform.tag) || IsGround(transform.parent);
    }

    public static bool IsGrounded(GameObject obj)
    {
        var collider = obj.GetComponent<BoxCollider2D>();
        var rigidbody=obj.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(collider);
        Assert.IsNotNull(rigidbody);

        // create a box to see if there is ground below the player.
        Vector2 boxPosition = new(collider.bounds.center.x, collider.bounds.min.y - collisionDetection / 2); ;
        Vector2 boxSize = new(collider.size.x * rigidbody.transform.lossyScale.x - 2 * collisionDetection, collisionDetection);

        var Colliders = Physics2D.OverlapBoxAll(boxPosition, boxSize, 0);

        return Colliders.Any(collider => IsGround(collider));
    }

    [InspectorName("Wall Close Distance")]
    [SerializeField] private float _wallCloseDistance;
    public static float wallCloseDistance;
    [InspectorName("Wall Detection Height")]
    [SerializeField] private float _wallDetectionHeight;
    public static float wallDetectionHeight;
    // wall is close to object, in the given direction
    public static bool WallClose(GameObject obj,float direction)
    {
        var collider = obj.GetComponent<Collider2D>();
        Assert.IsNotNull(collider);

        var position=new Vector2(obj.transform.position.x,collider.bounds.min.y+ wallDetectionHeight);
        var directionVector = direction < 0 ? Vector2.left : Vector2.right;
        var colliders = Physics2D.RaycastAll(position, directionVector, wallCloseDistance);

        return colliders.Any(collider => IsGround(collider.collider));
    }

    public string GetStartBiome()
    {
        return startBiome;
    }

    // gets the nearest ground below the given position
    public static float? GetGroundHeightBelow(Vector2 position)
    {
        // if inside ground return null
        //Debug.Log("position: " + position + " overlap: " + ground.OverlapPoint(position) + " closest point: "+ground.ClosestPoint(position));
        if (ground.OverlapPoint(position)) return null;

        var allHits = Physics2D.RaycastAll(position, Vector2.down);

        foreach (var hit in allHits)
        {
            if (IsGround(hit.collider))
                return hit.point.y;
        }

        return null;
    }
}
