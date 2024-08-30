using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class ServerClient
{
    private static string ip;
    private static int port;

    public static string ConnectionString(string ip, int port) => $"ws://{ip}:{port}";

    public static void SyncAddressWithLocal()
    {
        var savedAddress = ServerAddress.GetAddress();

        if(ip != savedAddress.ip || port != savedAddress.port)
        {
            ip = savedAddress.ip;
            port = savedAddress.port;
        }
    }

    /// <summary>
    /// One request, with one response.<br/>
    /// If different ip or port are provided, it will use
    /// the given ip and port.<br/>
    /// meaning: if one is provided, the other must too.
    /// </summary>
    /// <param name="request">json of the request</param>
    /// <param name="ip">use a different ip than the saved one</param>
    /// <param name="port">use a different port than the saved one</param>
    /// <returns>json of the response</returns>
    public async static Task<string> Request(string request, string ip ="", int port = -1)
    {
        string connectionString;
        if (ip != "")
            connectionString = ConnectionString(ip, port);
        else
            connectionString = ConnectionString(ServerClient.ip, ServerClient.port);


        ClientWebSocket client = new();
        Uri serverUri;
        try { serverUri = new(connectionString); }
        catch { return "Invalid server address"; }

        // connect to server
        try { await client.ConnectAsync(serverUri, CancellationToken.None); }
        catch { return "Couldn't connect to server"; }
        // send json as byte array
        byte[] buffer = Encoding.UTF8.GetBytes(request);
        // 'true' means this is the last message
        try { await client.SendAsync(new(buffer), WebSocketMessageType.Text, true, CancellationToken.None); }
        catch { return "Couldn't send to server"; }

        List<byte> preResponse = new();
        // reuse the same buffer (we don't need it anymore)
        buffer = new byte[1024];
        WebSocketReceiveResult result;
        do
        {
            try { result = await client.ReceiveAsync(new(buffer), CancellationToken.None); }
            catch { return "Couldn't receive from server"; }
            preResponse.AddRange(buffer[0..result.Count]);
        } while (!result.EndOfMessage);

        await client.CloseAsync(WebSocketCloseStatus.NormalClosure,"response received",CancellationToken.None);

        // convert to string
        string response = Encoding.UTF8.GetString(preResponse.ToArray());



        return response;

    }
}
