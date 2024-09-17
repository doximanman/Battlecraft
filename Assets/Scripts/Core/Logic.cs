using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public enum Biome { PLAINS, ICE, DESERT };
public enum Direction { ZERO, LEFT, RIGHT }
public class Logic : MonoBehaviour
{

    // map bounds
    public const float minX = -470;
    public const float minY = -20;
    public const float maxX = 470;
    public const float maxY = 20;

    private static readonly Biome startBiome = Biome.PLAINS;

    private static readonly List<string> canJumpFrom = new();

    [InspectorName("Collision Detection")]
    [SerializeField] private float _collisionDetection;
    public static float collisionDetection = 0.05f;

    // biome change event - game objects can listen to.
    private static readonly List<IBiomeListener> biomeListeners = new();

    public static readonly Dictionary<GameObject, List<(Collider2D, Vector2)>> touching = new();

    [InspectorName("Ground Collider")]
    [SerializeField] private CompositeCollider2D _ground;

    private static CompositeCollider2D ground;

    private static Biome biome;
    public static Biome Biome
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

    private void OnValidate()
    {
        canJumpFrom.Add("Ground");
        canJumpFrom.Add("Obsticles");

        ground = _ground;
        collisionDetection = _collisionDetection;
        wallCloseDistance = _wallCloseDistance;
        maximumWallAngle = _maximumWallAngle;
    }

    // Start is called before the first frame update
    void Awake()
    {
        canJumpFrom.Add("Ground");
        canJumpFrom.Add("Obsticles");

        ground = _ground;
        collisionDetection = _collisionDetection;
        wallCloseDistance = _wallCloseDistance;
        maximumWallAngle = _maximumWallAngle;
    }

    private static System.Random rand = new();

    public static void RegisterBiomeListener(IBiomeListener listener)
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

    // gameobject must have registered their collisions in
    // the "touching" dictionary
    public static bool IsGrounded(GameObject obj)
    {
        var collider = obj.GetComponent<BoxCollider2D>();
        var rigidbody = obj.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(collider);
        Assert.IsNotNull(rigidbody);

        // create a box to see if there is ground below the player.
        var boxPosition = new Vector2(collider.bounds.center.x, collider.bounds.min.y - collisionDetection / 2);
        var boxSize = new Vector2(collider.bounds.size.x - collisionDetection, collisionDetection);


        var Colliders = Physics2D.OverlapBoxAll(boxPosition, boxSize, 0);

        return Colliders.Any(collider => IsGround(collider));
    }

    [InspectorName("Wall Close Distance")]
    [SerializeField] private float _wallCloseDistance;
    public static float wallCloseDistance;
    [InspectorName("Wall Detection Height")]
    [SerializeField] private float _wallDetectionHeight;
    public static float wallDetectionHeight;
    [InspectorName("Maximum Angle Of Wall Normal With (0,1)")]
    [SerializeField] private float _maximumWallAngle;
    public static float maximumWallAngle;
    // wall is close to object, in the given direction
    public static bool WallClose(GameObject obj, Direction direction)
    {
        var collider = obj.GetComponent<Collider2D>();
        Assert.IsTrue(collider != null);

        var directionVector = direction == Direction.RIGHT ? Vector2.right : Vector2.left;

        RaycastHit2D[] collisions = Physics2D.BoxCastAll(collider.bounds.center, collider.bounds.size, 0, directionVector, wallCloseDistance);

        // normal of collision must point up
        // otherwise its just a slope
        // and the collider must be ground
        return collisions.Any(collision =>
        {
            var angle = Vector2.Angle(-directionVector, collision.normal) % 180;
            bool wallAndGround = angle < maximumWallAngle && IsGround(collision.collider);
            return wallAndGround;
        });
    }

    public static bool CanJump(GameObject obj, Direction direction)
    {
        // raycast up
        // then in the given direction (r shape)
        var collider=obj.GetComponent<Collider2D>();
        Assert.IsTrue(collider != null);

        var center=collider.bounds.center;
        var size = collider.bounds.size;

        RaycastHit2D[] upCollisions = Physics2D.BoxCastAll(center,size, 0, Vector2.up, WorldTiles.tileSize.y);

        // if there's ground above - can't jump.
        if (upCollisions.Any(collision => IsGround(collision.collider)))
            return false;

        if (direction == Direction.ZERO)
            return true;

        var directionVector = direction == Direction.RIGHT ? Vector2.right : Vector2.left;
        // raycast from the higher position (r shape)
        center=new(center.x,center.y+WorldTiles.tileSize.y);

        RaycastHit2D[] diagCollisions = Physics2D.BoxCastAll(center,size, 0, directionVector, wallCloseDistance);

        // ground diagnoally - can't jump.
        if (upCollisions.Any(collision => IsGround(collision.collider)))
            return false;

        return true;
    }

    public static Biome GetStartBiome()
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

    /// <summary>
    /// get the ground height at x position
    /// </summary>
    /// <returns>null if no ground at that x, otherwise the height of the first ground from above</returns>
    public static float? GetGroundHeightAt(float x)
    {
        return GetGroundHeightBelow(new Vector2(x, maxY));
    }

    /// <summary>
    /// Checks the angle between a gameobject and the ground.
    /// The game object should be grounded.
    /// </summary>
    /// <param name="gameObject">Object to check below.</param>
    /// <returns>The angle between the ground and the object.</returns>
    public static bool ShouldSlide(GameObject obj)
    {
        Collider2D collider2D = obj.GetComponent<Collider2D>();

        Vector2 centerOfObject = new(collider2D.bounds.center.x, collider2D.bounds.min.y);
        Vector2 sizeOfDetection = new(collider2D.bounds.size.x, 1f);

        var results = Physics2D.BoxCastAll(centerOfObject, sizeOfDetection, 0, Vector2.down);

        foreach (var hit in results)
        {
            if (IsGround(hit.collider))
                return Vector2.Angle(hit.normal, Vector2.up) < maximumWallAngle;
        }
        return false;
    }



    /// <summary>
    /// generates an out of camera x position, between minX and maxX, uniformly.
    /// </summary>
    public static float GeneratePosition(float minX, float maxX)
    {
        // (0,0) viewport point to world point gives the left most x value at the x coordinate.
        float leftCameraPosition = Camera.main.ViewportToWorldPoint(new(0,0,Camera.main.nearClipPlane)).x - 5f;
        // (1,1) viewport point to world point gives the right most x value at the x coordinate.
        float rightCameraPosition = Camera.main.ViewportToWorldPoint(new(1,1,Camera.main.nearClipPlane)).x + 5f;

        // generate an x value until it is outside of the camera's bounds
        float x = Random(minX, maxX);
        while( x > leftCameraPosition && x < rightCameraPosition )
            x = Random(minX, maxX);
        return x;
    }

    public static float Random(float min, float max)
    {
        return min + (max - min) * ((float)rand.NextDouble());
    }

    public static int Random(int min, int max)
    {
        return rand.Next(min, max);
    }
}
