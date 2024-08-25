using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class WorldSaver
{
    /// <summary>
    /// save path of the default world
    /// </summary>
    public static string defaultWorldPath = Path.Combine(Application.streamingAssetsPath,"defaults","env.save");

    public static void SaveWorld(InteractableData[] data)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "player1");

        string path = Path.Combine(Application.persistentDataPath,playerName,"env.save");
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        FileStream fs = new(path, FileMode.Create);
        BinaryFormatter bf = new();

        bf.Serialize(fs, data);

        fs.Close();
        
    }

    public static InteractableData[] LoadWorld(bool defaultWorld = false)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "player1");
        string path = Path.Combine(Application.persistentDataPath,playerName,"env.save");
        FileStream fs;
        if (!defaultWorld && File.Exists(path))
        {
            fs = new(path, FileMode.Open);
        }
        else
        {
            fs = new(defaultWorldPath, FileMode.Open);
        }
        BinaryFormatter bf = new();
        InteractableData[] data = bf.Deserialize(fs) as InteractableData[];

        fs.Close();

        return data;
    }
}
