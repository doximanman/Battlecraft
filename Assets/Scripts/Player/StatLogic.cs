using UnityEngine;

/// <summary>
/// relation between stats of the player
/// </summary>
public class StatLogic : MonoBehaviour
{
    private Stat health;
    private Stat food;

    [SerializeField] private float statUpdateRate;
    private float statUpdateInterval;

    [SerializeField] private float healAboveFood;
    [SerializeField][Min(0)] private float healRate;
    [SerializeField][Min(0)] private float loseFoodRate;

    private void OnValidate()
    {
        statUpdateInterval = 1.0f / statUpdateRate;
    }

    private void OnEnable()
    {
        statUpdateInterval = 1.0f / statUpdateRate;
    }

    bool healthIsLow = false;
    bool foodIsLow = false;
    float timer = 0;
    private void Update()
    {
        if(health == null)
        {
            health = GetComponent<StatManager>().GetStat(StatManager.Health);
            food = GetComponent<StatManager>().GetStat(StatManager.Food);
        }

        // "low" is considered 20% above minimum value
        if (healthIsLow && health.Value > (health.MinValue + 0.2f * health.Range))
            // if health is above 20% above minimum value, health is not low
            healthIsLow = false;
        if (!healthIsLow && health.Value < (health.MinValue + 0.2f * health.Range))
        {
            // if health is below 20% above minimum value, health is low
            healthIsLow = true;
            // notify that the player might be having a bad time
            // logic: if the player goes below 20% hp frequently, perhaps they would like
            // to change the difficulty.
            DifficultyLearner.current.GameIsDifficult();
        }
        // same for food
        if (foodIsLow && food.Value > (food.MinValue + 0.2f * food.Range))
            foodIsLow = false;
        if (!foodIsLow && food.Value < (food.MinValue + 0.2f * food.Range))
        {
            foodIsLow = true;
            DifficultyLearner.current.GameIsDifficult();
        }


        timer += Time.deltaTime;
        if(timer > statUpdateInterval)
        {
            timer = 0;
            // mutltiplier to multiply or divide values by, according to difficulty.
            // when losing a stat - divide by the multiplier. that makes it lose less if the multiplier is low.
            // when gaining a stat - multiply it. that makes it gain more if the multiplier is low.
            // (low means low difficulty).
            float difficultyMultiplier = Settings.current.Difficulty.GetMultiplier();
            // lose the food, and if there is enough food left,
            // regenerate health.
            food.Value -= loseFoodRate / (statUpdateRate* difficultyMultiplier);
            if (food.Value > healAboveFood)
            {
                // lose a bit more food if it was used for healing
                float healthVal = health.Value;
                float newHealthVal = healthVal + difficultyMultiplier * healRate / statUpdateRate;
                if (Mathf.Abs(healthVal - newHealthVal) > 0.00001f)
                {
                    food.Value -= 4 * loseFoodRate / (statUpdateRate* difficultyMultiplier);
                    health.Value += difficultyMultiplier * healRate / statUpdateRate;
                }

            }
        }
    }


}
