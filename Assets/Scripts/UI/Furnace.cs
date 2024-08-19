using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    [SerializeField] private VisualSlider fuelSlider;

    [SerializeField] private float minFuel;
    [SerializeField] private float maxFuel;

    private void Start()
    {
        fuelSlider.ValueLimits = ((0, 1), (minFuel, maxFuel));
        fuelSlider.Values = (1, minFuel);

        
    }

}
