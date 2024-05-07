using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MovementSettings
{
    public float minWaitTime;
    public float maxWaitTime;
    public float minMoveTime;
    public float maxMoveTime;
    public float chanceToGoRight;
    public float xSpeed;
    public float xRunSpeed;
    public float jumpHeight;
    public float jumpChance;
}
