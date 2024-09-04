using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Internal;

/// <summary>
/// tracks how difficult the game is for the player.
/// internally saves a number to represent the preceived difficulty,
/// and past or below a certain point asks the player to
/// change the difficulty.
/// </summary>
public class DifficultyLearner : MonoBehaviour
{
    public static DifficultyLearner current;

    [SerializeField] private int difficultyEstimate;
    public int DifficultyEstimate
    {
        get => difficultyEstimate;
        private set
        {
            difficultyEstimate = Math.Clamp(value, lowerValue, upperValue);
            CheckValues();
        }
    }

    [Tooltip("How common to decrement the difficulty estimator")]
    [SerializeField] private float decrementInterval;
    [Tooltip("If above this value, alert the player")]
    [SerializeField] private int upperValue;
    [Tooltip("If below this value, alert the player")]
    [SerializeField] private int lowerValue;

    private void Awake()
    {
        current = this;
        var values = (int[])Enum.GetValues(typeof(DifficultyType));
        minDifficulty = values.Min();
        maxDifficulty = values.Max();
    }
    private float minDifficulty;
    private float maxDifficulty;

    float timer = 0;
    private void Update()
    {
        timer += Time.deltaTime;
        // every 'decrementInterval' seconds, decrement the estimate
        if (timer > decrementInterval)
        {
            timer = 0;
            DifficultyEstimate--;
        }
    }

    public void CheckValues()
    {
        if (difficultyEstimate >= upperValue)
        {
            // reset estimate
            difficultyEstimate = (lowerValue + upperValue) / 2;
            AskToDecreaseDifficulty();
        }
        if (difficultyEstimate <= lowerValue)
        {
            // reset estimate
            difficultyEstimate = (lowerValue + upperValue) / 2;
            AskToIncreaseDifficulty();
        }
    }

    [ContextMenu("Ask To Decrease Difficulty")]
    public void AskToDecreaseDifficulty()
    {
        int currentDifficulty = (int)Settings.current.Difficulty;
        // if already minimum difficulty - don't offer to change it.
        if (currentDifficulty == minDifficulty)
            return;
        MetaLogic.Pause();
        Modal.Ask("Change Difficulty?",
            "You seem to be having some trouble.<br>Would you like to lower the difficulty?",
            "Lower Difficulty",
            "Keep Difficulty",
            () => {
                Settings.current.Difficulty = (DifficultyType)(currentDifficulty - 1);
                MetaLogic.Unpause();
            },
            () => {
                MetaLogic.Unpause();
            });
    }

    [ContextMenu("Ask to increase difficulty")]
    public void AskToIncreaseDifficulty()
    {
        int currentDifficulty = (int)Settings.current.Difficulty;
        // if already maximum difficulty - don't offer to change it.
        if ((int)Settings.current.Difficulty == maxDifficulty)
            return;
        MetaLogic.Pause();
        Modal.Ask("Change Difficulty?",
            "You seem to be having an easy time.<br>Would you like to increase the difficulty?",
            "Increase Difficulty",
            "Keep Difficulty",
            () => {
                Settings.current.Difficulty = (DifficultyType)(currentDifficulty + 1);
                MetaLogic.Unpause();
            },
            () => {
                MetaLogic.Unpause();
            });
    }

    [ContextMenu("Increase difficulty estimate")]
    // for other classes to use to indicate difficulty
    public void GameIsDifficult()
    {
        DifficultyEstimate++;
    }

    [ContextMenu("Decrease difficulty estimate")]
    public void GameIsEasy()
    {
        DifficultyEstimate--;
    }
}
