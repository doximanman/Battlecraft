using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Login : MonoBehaviour
{
    public delegate void OnLogin(bool loggedIn, string username);

    public static OnLogin onLogin;

    private static (bool loggedIn, string username) current = (false,"");
    public static (bool loggedIn, string username) Current
    {
        get => current;
        set
        {
            if (current == value) return;
            current = value;
            // if logged in - change username
            if (current.loggedIn)
                PlayerPrefs.SetString("PlayerName", current.username);
            onLogin?.Invoke(current.loggedIn, current.username);
        }
    }

    private void OnEnable()
    {

        // if client is disconnected - not logged in.
        ServerClient.onConnect += OnConnect;
    }

    public async void OnConnect(bool connected)
    {
        if (connected)
            // try to restore connection
            await TryLoginWithToken();
        else
            Current = (false, string.Empty);
    }

    /// <summary>
    /// tries to login with SAVED TOKEN.<br/>
    /// note: no return value. Listen to onLogin
    /// to get notified of login changes.<br/>
    /// Does not update in game fields and feedback messages.
    /// </summary>
    public async static Task TryLoginWithToken()
    {
        if (!PlayerPrefs.HasKey("token"))
            return;

        string token = PlayerPrefs.GetString("token");
        (bool success, string username) = await UserAPI.RestoreLogin(token);
        if (success)
            Current = (success, username);

    }

    private void OnDisable()
    {
        ServerClient.onConnect -= OnConnect;
    }
}
