using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public delegate void OnChange(float oldValue, float newValue);

    private float food;
    public float Food
    {
        get { return food; }
        set
        {
            float oldFood = food;
            food = value < 0 ? 0 : value;
            if(oldFood != food)
            {
                OnFoodChange?.Invoke(oldFood, food);
            }
        }
    }

    public OnChange OnFoodChange;
}
