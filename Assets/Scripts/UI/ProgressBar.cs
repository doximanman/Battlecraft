using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;

public class ProgressBar : MonoBehaviour
{
    #region Direction
    public enum ProgressDirection { leftToRight, rightToLeft ,bottomToTop,topToBottom };
    [SerializeField] private ProgressDirection direction;
    /// <summary>
    /// Defines the direction the progress goes
    /// </summary>
    public ProgressDirection Direction
    {
        get => direction;
        set
        {
            direction = value;
            switch (direction)
            {
                case ProgressDirection.rightToLeft:
                    transform.rotation = Quaternion.Euler(new(0,0,180));
                    break;
                case ProgressDirection.bottomToTop:
                    transform.rotation = Quaternion.Euler(new(0, 0, 90));
                    break;
                case ProgressDirection.topToBottom:
                    transform.rotation = Quaternion.Euler(new(0,0,-90));
                    break;
                default:
                    transform.rotation = Quaternion.Euler(new(0,0,0));
                    break;
            }
        }
    }
    #endregion Direction

    [SerializeField] private RectTransform progressBar;
    [SerializeField] private float originalWidth;
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;

    private void Awake()
    {
        originalWidth = progressBar.rect.width;
    }

    #region Progress
    [SerializeField] private float progress;
    public float Progress
    {
        get => progress;
        set
        {
            // only set if value changed
            if (progress == value) return;
            // treat small numbers as 0
            progress = value > minValue + 0.01f ? value : 0;
            if (progress == 0)
            {
                // don't show progress when progress is 0
                progressBar.gameObject.SetActive(false);
            }
            else
            {
                if (!progressBar.gameObject.activeSelf)
                    progressBar.gameObject.SetActive(true);
                // set the progress bar according to progress.
                // seems like a random formula, it's just what works.
                progressBar.offsetMax = new(originalWidth * (NormalizedProgress - 1), 0);
            }
        }
    }
    private float NormalizedProgress => (progress - minValue) / (maxValue - minValue);
    #endregion Progress


}

[CustomEditor(typeof(ProgressBar))]
public class ProgressBarEditor : Editor
{
    SerializedProperty progress;
    SerializedProperty progressBar;
    SerializedProperty originalWidth;
    SerializedProperty direction;
    SerializedProperty minValue;
    SerializedProperty maxValue;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement returnVal= base.CreateInspectorGUI();
        progress=serializedObject.FindProperty(nameof(progress));
        progressBar=serializedObject.FindProperty(nameof(progressBar));
        originalWidth=serializedObject.FindProperty(nameof(originalWidth));
        minValue=serializedObject.FindProperty(nameof(minValue));
        direction=serializedObject.FindProperty(nameof(direction));
        maxValue=serializedObject.FindProperty(nameof(maxValue));
        return returnVal;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(progressBar);
        EditorGUILayout.PropertyField(direction);
        EditorGUILayout.PropertyField(minValue);
        EditorGUILayout.PropertyField(maxValue);
        EditorGUILayout.Slider(progress, minValue.floatValue, maxValue.floatValue);

        ProgressBar bar = target as ProgressBar;
        bar.Progress = progress.floatValue;
        bar.Direction = (ProgressBar.ProgressDirection) direction.intValue;

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
