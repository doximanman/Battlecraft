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
    [SerializeField] private Feedback feedback;

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

    public async void Connect()
    {
        (string ip, int port) = GetAddress();
        SetAddress(ip, port);

        // check validity of ip format locally
        // (if invalid, why involve server?)
        feedback.StartLoading();


        string address = addressInput.text;

        string[] ipPort = address.Split(':');
        if(ipPort.Length != 2)
        {
            feedback.SetFeedback("Invalid ip address and port");
            return;
        }

        if (!IPAddress.TryParse(ipPort[0],out IPAddress _))
        {
            feedback.SetFeedback("Invalid ip address");
            return;
        }

        if (!int.TryParse(ipPort[1],out int tryPort))
        {
            feedback.SetFeedback("Invalid port");
            return;
        }

        if(tryPort < IPEndPoint.MinPort || tryPort > IPEndPoint.MaxPort)
        {
            feedback.SetFeedback("Port out of range");
            return;
        }


        ServerClient.SyncAddressWithLocal();
        (bool success, string error) = await ServerClient.Connect();
        if (!success)
        {
            feedback.SetFeedback(error);
            return;
        }
            
        success = await ServerAPI.VerifyServer();
        if (success)
        {
            SetAddress(ipPort[0], tryPort);
            feedback.SetFeedback("Connected");
        }
        else
        {
            feedback.SetFeedback("Couldn't connect");
        }
    }
}
