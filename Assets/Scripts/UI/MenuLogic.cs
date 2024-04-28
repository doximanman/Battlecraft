using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
    public delegate void ToggleListener(bool toggle);

    [InspectorName("Settings Tab")]
    [SerializeField] private GameObject _settingsTab;
    [InspectorName("Back To Game Button")]
    [SerializeField] private Button _backToGame;
    [InspectorName("Settings Button")]
    [SerializeField] private Button _settingsButton;
    [InspectorName("Quit Button")]
    [SerializeField] private Button _quitButton;

    public static GameObject settingsTab;
    private static Button backToGame;
    private static Button settingsButton;
    private static Button quitButton;

    private void Start()
    {
        settingsTab=_settingsTab;
        backToGame = _backToGame;
        settingsButton = _settingsButton;
        quitButton = _quitButton;

        settingsButton.onClick.AddListener(EnableSettingsTab);

        MetaLogic.pauseMenuListeners += (on) =>
        {
            if (on) return;
            if (!on) DisableSettingsTab();
        };
    }

    public static void DisableButtons()
    {
        backToGame.gameObject.SetActive(false);
        settingsButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
    }

    public static void EnableButtons()
    {
        backToGame.gameObject.SetActive(true);
        settingsButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    public static ToggleListener onSettingsTabBefore;
    public static ToggleListener onSettingsTabAfter;

    public static void EnableSettingsTab()
    {
        onSettingsTabBefore?.Invoke(true);
        DisableButtons();
        settingsTab.SetActive(true);
        onSettingsTabAfter?.Invoke(true);
    }

    public static void DisableSettingsTab()
    {
        onSettingsTabBefore?.Invoke(false);
        settingsTab.SetActive(false);
        EnableButtons();
        onSettingsTabAfter?.Invoke(false);
    }

}
