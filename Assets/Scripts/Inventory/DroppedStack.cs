using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class DroppedStack : MonoBehaviour
{

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider2D;

    [SerializeField] private float borderSubtract;

    [SerializeField] private StackData stack;
    public StackData Stack
    {
        get { return stack; }
        set
        {
            if (stack == null || stack.Equals(null))
                spriteRenderer.sprite = null;
            else
            {
                spriteRenderer.sprite = stack.type.icon;
                boxCollider2D.size = spriteRenderer.size - borderSubtract*Vector2.one;
                stack = value;
            }
        }
    }

    private void Start()
    {
        Stack = stack;


        DroppedStacksManager manager = DroppedStacksManager.instance;
        // if below a certain distance from a stack of the same type, combine with it.
        manager.RegisterCloseListener(gameObject, manager.combineStacksDistance, (droppedStack) =>
        {
            StackData otherStack = droppedStack.Stack;
            // same type and both stacks have less than maxStack items
            // (otherwise two stacks would swap items infinitely if one has maxStack)
            if (stack.type.Equals(otherStack.type)
                && stack.count < stack.type.maxStack && otherStack.count < stack.type.maxStack)
            {
                int combined=stack.count + otherStack.count;
                if(combined < stack.type.maxStack)
                {
                    // combined count fits in one stack - combine into this stack.
                    // Set the other count to 0.
                    // (should be removed by the manager).
                    stack.count = combined;
                    otherStack.count = 0;
                }
                else
                {
                    // combined count doesn't fit in one stack - 
                    // add the maximum count to this stack and
                    // set the other's count to be the remainder.
                    int remainder = combined - stack.type.maxStack;
                    stack.count = stack.type.maxStack;
                    otherStack.count = remainder;
                }
            }
        });
    }
    
    // update item when changed via inspector
    private bool update = false;
    private void OnValidate() => update = true;

    public float pickupDelay;
    private void Update()
    {
        if (update) Stack = stack;

        if (pickupDelay > 0)
            pickupDelay = pickupDelay < Time.deltaTime ? 0 : pickupDelay - Time.deltaTime;
    }
}
