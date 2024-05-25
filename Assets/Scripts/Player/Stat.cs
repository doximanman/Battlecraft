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
            this.value = value < MinValue ? MinValue : value;
            this.value = this.value > MaxValue ? MaxValue : value;
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

    [SerializeField] private bool changeOvertime;
    public bool ChangeOvertime => changeOvertime;
    [SerializeField] private float changeInterval;
    public float ChangeInterval => changeInterval;
    [SerializeField] private float changeAmount;
    public float ChangeAmount => changeAmount;

    private void Start()
    {
        SetValueWithoutNotify(initialValue);
    }

    private float timer;
    private void Update()
    {
        if (changeOvertime)
        {
            timer += Time.deltaTime;
            if (timer > changeInterval)
            {
                Value += changeAmount;
                timer = 0;
            }
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
    SerializedProperty changeOvertime;
    SerializedProperty changeInterval;
    SerializedProperty changeAmount;
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
        changeOvertime = serializedObject.FindProperty("changeOvertime");
        changeInterval = serializedObject.FindProperty("changeInterval");
        changeAmount = serializedObject.FindProperty("changeAmount");
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
        EditorGUILayout.PropertyField(changeOvertime, new GUIContent("Change Overtime"));
        if(changeOvertime.boolValue)
        {
            //SerializedProperty loseInterval = serializedObject.FindProperty("loseInterval");
            //SerializedProperty loseAmount = serializedObject.FindProperty("loseAmount");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(changeInterval, new GUIContent("Change Interval (Change every X seconds)"));
            EditorGUILayout.PropertyField(changeAmount, new GUIContent("Change Amount (Add X every interval)"));
            EditorGUI.indentLevel--;
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
