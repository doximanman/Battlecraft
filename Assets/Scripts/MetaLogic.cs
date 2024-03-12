using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class MetaLogic : MonoBehaviour
{
    public static bool paused=false;

    public static void Pause()
    {
        Time.timeScale = 0;
        Camera.main.GetComponent<PostProcessVolume>().enabled = true;
        paused = true;
    }

    public static void Unpause()
    {
        Time.timeScale = 1;
        Camera.main.GetComponent<PostProcessVolume>().enabled = false;
        paused = false;
    }
}
