using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class VisualSlider : MonoBehaviour
{
    private RectTransform rectTransform;

    [SerializeField] private float minValueX;
    [SerializeField] private float minValueY;
    [SerializeField] private float maxValueX;
    [SerializeField] private float maxValueY;

    [SerializeField] private float dimensionX;
    [SerializeField] private float dimensionY;

    private Vector2 originalDimension;

    private void OnValidate()
    {
        rectTransform = GetComponent<RectTransform>();
        originalDimension = transform.parent.GetComponent<GridLayoutGroup>().cellSize;
    }

    public (float,float) Dimension
    {
        get => new(dimensionX,dimensionY);
        set
        {
            if(rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            dimensionX = value.Item1;
            dimensionY = value.Item2;
            //originalDimension = new(1, 1);
            // go from [minValueX,maxValueX] to [0,maxHeight]
            // to go from [a,b] range to [c,d] given x in [a,b]
            // the formula is: x->(d-c)*(x-a) + c
            // which is x-> maxHeight*(x-minValueX)/(maxValueX-minValueX)
            float scaledX = (originalDimension.x) * (value.Item1 - minValueX) / (maxValueX - minValueX);
            float scaledY = (originalDimension.y) * (value.Item2 - minValueY) / (maxValueY - minValueY);
            rectTransform.sizeDelta = new(scaledX, scaledY);
        }
    }
}

[CustomEditor(typeof(VisualSlider))]
public class SliderEditor : Editor
{
    SerializedProperty minValueX;
    SerializedProperty maxValueX;
    SerializedProperty minValueY;
    SerializedProperty maxValueY;
    SerializedProperty dimensionX;
    SerializedProperty dimensionY;

    public override VisualElement CreateInspectorGUI()
    {
        var returnVal = base.CreateInspectorGUI();
        minValueX = serializedObject.FindProperty("minValueX");
        maxValueX = serializedObject.FindProperty("maxValueX");
        minValueY = serializedObject.FindProperty("minValueY");
        maxValueY = serializedObject.FindProperty("maxValueY");
        dimensionX = serializedObject.FindProperty("dimensionX");
        dimensionY = serializedObject.FindProperty("dimensionY");
        return returnVal;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(minValueX);
        EditorGUILayout.PropertyField(maxValueX);
        EditorGUILayout.PropertyField(minValueY);
        EditorGUILayout.PropertyField(maxValueY);

        var Dimension = ((VisualSlider)target).Dimension;

        float xValue = EditorGUILayout.Slider("Width", Dimension.Item1, minValueX.floatValue, maxValueX.floatValue);
        float yValue = EditorGUILayout.Slider("Height", Dimension.Item2, minValueY.floatValue, maxValueY.floatValue);

        if(xValue != Dimension.Item1 || yValue != Dimension.Item2)
            ((VisualSlider)target).Dimension = (xValue, yValue);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}

