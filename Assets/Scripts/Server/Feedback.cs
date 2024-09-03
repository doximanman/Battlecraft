using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Feedback : MonoBehaviour
{
    [SerializeField] private GameObject spinningCircle;
    [SerializeField] private TMP_Text feedbackText;

    public void Clear()
    {
        spinningCircle.SetActive(false);
        feedbackText.text = string.Empty;
    }

    public void StartLoading()
    {
        spinningCircle.SetActive(true);
        feedbackText.text = string.Empty;
    }

    public void StopLoading()
    {
        spinningCircle.SetActive(false);
    }

    public void SetFeedback(string text)
    {
        feedbackText.text = text;
        if (text != string.Empty)
        {
            spinningCircle.SetActive(false);
        }
    }
}
