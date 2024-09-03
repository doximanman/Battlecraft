using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ServerClient : MonoBehaviour
{
    public static ServerClient current;
    public delegate void OnConnect(bool connected);

    public static OnConnect onConnect;
    private static ClientWebSocket client = null;

    private void Awake()
    {
        current = this;
    }

    private async void OnEnable()
    {
        ServerAddress.onAddressChange += OnAddressChange;
        if (Connected) return;
        await Connect();

        
    }


    [SerializeField] private float pingInterval;
    private static float downTime = 0;
    private void Update()
    {
        downTime += Time.deltaTime;
        if (downTime > pingInterval)
        {
            downTime = 0;
            Heartbeat();
        }
    }

    private bool heartBeating = false;
    /// <summary>
    /// preiodically ping the server
    /// and reconnect if needed
    /// </summary>
    public async void Heartbeat()
    {
        if (heartBeating) return;
        // only one heartbeat can be done simultaniously
        heartBeating = true;
        string pong = await Request("ping");
        if(pong != "pong")
        {
            Connected = false;
            await Connect();
        }
        heartBeating = false;
    }


    public async void OnAddressChange(string ip, int port)
    {
        if (Connected)
            await Disconnect();
        await Connect();
    }
    public string ConnectionString(string ip,int port) => $"ws://{ip}:{port}";


    private static bool connected;
    public static bool Connected
    {
        get => connected;
        set {
            if (connected == value) return;
            connected = value;
            onConnect?.Invoke(connected);
        }
    }

    private async Task<(bool success, string errorMessage)> Connect()
    {
        client = new();
        Uri serverUri;
        (string ip, int port) = ServerAddress.Address;
        try { serverUri = new(ConnectionString(ip, port)); }
        catch(Exception e)
        {
            UnityEngine.Debug.Log("At creating URI: "+e);
            Connected = false;
            return (false,"Invalid server address");
        }

        try
        {
            CancellationTokenSource source = new();
            // try to connect with 5 second timeout
            source.CancelAfter(5000);
            await client.ConnectAsync(serverUri, source.Token);
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log("At connect: "+e);
            Connected = false;
            return (false,"Couldn't connect to server");
        }


        string pong = await Request("ping",true);
        if (pong != "pong")
        {
            Connected = false;
            return (false, pong);
        }

        Connected = true;
        return (true,string.Empty);
    }

    private async Task<string> Disconnect()
    {
        if (!Connected)
            return "Not connected";

        try {
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "request to disconnect", CancellationToken.None);
        }
        catch(Exception e)
        {
            UnityEngine.Debug.Log("At close: "+e);
            if (client.State != WebSocketState.Open)
                Connected = false;
            return "Error while disconnecting";
        }

        Connected = false;
        return "Disconnected";
    }

    private static bool busy = false;
    /// <summary>
    /// Send a request using THE CONNECTED client.<br/>
    /// (must be connected. use Connect function.)
    /// </summary>
    /// <param name="request">json of the request</param>
    /// <returns>json of the response</returns>
    public static async Task<string> Request(string request,bool ignoreConnected = false)
    {
        if (!ignoreConnected && !Connected)
            return "Not connected";

        // another request is being handled - wait.
        while (busy)
            await Task.Delay(25);

        // send json as byte array
        byte[] buffer = Encoding.UTF8.GetBytes(request);
        // 'true' means this is the last message
        busy = true;
        try { await client.SendAsync(new(buffer), WebSocketMessageType.Text, true, CancellationToken.None); }
        catch(Exception e){
            busy = false;
            UnityEngine.Debug.Log("At send: "+e);
            // notify of possible disconnect
            if (client.State != WebSocketState.Open)
                Connected = false;
            return "Couldn't send to server";
        }
        busy = false;

        // read until end of message
        List<byte> preResponse = new();
        // reuse the same buffer
        buffer = new byte[4096];
        busy = true;
        WebSocketReceiveResult result;
        do
        {
            try { result = await client.ReceiveAsync(new(buffer), CancellationToken.None); }
            catch(Exception e){
                busy = false;
                UnityEngine.Debug.Log("At receive: "+e);
                if (client.State != WebSocketState.Open)
                    Connected = false;
                return "Couldn't receive from server";
            }
            preResponse.AddRange(buffer.Take(result.Count));
        } while (!result.EndOfMessage);
        busy = false;

        // convert to string
        return Encoding.UTF8.GetString(preResponse.ToArray());

    }

    private async void OnApplicationQuit()
    {
        await Disconnect();
    }

    private void OnDisable()
    {
        ServerAddress.onAddressChange -= OnAddressChange;
    }
}
