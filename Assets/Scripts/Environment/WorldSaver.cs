using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class WorldSaver
{
    // cannot serialize arrays
    public class InteractableDataList
    {
        public InteractableData[] data;

        public InteractableDataList(InteractableData[] data)
        {
            this.data = data;
        }
    }

    /// <summary>
    /// save path of the default world
    /// </summary>
    public static string defaultWorldPath = Path.Combine(Application.streamingAssetsPath,"defaults","env.save");

    public static void SaveWorld(InteractableData[] data, bool defaultWorld = false)
    {
        InteractableDataList dataList= new(data);

        // player name is saved as playerprefs
        string playerName = PlayerPrefs.GetString("PlayerName", "player1");

        string path;

        // path is the default world path
        // or inside the player folder at env.save
        if (!defaultWorld)
            path = Path.Combine(Application.persistentDataPath, playerName, "env.save");
        else
            path = defaultWorldPath;

        // create player folder if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        // write the data into json
        string json = JsonUtility.ToJson(dataList);

        // write the json into the file
        File.WriteAllText(path, json);
    }

    public static InteractableData[] LoadWorld(bool defaultWorld = false)
    {
        // same as save

        string playerName = PlayerPrefs.GetString("PlayerName", "player1");

        string path = Path.Combine(Application.persistentDataPath, playerName, "env.save");

        if (defaultWorld || !File.Exists(path))
            path = defaultWorldPath;

        // read json test from file
        string json = File.ReadAllText(path);

        // convert json to object
        InteractableData[] data = JsonUtility.FromJson<InteractableDataList>(json).data;
        return data;

    }

}
