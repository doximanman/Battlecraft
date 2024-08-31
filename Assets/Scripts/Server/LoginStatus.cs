using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoginStatus
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
            // if not logged in - remove playername
            // from playerprefs
            if (!current.loggedIn)
                PlayerPrefs.DeleteKey("PlayerName");
            else
                PlayerPrefs.SetString("PlayerName", current.username);
            onLogin?.Invoke(current.loggedIn, current.username);
        }
    }
}
