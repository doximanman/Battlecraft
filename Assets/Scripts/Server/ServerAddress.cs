using UnityEngine;
using TMPro;
using System.Net;

public class ServerAddress : MonoBehaviour
{
    public delegate void OnAddressChange(string newIP, int newPort);

    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private Feedback feedback;


    public static OnAddressChange onAddressChange;
    public static (string ip, int port) Address
    {
        get
        {
            if (!HasAddress())
                return (string.Empty, -1);
            return (PlayerPrefs.GetString("IP"), PlayerPrefs.GetInt("Port"));
        }
        set
        {
            if (Address == value)
                return;
            PlayerPrefs.SetString("IP", value.ip);
            PlayerPrefs.SetInt("Port", value.port);
            onAddressChange?.Invoke(value.ip, value.port);
        }
    }

    private void OnEnable()
    {
        feedback.Clear();
        if (HasAddress())
        {
            (string ip, int port) = Address;
            addressInput.text = ip+":"+port;
        }
        else
        {
            addressInput.text = string.Empty;
        }

        ServerClient.onConnect += OnConnect;
    }

    public void OnConnect(bool _)
    {
        feedback.Clear();
    }

    public static bool HasAddress()
    {
        return PlayerPrefs.HasKey("IP") && PlayerPrefs.HasKey("Port");
    }

    public void ChangeAddress()
    {
        (string oldIP, int oldPort) = Address;

        // try to extract ip and port from text
        string address = addressInput.text;
        string[] ipPort = address.Split(':');
        IPAddress newIP = null;
        int newPort = -1;
        if (ipPort.Length != 2)
            // no colon in the text
            feedback.SetFeedback("Invalid ip address and port");
        else if (!IPAddress.TryParse(ipPort[0], out newIP))
            // ip part isn't an ip
            feedback.SetFeedback("Invalid ip address");
        else if (!int.TryParse(ipPort[1], out newPort))
            // port part isn't a number
            feedback.SetFeedback("Invalid port");
        else if (newPort < IPEndPoint.MinPort || newPort > IPEndPoint.MaxPort)
            // port is outside of valid range
            feedback.SetFeedback("Port out of range");
        else
            feedback.SetFeedback("Address changed");

        string newIPString = newIP == null ? string.Empty : newIP.ToString();
        if (newIPString == oldIP && newPort == oldPort)
            feedback.SetFeedback("Address didn't change");
        else
        {
            feedback.SetFeedback("Address changed");
            Address = (newIPString, newPort);
        }
    }

    private void OnDisable()
    {
        ServerClient.onConnect -= OnConnect;
    }
}
