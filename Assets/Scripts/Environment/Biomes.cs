using System.Collections;
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
    }

    /// <summary>
    /// Gets the biome at an X position
    /// </summary>
    /// <param name="x">the position</param>
    /// <returns>Biome at X=x</returns>
    public static Biome GetBiome(float x)
    {
        Assert.IsTrue(borders.Length > 0);

        // determines which two borders x is between

        // if x is to the left of the first border return the left biome of that border
        if (x < borders[0].Position) return borders[0].leftBiome;

        // otherwise try to find the two borders that x is between
        for(int i = 0; i < borders.Length - 1; i++)
        {
            float firstBorder = borders[i].Position;
            float secondBorder = borders[i+1].Position;
            if (x > firstBorder && x < secondBorder)
                return borders[i].rightBiome;
        }

        // otherwise necessarily x is past the last border, return its right biome.
        return borders[^1].rightBiome;
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
