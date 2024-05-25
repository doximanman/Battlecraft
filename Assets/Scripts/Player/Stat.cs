using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
            bool different = valueSlider.value != value;
            this.value = value < 0 ? 0 : value;
            valueSlider.value = this.value;
            if (different)
                OnValueChanged?.Invoke(this.value);
        }
    }

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
            valueSlider.minValue = minValue;
        }
    }
    #endregion

    #region Functionality

    [SerializeField] private bool loseOvertime;
    public bool LoseOvertime => loseOvertime;
    [SerializeField] private float loseInterval;
    public float LoseInterval => loseInterval;
    [SerializeField] private float loseAmount;
    public float LoseAmount => loseAmount;

    private void Start()
    {
        SetValueWithoutNotify(initialValue);
    }

    private float timer;
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > loseInterval)
        {
            Value -= loseAmount;
            timer = 0;
        }
    }

    public Action<float> OnValueChanged;

    #endregion
}

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
    SerializedProperty loseOvertime;
    SerializedProperty loseInterval;
    SerializedProperty loseAmount;
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
        loseOvertime = serializedObject.FindProperty("loseOvertime");
        loseInterval = serializedObject.FindProperty("loseInterval");
        loseAmount = serializedObject.FindProperty("loseAmount");
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
            //SerializedProperty valueSlider = serializedObject.FindProperty("valueSlider");
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

        //SerializedProperty loseOvertime = serializedObject.FindProperty("loseOvertime");
        EditorGUILayout.PropertyField(loseOvertime, new GUIContent("Lose Overtime"));
        if(loseOvertime.boolValue)
        {
            //SerializedProperty loseInterval = serializedObject.FindProperty("loseInterval");
            //SerializedProperty loseAmount = serializedObject.FindProperty("loseAmount");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(loseInterval, new GUIContent("Lose Interval (lose every X seconds)"));
            EditorGUILayout.PropertyField(loseAmount, new GUIContent("Lose Amount"));
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
