using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class ConnectionKeeper : MonoBehaviour
{
    /*[SerializeField] private TMP_Text connectionText;

    async void Start()
    {
        LoginStatus.onLogin += OnLogin;
        ServerClient.onConnect += OnConnect;

        if (!ServerClient.current.Connected)
        {
            // Connect to server
            connectionText.text = "Offline";

            // first, connect to server
            ServerClient.current.SyncAddressWithLocal();
            await ServerClient.current.Connect();
        }
        else if(!LoginStatus.Current.loggedIn)
        {
            connectionText.text = "Not logged in";
        }

        UpdateState();
    }

    public void UpdateState()
    {
        if (ServerClient.Connected && LoginStatus.Current.loggedIn)
            connectionText.text = $"Connected as {LoginStatus.Current.username}";
        else if (ServerClient.Connected && !LoginStatus.Current.loggedIn)
            connectionText.text = "Not logged in";
        else
            connectionText.text = "Offline";
    }

    public void OnLogin(bool a, string b)
    {
        UpdateState();
    }

    public void OnConnect(bool a)
    {
        UpdateState();
    }

    private void OnDestroy()
    {
        LoginStatus.onLogin -= OnLogin;
        ServerClient.onConnect -= OnConnect;
    }*/
}
