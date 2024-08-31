using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public static class PlayerAPI
{
    public async static Task<(bool userExists,bool hasData, string dataOrMessage)> GetPlayerData()
    {
        if (!LoginStatus.Current.loggedIn)
            return (false,false, "Not logged in");

        string token = PlayerPrefs.GetString("token");

        JObject request = new()
        {
            ["type"] = "player",
            ["subtype"] = "get_data",
            ["token"] = token
        };

        string responseJson = await ServerClient.Request(request.ToString());

        JObject response;
        try {  response = JObject.Parse(responseJson);}
        catch { return (false, false, responseJson); }

        if ((bool)response["success"])
        {
            string data = response["data"].ToString();
            if (data == string.Empty)
                return (true, false, string.Empty);
            else
                return (true,true, response["data"].ToString());
        }
        else
            return (false,false, response["message"].ToString());
    }

    public async static Task<(bool success, string error)> SavePlayerData(string playerData)
    {
        if (!LoginStatus.Current.loggedIn)
            return (false, "Not logged in");

        string token = PlayerPrefs.GetString("token");
        JObject request = new()
        {
            ["type"] = "player",
            ["subtype"] = "save_data",
            ["data"] = playerData,
            ["token"] = token,
        };

        string responseJson = await ServerClient.Request(request.ToString());

        JObject response;
        try { response = JObject.Parse(responseJson); }
        catch { return (false, responseJson); }

        if (!response.HasValues)
            return (false, responseJson);

        if ((bool)response["success"])
            return (true, string.Empty);
        else
            return (false, response["message"].ToString());
    }
}
