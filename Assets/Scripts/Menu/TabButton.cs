using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


/// <summary>
/// for opening or closing tabs
/// </summary>
public class TabButton : MonoBehaviour
{

    [SerializeField] private MenuLogic menuLogic;

    public enum ButtonType { OPEN, CLOSE };

    /// <summary>
    /// defines whether this button opens a tab -  
    /// in which case a specific tab to open needs to be
    /// specified - or closes a tab; with no type specified.
    /// (closes the last tab)
    /// </summary>
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private TabType openType;

    private void Start()
    {
        // appropriate onclick function of button
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            switch (buttonType)
            {
                // if it's an open button - open the type specified.
                case ButtonType.OPEN:
                    menuLogic.OpenTab(openType);
                    break;
                // if its a close button - close the current tab.
                case ButtonType.CLOSE:
                    menuLogic.CloseTab();
                    break;
            }
        });
    }
}

[CustomEditor(typeof(TabButton))]
public class ButtonEditor : Editor
{
    SerializedProperty buttonType;
    SerializedProperty menuLogic;
    SerializedProperty openType;

    public override VisualElement CreateInspectorGUI()
    {
        var result = base.CreateInspectorGUI();
        buttonType = serializedObject.FindProperty("buttonType");
        menuLogic = serializedObject.FindProperty("menuLogic");
        openType = serializedObject.FindProperty("openType");
        return result;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(menuLogic);
        EditorGUILayout.PropertyField(buttonType);

        // provide a type to open ONLY IF the button
        // is an open button.
        if(buttonType.enumValueFlag == ((int)TabButton.ButtonType.OPEN))
        {
            EditorGUILayout.PropertyField(openType);
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
