using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Login : MonoBehaviour
{

    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Feedback feedback;


    private void Start()
    {
        // if client is disconnected - not logged in.
        ServerClient.onConnect += async (bool connected) =>
        {
            if (connected)
                // try to restore connection
                await TryLoginWithToken();
            else
                LoginStatus.Current = (false, string.Empty);

        };
    }

    private void OnEnable()
    {
        feedback.Clear();
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
            LoginStatus.Current = (success, username);
        
    }

    /// <summary>
    /// tries to log in with the IN GAME FIELDS
    /// of username and password
    /// </summary>
    public async void TryLogin()
    {
        string username = usernameField.text;
        string password = passwordField.text;

        ClearFields();
        feedback.StartLoading();

        (bool success, string TokenOrMessage) = await UserAPI.Login(username, password);

        if (success)
        {
            PlayerPrefs.SetString("token", TokenOrMessage);
            feedback.SetFeedback($"Welcome {username}!");
            LoginStatus.Current = (true, username);
        }
        else
        {
            LoginStatus.Current = (false, string.Empty);
            feedback.SetFeedback(TokenOrMessage);
        }
    }

    public void ClearFields()
    {
        usernameField.text = string.Empty;
        passwordField.text = string.Empty;
        feedback.SetFeedback(string.Empty);
        feedback.StopLoading();
    }

    private void OnDisable()
    {
        ClearFields();
    }
}
