using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class DifficultyDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown difficultyDropdown;

    private DifficultyType difficulty;
    public DifficultyType Difficulty
    {
        get => difficulty;
        set
        {
            difficulty = value;
            difficultyDropdown.@value = (int)value;
        }
    }

    private void Awake()
    {
        string[] values = Enum.GetNames(typeof(DifficultyType));
        // map every enum value string to a new dropdown option
        List<TMP_Dropdown.OptionData> options = new(values.Select(
            value => new TMP_Dropdown.OptionData(CapitalizeFirst(value))
        ));

        difficultyDropdown.options = options;

        difficultyDropdown.onValueChanged.AddListener((value) => listeners?.Invoke((DifficultyType)value));
    }

    Action<DifficultyType> listeners;
    public void AddValueChangeListener(Action<DifficultyType> onChange)
    {
        listeners += onChange;
    }

    public void RemoveValueChangeListener(Action<DifficultyType> onChange)
    {
        listeners -= onChange;
    }

    public void SetValueWithoutNotify(DifficultyType value)
    {
        difficultyDropdown.SetValueWithoutNotify((int)value);
    }

    /// <summary>
    /// receive any word with capitalization
    /// and return the same word, with all the letters lowered,
    /// with the first letter capitalized.
    /// </summary>
    private string CapitalizeFirst(string word)
    {
        if (word == string.Empty)
           return word;

        char[] newWord = word.ToLower().ToCharArray();
        newWord[0] = char.ToUpper(word[0]);
        return new string(newWord);
    }
}
