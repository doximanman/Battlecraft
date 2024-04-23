using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableConstants : MonoBehaviour
{
    public static float lengthToWood;
    public static float widthToSticks;
    public static float chopTime;
    public static ItemType wood;
    public static ItemType sticks;
    public static ItemType axe;
    public static float maxInteractDistance;
    

    [SerializeField] private float _lengthToWood;
    [SerializeField] private float _widthToSticks;
    [SerializeField] private float _chopTime;
    [SerializeField] private ItemType _wood;
    [SerializeField] private ItemType _sticks;
    [SerializeField] private ItemType _axe;
    [SerializeField] private float _maxInteractDistance;

    private void Start()
    {
        lengthToWood = _lengthToWood;
        widthToSticks = _widthToSticks;
        chopTime = _chopTime;
        wood = _wood;
        sticks= _sticks;
        maxInteractDistance = _maxInteractDistance;
        axe = _axe;
    }
}
