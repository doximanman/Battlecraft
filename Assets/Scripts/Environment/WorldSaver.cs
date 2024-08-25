using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class WorldSaver
{
    public static void SaveWorld(InteractableData[] data)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "default");

        string path = Application.persistentDataPath + "/" + playerName + "_env.save";

        FileStream fs = new(path, FileMode.Create);
        BinaryFormatter bf = new();

        bf.Serialize(fs, data);

        fs.Close();
        
    }

    public static InteractableData[] LoadWorld()
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "default");
        string path = Application.persistentDataPath + "/" + playerName + "_env.save";
        FileStream fs = new(path,FileMode.Open);
        BinaryFormatter bf = new();
        InteractableData[] data = bf.Deserialize(fs) as InteractableData[];

        fs.Close();

        return data;
    }
}
