using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Net;
using System.Threading;
using UnityEditor.Experimental.GraphView;

public class ServerAddress : MonoBehaviour
{
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private GameObject loadingCircle;
    [SerializeField] private TMP_Text errorText;

    private void OnEnable()
    {
        if (HasAddress())
        {
            (string ip, int port) = GetAddress();
            addressInput.text = ip+":"+port;
        }
        else
        {
            addressInput.text = string.Empty;
        }

        loadingCircle.SetActive(false);
        errorText.text = string.Empty;
    }

    public void StartLoading()
    {
        loadingCircle.SetActive(true);
        errorText.text = string.Empty;
    }

    public void ShowMessage(string message)
    {
        loadingCircle.SetActive(false);
        errorText.text = message;
    }

    private void SetAddress(string ip,int port)
    {
        PlayerPrefs.SetString("IP", ip);
        PlayerPrefs.SetInt("Port", port);
    }

    public static bool HasAddress()
    {
        return PlayerPrefs.HasKey("IP") && PlayerPrefs.HasKey("Port");
    }

    /// <summary>
    /// Returns the saved ip and port. <br/>
    /// Assumes it already exists!
    /// Use HasAddress to check existance.
    /// </summary>
    public static (string ip,int port) GetAddress()
    {
        return (PlayerPrefs.GetString("IP"), PlayerPrefs.GetInt("Port"));
    }

    public void Validate()
    {
        StartLoading();

        string address = addressInput.text;

        string[] ipPort = address.Split(':');
        if(ipPort.Length != 2)
        {
            ShowMessage("Invalid ip address and port");
            return;
        }

        if (!IPAddress.TryParse(ipPort[0],out IPAddress _))
        {
            ShowMessage("Invalid ip address");
            return;
        }

        if (!int.TryParse(ipPort[1],out int tryPort))
        {
            ShowMessage("Invalid port");
            return;
        }

        if(tryPort < IPEndPoint.MinPort || tryPort > IPEndPoint.MaxPort)
        {
            ShowMessage("Port out of range");
            return;
        }


        // send request to server with this
        // as the success function:
        SetAddress(ipPort[0],tryPort);
        ShowMessage("Success!");
    }

}
