using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableConstants : MonoBehaviour
{
    public static float lengthToSticks;
    public static float lengthToWood;
    public static float widthToSticks;
    public static float woodChopTime;
    public static float bushChopTime;
    public static ItemType wood;
    public static ItemType sticks;
    public static ItemType axe;
    public static float maxInteractDistance;

    [SerializeField] private float _lengthToSticks;
    [SerializeField] private float _lengthToWood;
    [SerializeField] private float _widthToSticks;
    [SerializeField] private float _woodChopTime;
    [SerializeField] private float _bushChopTime;
    [SerializeField] private ItemType _wood;
    [SerializeField] private ItemType _sticks;
    [SerializeField] private ItemType _axe;
    [SerializeField] private float _maxInteractDistance;

    private void Start()
    {
        lengthToSticks = _lengthToSticks;
        lengthToWood = _lengthToWood;
        widthToSticks = _widthToSticks;
        woodChopTime=_woodChopTime;
        bushChopTime=_bushChopTime;
        wood = _wood;
        sticks= _sticks;
        axe = _axe;
        maxInteractDistance = _maxInteractDistance;
    }
}
