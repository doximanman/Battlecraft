using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static int playingScene = 1;
    public static int mainMenuScene = 0;

    public void Load(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
