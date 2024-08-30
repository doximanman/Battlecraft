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
    [SerializeField] private Feedback feedback;

    private void OnEnable()
    {
        feedback.Clear();
    }

    public async void TryRegister()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        string confirm = passwordConfirmField.text;

        ClearFields();
        feedback.StartLoading();

        if(password != confirm)
        {
            feedback.SetFeedback("Passwords don't match");
        }
        else
        {
            //Debug.Log("Register with: " + username + " , " + password);

            (bool success,string usernameOrMessage) = await UserAPI.Register(username, password);
            if (success)
                feedback.SetFeedback("Successfully registered!");
            else
                feedback.SetFeedback(usernameOrMessage);
        }
    }

    public void ClearFields()
    {
        usernameField.text = "";
        passwordField.text = "";
        passwordConfirmField.text = "";
        feedback.SetFeedback("");
    }

    private void OnDisable()
    {
        ClearFields();
    }
}
