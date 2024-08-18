using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class DroppedStacksManager : MonoBehaviour
{
    [SerializeField] GameObject droppedStackPrefab;

    public float combineStacksDistance;
    public float pickupDelay;
    public Func<Vector2> dropPosition = null;
    public Func<Vector2> dropVelocity=null;

    [InspectorName("Instance")]
    [SerializeField] private DroppedStacksManager _instance;
    [SerializeReference] public static DroppedStacksManager instance;

    private void OnValidate()
    {
        instance = _instance;
    }

    void FixedUpdate()
    {

        // iterate children, each child is a droppedstack.
        // calculate distances from the droppedstack to 
        // all the listening game object, and notify them.
        foreach (Transform child in transform)
            foreach (var (gameObject, actDist, onCloseTo) in closeListeners)
            {
                // destroyed objects become null
                if (gameObject == null || child == null || gameObject == child.gameObject) continue;
                if (Vector2.Distance(gameObject.transform.position, child.transform.position) < actDist)
                {
                    DroppedStack droppedStack = child.GetComponent<DroppedStack>();
                    if (droppedStack.pickupDelay > 0)
                        continue;
                    onCloseTo(droppedStack);
                    if (droppedStack.Stack.count == 0)
                        Remove(droppedStack);
                }
            }

        // Remove null gameObjects.
        // Could have been removed.
        closeListeners.RemoveAll(listener => listener.Item1 == null);

    }

    public void Drop(StackData stack)
    {
        DroppedStack droppedStack = Instantiate(droppedStackPrefab, transform).GetComponent<DroppedStack>();
        droppedStack.transform.position = dropPosition();
        if (dropVelocity != null)
            droppedStack.GetComponent<Rigidbody2D>().velocity = dropVelocity();
        droppedStack.Stack = stack;
        // after dropping a stack, wait for pickupTime seconds 
        // before it is able to be picked up.
        droppedStack.pickupDelay = pickupDelay;
    }

    /// <summary>
    /// Remove a dropped stack
    /// </summary>
    /// <param name="droppedStack">Dropped stack to remove</param>
    public void Remove(DroppedStack droppedStack)
    {
        // iterate through children and find the given stack.
        GameObject toRemove = null;
        foreach (Transform child in transform)
            if (child.GetComponent<DroppedStack>() == droppedStack)
            {
                toRemove = child.gameObject;
                child.SetParent(null);
            }

        // remove it if found.
        if (toRemove)
            DestroyImmediate(toRemove);
    }

    // each gameobject provides a distance and a function.
    // when the gameobject is closer than the provided distance,
    // the provided function will activate.
    private readonly List<(GameObject, float, Action<DroppedStack>)> closeListeners = new();
    /// <summary>
    /// At a given distance to a dropped stack, get notified of the stack.
    /// <br />
    /// IMPORTANT: Listener's responsibility to update the count of the stack.
    /// </summary>
    /// <param name="listener">Object that needs to be close to the stack</param>
    /// <param name="actuationDistance">Distance below which notify</param>
    /// <param name="onCloseTo">Listener function that gets the stack on the ground.</param>
    public void RegisterCloseListener(GameObject listener, float actuationDistance, Action<DroppedStack> onCloseTo)
    {
        closeListeners.Add((listener, actuationDistance, onCloseTo));
    }
}
