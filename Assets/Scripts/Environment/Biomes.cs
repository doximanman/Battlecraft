using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class Biomes : MonoBehaviour
{
    [InspectorName("Borders")]
    [SerializeField] private Border[] _borders;

    public static Border[] borders;

    private void Start()
    {
        borders = _borders;
        biomeDetection = _biomeDetection;
    }

    [InspectorName("Biome Detection")]
    [SerializeField] private float _biomeDetection = 1;
    private static float biomeDetection;
    public static Biome GetBiomeClose(float x,Direction direction)
    {
        // move in direction and get the biome there.
        float newX = direction == Direction.RIGHT ? x + biomeDetection :
            direction == Direction.LEFT ? x - biomeDetection : x;
        return GetBiomeAt(newX);
    }

    private static readonly float epsilon = 0.1f;

    /// <summary>
    /// Custom comparator for the cache dictionary.
    /// Treat close x values as the same.
    /// </summary>
    private class DictCacheComparator : IEqualityComparer<(float, Biome)>
    {
        bool IEqualityComparer<(float, Biome)>.Equals((float, Biome) x, (float, Biome) y)
        {
            return Mathf.Abs(x.Item1 - y.Item1) < epsilon && x.Item2==y.Item2;
        }

        int IEqualityComparer<(float, Biome)>.GetHashCode((float, Biome) obj)
        {
            return obj.Item1.GetHashCode() + obj.Item2.GetHashCode();
        }
    }

    // cache close x values and which direction the given biome is in
    static Dictionary<(float,Biome),Direction> biomeWhereCache=new(new DictCacheComparator());

    /// <summary>
    /// Which direction a biome is in,
    /// from a given position.
    /// </summary>
    /// <param name="biome">Biome to look for</param>
    /// <param name="x">Position to look from</param>
    /// <returns>The direction where the biome is at from x</returns>
    public static Direction BiomeWhere(Biome biome,float x)
    {
        // biomes don't change - so cache is nice!
        if (biomeWhereCache.ContainsKey((x, biome)))
            return biomeWhereCache[(x, biome)];

        Assert.IsTrue(borders.Length > 0);

        // gets the position of the biome's borders
        float? leftBorder=null;
        float? rightBorder = null ;

        foreach(Border border in borders)
        {
            // if the right biome of the border is the biome,
            // that means this border is the left border of the biome.
            // and vice versa.
            if (border.rightBiome == biome)
                leftBorder = border.Position;
            else if(border.leftBiome == biome)
                rightBorder = border.Position;
        }

        // biome must exist??
        Assert.IsTrue(leftBorder.HasValue || rightBorder.HasValue);

        // only left border - biome is to the right.
        if (leftBorder.HasValue && !rightBorder.HasValue)
            return biomeWhereCache[(x, biome)] = Direction.RIGHT;
        else if(!leftBorder.HasValue && rightBorder.HasValue)
            return biomeWhereCache[(x,biome)]= Direction.LEFT;
        else
        {
            // find middle of biome and go towards that
            float middle=(leftBorder.Value + rightBorder.Value)/2;
            return biomeWhereCache[(x,biome)] = x > middle ? Direction.LEFT : Direction.RIGHT;
        }
    }

    /// <summary>
    /// Custom comparator for the cache dictionary.
    /// Treat close x values as the same.
    /// </summary>
    private class FloatComparator : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y)
        {
            return Mathf.Abs(x - y) < epsilon;
        }

        int IEqualityComparer<float>.GetHashCode(float obj)
        {
            return obj.GetHashCode();
        }
    }
    static Dictionary<float, Biome> biomeAtCache=new(new FloatComparator());

    /// <summary>
    /// Gets the biome at an X position
    /// </summary>
    /// <param name="x">the position</param>
    /// <returns>Biome at X=x</returns>
    public static Biome GetBiomeAt(float x)
    {
        if (biomeAtCache.ContainsKey(x))
            return biomeAtCache[x];

        Assert.IsTrue(borders.Length > 0);

        // determines which two borders x is between

        // if x is to the left of the first border return the left biome of that border
        if (x < borders[0].Position) return biomeAtCache[x] = borders[0].leftBiome;

        // otherwise try to find the two borders that x is between
        for(int i = 0; i < borders.Length - 1; i++)
        {
            float firstBorder = borders[i].Position;
            float secondBorder = borders[i+1].Position;
            if (x > firstBorder && x < secondBorder)
                return biomeAtCache[x] = borders[i].rightBiome;
        }

        // otherwise necessarily x is past the last border, return its right biome.
        return biomeAtCache[x] = borders[^1].rightBiome;
    }

    // keep the borders sorted by x position
    [CustomEditor(typeof(Biomes))]
    public class BordersEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var instance = (Biomes)target;

            instance._borders=instance._borders.OrderBy(border => border.Position).ToArray();
        }
    }
}
