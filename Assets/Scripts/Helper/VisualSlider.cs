using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class VisualSlider : MonoBehaviour
{
    private RectTransform rectTransform;

    [SerializeField] private float minValueX;
    [SerializeField] private float minValueY;
    [SerializeField] private float maxValueX;
    [SerializeField] private float maxValueY;

    [SerializeField] private float valueX;
    [SerializeField] private float valueY;

    private Vector2 originalDimension;

    private void OnValidate()
    {
        rectTransform = GetComponent<RectTransform>();
        originalDimension = transform.parent.GetComponent<GridLayoutGroup>().cellSize;
    }

    // set/get limits of dimensions
    // 
    public ((float,float),(float,float)) ValueLimits
    {
        get => ((minValueX, maxValueX), (minValueY, maxValueY));
        set
        {
            minValueX = value.Item1.Item1;
            maxValueX = value.Item1.Item2;
            minValueY = value.Item2.Item1;
            maxValueY = value.Item2.Item2;

            float newX = Mathf.Clamp(valueX, minValueX, maxValueX);
            float newY = Mathf.Clamp(valueY, minValueY, maxValueY);

            Values = (newX,newY);
        }
    }

    public (float,float) Values
    {
        get => new(valueX,valueY);
        set
        {
            if(rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            valueX = value.Item1;
            valueY = value.Item2;
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

#if UNITY_EDITOR
[CustomEditor(typeof(VisualSlider))]
public class SliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        VisualSlider slider = (VisualSlider)target;
        var valueLimits = slider.ValueLimits;
        var values = slider.Values;

        float minX = EditorGUILayout.FloatField("Minimum X Value", valueLimits.Item1.Item1);
        float maxX = EditorGUILayout.FloatField("Maximum X Value", valueLimits.Item1.Item2);
        float minY = EditorGUILayout.FloatField("Minimum Y Value", valueLimits.Item2.Item1);
        float maxY = EditorGUILayout.FloatField("Maximum Y Value", valueLimits.Item2.Item2);

        var newLimits = ((minX, maxX), (minY, maxY));
        if(newLimits != valueLimits)
            slider.ValueLimits = newLimits;

        float x = EditorGUILayout.Slider("Width", values.Item1, minX, maxX);
        float y = EditorGUILayout.Slider("Height", values.Item2, minY, maxY);

        var newValues = (x, y);
        if(newValues != values)
            ((VisualSlider)target).Values = newValues;

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
