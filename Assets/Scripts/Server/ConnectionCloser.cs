using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionCloser : MonoBehaviour
{
    private async void OnApplicationQuit()
    {
        await ServerClient.Disconnect();
    }
}
