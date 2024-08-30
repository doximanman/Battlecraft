using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class ServerClient
{
    public delegate void OnConnect(bool connected);

    private static string ip;
    private static int port;
    public static OnConnect onConnect;

    public static string ConnectionString => $"ws://{ip}:{port}";

    public static void SyncAddressWithLocal()
    {
        var savedAddress = ServerAddress.GetAddress();

        if(ip != savedAddress.ip || port != savedAddress.port)
        {
            ip = savedAddress.ip;
            port = savedAddress.port;
        }
    }

    private static bool connected;
    private static bool Connected
    {
        get => connected;
        set {
            if (connected == value) return;

            connected = value;
            onConnect?.Invoke(connected);
        }
    }
    private static ClientWebSocket client;
    public async static Task<(bool success, string errorMessage)> Connect()
    {
        client = new ClientWebSocket();
        Uri serverUri;
        try { serverUri = new(ConnectionString); }
        catch { return (false,"Invalid server address"); }

        try { await client.ConnectAsync(serverUri, CancellationToken.None); }
        catch { return (false,"Couldn't connect to server"); }

        Connected = true;
        return (true,string.Empty);
    }

    public async static Task<string> Disconnect()
    {
        if (!Connected)
            return "Not connected";

        try {
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "request to disconnect", CancellationToken.None);
        }
        catch {
            if (client.State != WebSocketState.Open)
                Connected = false;
            return "Error while disconnecting";
        }

        Connected = false;
        return "Disconnected";
    }

    /// <summary>
    /// Send a request using THE CONNECTED client.<br/>
    /// (must be connected. use Connect function.)
    /// </summary>
    /// <param name="request">json of the request</param>
    /// <returns>json of the response</returns>
    public async static Task<string> Request(string request)
    {
        if (client == null)
            return "Not connected";

        // send json as byte array
        byte[] buffer = Encoding.UTF8.GetBytes(request);
        // 'true' means this is the last message
        try { await client.SendAsync(new(buffer), WebSocketMessageType.Text, true, CancellationToken.None); }
        catch {
            // notify of possible disconnect
            if (client.State != WebSocketState.Open)
                Connected = false;
            return "Couldn't send to server";
        }

        // read until end of message
        List<byte> preResponse = new();
        // reuse the same buffer
        buffer = new byte[1024];
        WebSocketReceiveResult result;
        do
        {
            try { result = await client.ReceiveAsync(new(buffer), CancellationToken.None); }
            catch {
                if(client.State != WebSocketState.Open)
                    Connected = false;
                return "Couldn't receive from server";
            }
            preResponse.AddRange(buffer[0..result.Count]);
        } while (!result.EndOfMessage);

        // convert to string
        return Encoding.UTF8.GetString(preResponse.ToArray());

    }


}
