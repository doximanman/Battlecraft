using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;


public static class ServerAPI
{
    public async static Task<(bool success, string errorMessage)> VerifyServer(string ip, int port)
    {
        JObject request = new()
        {
            ["type"] = "ping"
        };

        string responseJson = await ServerClient.Request(request.ToString(),ip,port);

        JObject response;
        try { response = JObject.Parse(responseJson); }
        catch { return (false,responseJson); }

        if ((bool)response["success"])
            return (true, string.Empty);
        else
            // shouldn't happen!
            return (false, "Server unavailable");
    } 
}
