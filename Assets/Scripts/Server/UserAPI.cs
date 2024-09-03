using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


public static class UserAPI
{ 
    public async static Task<(bool success, string error)> RestoreLogin(string token)
    {
        JObject request = new()
        {
            ["type"] = "user",
            ["subtype"] = "login_with_token",
            ["token"] = token
        };

        string responseJson = await ServerClient.Request(request.ToString());

        JObject response;
        try { response = JObject.Parse(responseJson); }
        catch { return (false,responseJson); }

        if ((bool)response["success"])
            return (true, response["username"].ToString());
        else
            return (false, response["message"].ToString());

    }

    public async static Task<(bool success, string TokenOrMessage)> Login(string username,string password)
    {
        JObject request = new()
        {
            ["type"] = "user",
            ["subtype"] = "login",
            ["username"] = username,
            ["password"] = password
        };

        string responseJson = await ServerClient.Request(request.ToString());

        // if invalid json - probably means error

        JObject response;
        try { response = JObject.Parse(responseJson); }
        catch { return (false,responseJson); }


        
        if ((bool)response["success"])
            return (true, response["token"].ToString());
        else
            return (false, response["message"].ToString());
    }

    public async static Task<(bool success, string usernameOrMessage)> Register(string username,string password)
    {
        JObject request = new()
        {
            ["type"] = "user",
            ["subtype"] = "register",
            ["username"] = username,
            ["password"] = password
        };

        string responseJson = await ServerClient.Request(request.ToString());

        // if invalid json - probably means error

        JObject response;
        try { response = JObject.Parse(responseJson); }
        catch { return (false, responseJson); }

        if ((bool)response["success"])
            return (true, response["username"].ToString());
        else
            return (false, response["message"].ToString());
    }
}
