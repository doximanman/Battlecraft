using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
    public delegate void ToggleListener(bool toggle);

    [SerializeField] private Settings settingsTab;
    [SerializeField] private Button backToGame;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private bool settingsTabEnabled;

    private void Start()
    {
        settingsTabEnabled = true;
        print(settingsTab == null);

        MetaLogic.pauseMenuListeners += (on) =>
        {
            if (on) return;
            if (!on && settingsTabEnabled) DisableSettingsTab();
        };
    }

    private void Update()
    {
        
    }

    public void DisableButtons()
    {
        backToGame.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
    }

    public void EnableButtons()
    {
        backToGame.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    public void EnableSettingsTab()
    {
        settingsTab.gameObject.SetActive(true);
        DisableButtons();
        settingsTab.Open();
        settingsTabEnabled = true;
    }


    public void DisableSettingsTab()
    {
        settingsTab.Close();
        settingsTab.gameObject.SetActive(false);
        EnableButtons();
        settingsTabEnabled = false;
    }

}
