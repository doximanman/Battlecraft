using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;


public static class ServerAPI
{
    public async static Task<(bool success, string errorMessage)> VerifyServer()
    {
        // send ping request (defined by API) to server
        JObject request = new()
        {
            ["type"] = "ping"
        };
        string responseJson = await ServerClient.Request(request.ToString());

        // if not json - probably an error.
        JObject response;
        try { response = JObject.Parse(responseJson); }
        catch { return (false,responseJson); }

        // according to api, response should be {success: true}.
        if (!response.ContainsKey("success"))
            return (false, "Invalid server");

        try
        {
            if ((bool)response["success"])
                // server returned success true
                return (true, string.Empty);
            else
                // server returned success false,
                // it can't handle the connection right now.
                return (false, "Invalid server");
        }
        catch
        {
            // 'success' field isn't boolean - invalid response.
            return (false, "Invalid server");
        }
    } 
}
