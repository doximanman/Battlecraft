using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Modal : MonoBehaviour
{
    public static Modal instance;

    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text body;
    [SerializeField] private TMP_Text leftButtonText;
    [SerializeField] private TMP_Text rightButtonText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject modalBody;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        background.SetActive(false);
        modalBody.SetActive(false);
    }

    public static void Ask(string title,
                        string body,
                        string leftOption,
                        string rightOption,
                        Action leftClick,
                        Action rightClick)
    {
        instance.AskThis(title,
                        body,
                        leftOption,
                        rightOption,
                        leftClick,
                        rightClick);
    }

    /// <summary>
    /// ask the player a question using a modal.<br/>
    /// the question has two answers, the responses
    /// of each answer is defined by the Action given.
    /// </summary>
    /// <param name="title">title of the modal to show</param>
    /// <param name="body">body text of the modal to show</param>
    /// <param name="leftOption">text to show inside the left button</param>
    /// <param name="rightOption">text to show inside the right button</param>
    /// <param name="leftClick">action to do for the left option</param>
    /// <param name="rightClick">action to do for the right option</param>
    private void AskThis(string title,
                        string body,
                        string leftOption,
                        string rightOption,
                        Action leftClick,
                        Action rightClick)
    {
        background.SetActive(true);
        modalBody.SetActive(true);
        this.title.text = title;
        this.body.text = body;
        leftButtonText.text = leftOption;
        rightButtonText.text = rightOption;
        leftButton.onClick.AddListener(()=>{
            leftClick();
            background.SetActive(false);
            modalBody.SetActive(false);
        });
        rightButton.onClick.AddListener(()=>{
            rightClick();
            background.SetActive(false);
            modalBody.SetActive(false);
        });
    }

}
