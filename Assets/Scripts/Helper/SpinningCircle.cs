using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningCircle : MonoBehaviour
{
    public float rotateFrequency;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    // Update is called once per frame
    void Update()
    {
        // spin the circle
        rectTransform.Rotate(0,0, -rotateFrequency * Time.deltaTime * 360f);
    }
}
