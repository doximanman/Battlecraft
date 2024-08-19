using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelMeter : MonoBehaviour
{
    [SerializeField] private VisualSlider fuelSlider;

    public float minFuel;
    public float maxFuel;

    [SerializeField] private float fuel;

    public float Fuel
    {
        get => fuel;
        set {
            fuel = Mathf.Clamp(value, minFuel, maxFuel);
            fuelSlider.Values = (1,fuel);
        }
    }

    public float Remainder => maxFuel - fuel;

    private void Start()
    {
        fuel=0;
        fuelSlider.ValueLimits = ((0, 1), (minFuel, maxFuel));
        fuelSlider.Values = (1, minFuel);
    }

    
}
