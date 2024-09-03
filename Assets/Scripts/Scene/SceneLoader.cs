using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static int playingScene = 1;
    public static int mainMenuScene = 0;

    [SerializeField] private Canvas canvas;
    public VisualSlider loadingBar;


    private void Start()
    {
        canvas.gameObject.SetActive(false);
    }

    /// <summary>
    /// loading the world takes time
    /// therefore, show a loading screen
    /// </summary>
    public void LoadPlaying()
    {
        canvas.gameObject.SetActive(true);

        StartCoroutine(LoadPlayingScene());
    }

    private IEnumerator LoadPlayingScene()
    {
        AsyncOperation loadScene = SceneManager.LoadSceneAsync(playingScene);

        while (!loadScene.isDone)
        {
            loadingBar.Values = (loadScene.progress, 1);

            yield return null;
        }
    }

    /// <summary>
    /// main menu loading is immediate.
    /// therefore, use 
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
