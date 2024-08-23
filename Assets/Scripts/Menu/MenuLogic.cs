using PlasticGui.Configuration.CloudEdition.Welcome;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    [SerializeField] private Settings settingsTab;
    [SerializeField] private GameObject buttonsTab;
    [SerializeField] private Login loginTab;

    public void StartPlaying()
    {
        sceneLoader.Load(SceneLoader.playingScene);
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }

    public void OpenSettingsTab()
    {
        settingsTab.gameObject.SetActive(true);
        buttonsTab.SetActive(false);
        settingsTab.Open();
    }

    public void CloseSettingsTab()
    {
        settingsTab.gameObject.SetActive(false);
        buttonsTab.SetActive(true);
        settingsTab.Close();
    }

    public void OpenLogin()
    {

    }

    public void CloseLogin()
    {

    }
}
