using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


[Serializable]
public class Stat : MonoBehaviour
{
    public string statName;
    [SerializeField] private bool slider;
    public bool Slider => slider;
    [SerializeField] private Slider valueSlider;

    #region ValueProperties

    [SerializeField] float initialValue;
    public float InitialValue => initialValue;

    [SerializeField] private float value;
    public float Value
    {
        get => value;
        set
        {
            bool different = this.value != value;
            this.value = Mathf.Clamp(value, MinValue, MaxValue);
            valueSlider.value = value;
            if (different)
                OnValueChanged?.Invoke(this.value);
        }
    }

    public float Range => MaxValue - MinValue;

    public void SetValueWithoutNotify(float value)
    {
        this.value = value < 0 ? 0 : value;
        valueSlider.value = this.value;
    }

    [SerializeField] private float maxValue;
    public float MaxValue
    {
        get => maxValue;
        set
        {
            maxValue = value;
            Value = Mathf.Clamp(Value,MinValue,MaxValue);
            valueSlider.maxValue = maxValue;
        }
    }

    [SerializeField] private float minValue;
    public float MinValue
    {
        get => minValue;
        set
        {
            minValue = value;
            Value = Mathf.Clamp(Value, MinValue, MaxValue);
            valueSlider.minValue = minValue;
        }
    }
    #endregion

    #region Functionality

    private void Start()
    {
        SetValueWithoutNotify(initialValue);
    }

    public Action<float> OnValueChanged;

    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(Stat))]
public class StatEditor : Editor
{
    #region Serialized Properties
    SerializedProperty statName;
    SerializedProperty slider;
    SerializedProperty valueSlider;
    SerializedProperty initialValue;
    SerializedProperty value;
    SerializedProperty minValue;
    SerializedProperty maxValue;
    #endregion

    public override UnityEngine.UIElements.VisualElement CreateInspectorGUI()
    {
        var result= base.CreateInspectorGUI();
        statName = serializedObject.FindProperty("statName");
        slider = serializedObject.FindProperty("slider");
        initialValue = serializedObject.FindProperty("initialValue");
        value = serializedObject.FindProperty("value");
        valueSlider = serializedObject.FindProperty("valueSlider");
        minValue = serializedObject.FindProperty("minValue");
        maxValue = serializedObject.FindProperty("maxValue");
        return result;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(statName, new GUIContent("Name"));
        string name = statName.stringValue;

        // check if UI slider
        EditorGUILayout.PropertyField(slider, new GUIContent(name + " UI Slider"));
        if (slider.boolValue)
        {
            // slider reference
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(valueSlider, new GUIContent(name + " Slider"));
            EditorGUI.indentLevel--;
        }

        Stat stat = target as Stat;

        EditorGUILayout.PropertyField(initialValue, new GUIContent("Initial Value"));

        // set minimum food
        EditorGUILayout.PropertyField(minValue, new GUIContent("Minimum " + name));
        if(stat.MinValue != minValue.floatValue) stat.MinValue = minValue.floatValue;

        // set maximum food
        EditorGUILayout.PropertyField(maxValue, new GUIContent("Maximum " + name));
        if (stat.MaxValue != maxValue.floatValue) stat.MaxValue = maxValue.floatValue;

        // set current food value
        value.floatValue = EditorGUILayout.Slider(name, stat.Value,stat.MinValue,stat.MaxValue);
        if (value.floatValue != stat.Value) stat.Value = value.floatValue;

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif