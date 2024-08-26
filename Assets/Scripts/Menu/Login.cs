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
    [SerializeField] private TMP_Text errorField;
    public void TryLogin()
    {
        string username = usernameField.text;
        string password = passwordField.text;

        ClearFields();

        if (username == "hello")
            errorField.text = "Welcome!!";

        Debug.Log("Login with: " + username + " , " + password);
    }

    public void ClearFields()
    {
        usernameField.text = "";
        passwordField.text = "";
        errorField.text = "";
    }

    private void OnDisable()
    {
        ClearFields();
    }
}