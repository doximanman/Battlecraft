using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IHitListener : MonoBehaviour
{
    public abstract void OnHit(ItemType hitWith);
}
