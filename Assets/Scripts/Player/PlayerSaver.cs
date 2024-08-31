using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

public static class PlayerSaver
{
    public static string defaultPlayerPath = Path.Combine(Application.streamingAssetsPath, "defaults", "player.save");

    public static void SavePlayerDataPlain(string playerData, bool defaultPlayer = false)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "player1");

        // path is /playername/player.save
        // or /defaults/player.save as the default player
        string path = Path.Combine(Application.persistentDataPath, playerName, "player.save");

        if (defaultPlayer)
            path = defaultPlayerPath;

        // create the folders in the path if it doesnt exist
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        // write the json of the data
        File.WriteAllText(path, playerData);
    }

    public static string LoadPlayerDataPlain(bool defaultPlayer = false)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "player1");

        // path is /playername/player.save
        // or is /defaults/player.save as the default player, OR
        // if the player doesnt have data saved
        string path = Path.Combine(Application.persistentDataPath, playerName, "player.save");

        if (defaultPlayer || !File.Exists(path))
            path = defaultPlayerPath;

        return File.ReadAllText(path);
    }

    // asyncronous versions
    // only difference is the file operation
    public static async Task SavePlayerDataPlainAsync(string playerData, bool defaultPlayer = false)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "player1");
        string path = Path.Combine(Application.persistentDataPath, playerName, "player.save");
        if(defaultPlayer)
            path = defaultPlayerPath;
        // create directory asyncronously
        await Task.Run(() => { Directory.CreateDirectory(Path.GetDirectoryName(path)); });
        await File.WriteAllTextAsync(path, playerData);
    }

    public static async Task<string> LoadPlayerDataPlainAsync(bool defaultPlayer = false)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "player1");
        string path = Path.Combine(Application.persistentDataPath, playerName, "player.save");
        if(defaultPlayer || !File.Exists(path))
            path = defaultPlayerPath;

        return await File.ReadAllTextAsync(path);
    }

    public static void SavePlayerDataObject(PlayerData data,bool defaultPlayer = false)
    {
        string jsonText = JsonUtility.ToJson(data);
        // add timestamp to object
        JObject json = new()
        {
            ["data"] = jsonText,
            ["timestamp"] = DateTime.UtcNow.ToString("o")
        };
        SavePlayerDataPlain(json.ToString(),defaultPlayer);
    }

    public static PlayerData LoadPlayerDataObject(bool defaultPlayer = false)
    {
        string jsonText = LoadPlayerDataPlain(defaultPlayer);
        // remove timestamp from object
        JObject jsonWithTime = JObject.Parse(jsonText);
        string dataJson = jsonWithTime["data"].ToString();

        return JsonUtility.FromJson<PlayerData>(dataJson);
    }
}
