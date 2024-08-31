

using System;
using System.IO;
using UnityEngine;

public static class EntitySaver
{
    public static string defaultEntitiesPath = Path.Combine(Application.streamingAssetsPath, "defaults", "ent.save");

    [Serializable]
    public class EntityDataList
    {
        public EntityData[] list;
        public EntityDataList(EntityData[] list)
        {
            this.list = list;
        }
    }

    public static void SaveEntities(EntityData[] data, bool defaultEntities = false)
    {
        EntityDataList dataList = new(data);

        // player name is saved as playerprefs
        string playerName = PlayerPrefs.GetString("PlayerName", "player1");

        string path;

        // path is the default world path
        // or inside the player folder at env.save
        if (!defaultEntities)
            path = Path.Combine(Application.persistentDataPath, playerName, "ent.save");
        else
            path = defaultEntitiesPath;

        // create player folder if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(path));

        // write the data into json
        string json = JsonUtility.ToJson(dataList);

        // write the json into the file
        File.WriteAllText(path, json);
    }

    public static EntityData[] LoadEntities(bool defaultEntities = false)
    {
        // same as save

        string playerName = PlayerPrefs.GetString("PlayerName", "player1");

        string path = Path.Combine(Application.persistentDataPath, playerName, "ent.save");

        if (defaultEntities || !File.Exists(path))
            path = defaultEntitiesPath;

        // read json test from file
        string json = File.ReadAllText(path);

        // convert json to object
        EntityData[] data = JsonUtility.FromJson<EntityDataList>(json).list;
        return data;

    }
}
