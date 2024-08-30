using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Net.WebSockets;
using UnityEngine;

public class Register : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_InputField passwordConfirmField;
    [SerializeField] private TMP_Text errorField;

    public async void TryRegister()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        string confirm = passwordConfirmField.text;

        ClearFields();

        if(password != confirm)
        {
            errorField.text = "Passwords don't match";
        }
        else
        {
            //Debug.Log("Register with: " + username + " , " + password);

            (bool success,string usernameOrMessage) = await UserAPI.Register(username, password);
            if (success)
                errorField.text = "Successfully registered!";
            else
                errorField.text = usernameOrMessage;
        }
    }

    public void ClearFields()
    {
        usernameField.text = "";
        passwordField.text = "";
        passwordConfirmField.text = "";
        errorField.text = "";
    }

    private void OnDisable()
    {
        ClearFields();
    }
}
