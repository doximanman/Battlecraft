using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnectionStatus : MonoBehaviour
{
    [SerializeField] private TMP_Text connectionText;
    [SerializeField] private float updateInterval;

    private float timer;
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > updateInterval)
        {
            timer = 0;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        if (ServerClient.Connected && Login.Current.loggedIn)
            connectionText.text = $"Connected as {Login.Current.username}";
        else if (ServerClient.Connected && !Login.Current.loggedIn)
            connectionText.text = "Not logged in";
        else
            connectionText.text = "Offline";
    }
}
