using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Login : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Feedback feedback;
    public async void TryLogin()
    {
        string username = usernameField.text;
        string password = passwordField.text;

        ClearFields();
        feedback.StartLoading();

        (bool success, string JWTOrMessage) = await UserAPI.Login(username, password);

        if (success)
        {
            PlayerPrefs.SetString("JWT", JWTOrMessage);
            feedback.SetFeedback($"Welcome {username}!");
        }
        else
        {
            feedback.SetFeedback(JWTOrMessage);
        }
    }

    public void ClearFields()
    {
        usernameField.text = "";
        passwordField.text = "";
        feedback.SetFeedback(string.Empty);
    }

    private void OnDisable()
    {
        ClearFields();
    }
}
