using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// class with an instance so that unity can
/// use the 
/// </summary>
public class ServerSyncer : MonoBehaviour
{
    /// <summary>
    /// compare saves on the server
    /// to the current saves
    /// </summary>
    /// <returns></returns>
    public async Task CompareSaves()
    {
        // get local save
        string localDataWithTime = await PlayerSaver.LoadPlayerDataPlainAsync();
        JObject localSave = JObject.Parse(localDataWithTime);
        // save to server function
        async void Save() => await PlayerAPI.SavePlayerData(localDataWithTime);

        // try to get server save
        (bool exists,bool hasData, string serverDataWithTime) = await PlayerAPI.GetPlayerData();
        if (!exists)
        {
            Debug.Log(serverDataWithTime);
            return;
        }
        if (!hasData)
        {
            // player doesn't have any data saved on the server - save the local data on the server
            Save();
            return;
        }
        JObject serverSave = JObject.Parse(serverDataWithTime);
        // save to local function
        async void Import() => await PlayerSaver.SavePlayerDataPlainAsync(serverDataWithTime);

        DateTime serverTimestamp = DateTime.Parse(serverSave["timestamp"].ToString());
        DateTime localTimestamp = DateTime.Parse(localSave["timestamp"].ToString());

        if (serverTimestamp == localTimestamp)
            // timestamps are equal - probably the same save.
            // don't do anything.
            return;
        
        // one of them is more recent - ask the player which one to keep.

        // prepare arguments for modal
        string modalTitle = "Sync with server";
        string modalBody;
        string importText = "Import";
        string saveText = "Save To Server";

        if (serverTimestamp > localTimestamp)
        {
            modalBody = "Server has a more recent save then the local machine, would you like to import it?";
            Modal.Ask(modalTitle,
                modalBody,
                importText,
                saveText,
                Import,
                Save);
        }
        else if (serverTimestamp < localTimestamp)
        {
            modalBody = "Local machine has a more recent save than the server, would you like to save it on the server?";
            Modal.Ask(modalTitle,
                modalBody,
                saveText,
                importText,
                Save,
                Import);
        }



    }

    private void Start()
    {
        LoginStatus.onLogin += async (bool login, string username) =>
        {
            if (login)
            {
                // logged in - now compare to server
                await CompareSaves();
            }
        };
    }
}
