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

    float timer = 0;
    private void Update()
    {
        if(health == null)
        {
            health = GetComponent<StatManager>().GetStat(StatManager.Health);
            food = GetComponent<StatManager>().GetStat(StatManager.Food);
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
