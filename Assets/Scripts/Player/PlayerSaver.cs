using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class PlayerSaver
{
    public static string defaultPlayerPath = Path.Combine(Application.streamingAssetsPath, "defaults", "player.save");

    public static void SavePlayer(PlayerData data,bool defaultPlayer = false)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "player1");

        // path is /playername/player.save
        // or /defaults/player.save as the default player
        string path = Path.Combine(Application.persistentDataPath, playerName,"player.save");

        if (defaultPlayer)
            path = defaultPlayerPath;

        // create the folders in the path if it doesnt exist
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        // write the json of the data
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    public static PlayerData LoadPlayer(bool defaultPlayer = false)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "player1");

        // path is /playername/player.save
        // or is /defaults/player.save as the default player, OR
        // if the player doesnt have data saved
        string path = Path.Combine(Application.persistentDataPath, playerName, "player.save");

        if(defaultPlayer || !File.Exists(path))
            path = defaultPlayerPath;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<PlayerData>(json);
    }
}
