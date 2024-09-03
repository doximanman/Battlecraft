using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoginTab : MonoBehaviour
{

    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Feedback feedback;

    private void OnEnable()
    {
        feedback.Clear();
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
            Login.Current = (true, username);
        }
        else
        {
            Login.Current = (false, string.Empty);
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
