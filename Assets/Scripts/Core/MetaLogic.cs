using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class MetaLogic : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    public delegate void ToggleListener(bool on);

    public static GameObject darkenBackground;
    public static GameObject pauseMenu;

    public static double doubleClickDelay;

    public static bool paused;

    public static bool pauseMenuEnabled;

    public static bool mouseDownOnBlock;

    private void Start()
    {
        doubleClickDelay = 0.2f;
        paused = false;
        pauseMenuEnabled = false;
        mouseDownOnBlock = false;

        darkenBackground = GameObject.FindGameObjectWithTag("DarkBackground");
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");

        KeyInput.instance.onEscape += (down,_,_) =>
        {
            if (down)
            {
                if (pauseMenuEnabled) ClosePauseMenu();
                else OpenPauseMenu();
            }
        };


    }

    public static void DarkenBackground()
    {
        darkenBackground.GetComponent<Image>().enabled = true;
    }

    public static void UndarkenBackground()
    {
        darkenBackground.GetComponent<Image>().enabled = false;
    }

    public static ToggleListener pauseListeners;

    public static void Pause()
    {
        pauseListeners?.Invoke(true);
        Time.timeScale = 0;
        Camera.main.GetComponent<PostProcessVolume>().enabled = true;
        paused = true;
    }

    public static void Unpause()
    {
        pauseListeners?.Invoke(false);
        Time.timeScale = 1;
        Camera.main.GetComponent<PostProcessVolume>().enabled = false;
        paused = false;
    }

    public static ToggleListener pauseMenuListeners;

    public static void OpenPauseMenu()
    {
        pauseMenuListeners?.Invoke(true);
        Pause();
        pauseMenu.SetActive(true);
        DarkenBackground();
        pauseMenuEnabled = true;
    }

    public static void ClosePauseMenu()
    {
        pauseMenuListeners?.Invoke(false);
        Unpause();
        pauseMenu.SetActive(false);
        UndarkenBackground();
        pauseMenuEnabled = false;
    }

    public void Quit()
    {
        ClosePauseMenu();
        sceneLoader.Load(SceneLoader.mainMenuScene);
    }
}