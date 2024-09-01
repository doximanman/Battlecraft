using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public static class WorldAPI
{
    public async static Task<WorldData> GetWorldDataObject()
    {
        (bool exists, bool hasData, string dataJson) = await GetWorldData();

        if(!exists || !hasData) return null;

        // get field that includes the actual playerdata
        return WorldSaver.DeserializeWorldData(dataJson);

    }

    public async static Task SaveWorldDataObject(WorldData data)
    {
        string dataJson = WorldSaver.SerializeWorldData(data);

        await SaveWorldData(dataJson);
    }

    public async static Task<(bool userExists,bool hasData, string dataOrMessage)> GetWorldData()
    {
        if (!Login.Current.loggedIn)
            return (false,false, "Not logged in");

        string token = PlayerPrefs.GetString("token");

        JObject request = new()
        {
            ["type"] = "world",
            ["subtype"] = "get",
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

    public async static Task<(bool success, string error)> SaveWorldData(string worldData)
    {
        if (!Login.Current.loggedIn)
            return (false, "Not logged in");

        string token = PlayerPrefs.GetString("token");
        JObject request = new()
        {
            ["type"] = "world",
            ["subtype"] = "save",
            ["data"] = worldData,
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
