using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectionStarter : MonoBehaviour
{
    [SerializeField] private TMP_Text connectionText;

    private bool isLoggedIn = false;
    async void Start()
    {
        Login.onLogin += (bool loggedIn, string username) =>
        {
            isLoggedIn = loggedIn;
            if (loggedIn)
                connectionText.text = $"Connected as {username}";
            else
                connectionText.text = "Not logged in";
        };

        ServerClient.onConnect += (bool connected) =>
        {
            if (connected && !isLoggedIn)
                connectionText.text = "Not logged in";

            // connected and logged in - handled by login.

            if (!connected)
                connectionText.text = "Offline";
        };

        // Connect to server and try to restore
        // connection from previous login
        connectionText.text = "Connecting to server...";

        // first, connect to server
        ServerClient.SyncAddressWithLocal();
        await ServerClient.Connect();

        // then, try to log in with token
        await Login.TryLoginWithToken();
    }
}
