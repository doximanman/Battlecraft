using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelMeter : MonoBehaviour
{
    [SerializeField] private VisualSlider fuelSlider;

    private float minFuel;
    private float maxFuel;
    private float fuel;

    public float Fuel
    {
        get => fuel;
        set {
            fuel = Mathf.Clamp(value, minFuel, maxFuel);
            fuelSlider.Values = (1,fuel);
        }
    }

    public float MinFuel
    {
        get => minFuel;
        set
        {
            // sets the minimum fuel and updates the current fuel if necessary
            minFuel = value;
            Fuel = fuel;
        }
    }

    public float MaxFuel
    {
        get => maxFuel;
        set
        {
            // sets the maximum fuel and updates the current fuel if necessary
            maxFuel = value;
            Fuel = fuel;
        }
    }

    public float Remainder => maxFuel - fuel;



    private void Start()
    {
        // values should have already been set by the enabling object
        fuelSlider.ValueLimits = ((0, 1), (minFuel, maxFuel));
        fuelSlider.Values = (1, fuel);
    }

    
}
