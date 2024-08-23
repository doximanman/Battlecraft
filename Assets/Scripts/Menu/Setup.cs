using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// initial state of the scene
/// </summary>
public class Setup : MonoBehaviour
{
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject settingsTab;
    [SerializeField] private GameObject loginTab;

    private void Start()
    {
        settingsTab.SetActive(false);
        loginTab.SetActive(false);
        buttons.SetActive(true);
    }
}
