using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaLogic : MonoBehaviour
{
    public static bool paused=false;

    public static void Pause()
    {
        Time.timeScale = 0;
        paused = true;
    }

    public static void Unpause()
    {
        Time.timeScale = 1;
        paused = false;
    }
}
