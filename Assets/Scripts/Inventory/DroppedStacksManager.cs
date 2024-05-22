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
    public Func<Vector2> dropPosition;

    [InspectorName("Instance")]
    [SerializeField] private DroppedStacksManager _instance;
    [SerializeReference] public static DroppedStacksManager instance;

    private void OnValidate()
    {
        instance = _instance;
    }

    (GameObject, float, Action<DroppedStack>)[] keys = { };
    void FixedUpdate()
    {
        if (keys.Length < closeListeners.Keys.Count)
            keys=new (GameObject, float, Action<DroppedStack>)[closeListeners.Keys.Count];
        closeListeners.Keys.CopyTo(keys,0);
        foreach (Transform child in transform)
            foreach (var (gameObject, actDist, onCloseTo) in keys)
            {
                // destroyed objects become null
                if (gameObject==child.gameObject || gameObject == null || child.gameObject==null) continue;
                if (Vector2.Distance(gameObject.transform.position, child.transform.position) < actDist)
                {
                    closeListeners[(gameObject, actDist, onCloseTo)] += Time.fixedDeltaTime;
                    if (closeListeners[(gameObject, actDist, onCloseTo)] > pickupDelay)
                    {
                        DroppedStack droppedStack = child.GetComponent<DroppedStack>();
                        onCloseTo(droppedStack);
                        if (droppedStack.Stack.count == 0)
                            Remove(droppedStack);
                    }
                }
                else
                    closeListeners[(gameObject, actDist, onCloseTo)] = 0;
            }

        // Remove null gameObjects.
        // Could have been removed.
        foreach(var listener in keys)
        {
            if (listener.Item1 == null)
                closeListeners.Remove(listener);
        }
    }

    public void Drop(StackData stack)
    {
        DroppedStack droppedStack=Instantiate(droppedStackPrefab, transform).GetComponent<DroppedStack>();
        droppedStack.transform.position = dropPosition();
        droppedStack.Stack = stack;
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
            Destroy(toRemove);
    }

    // each gameobject provides a distance and a function.
    // when the gameobject is closer than the provided distance,
    // the provided function will activate.
    private readonly Dictionary<(GameObject, float, Action<DroppedStack>),float> closeListeners = new();
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
        closeListeners.Add((listener, actuationDistance, onCloseTo),0);
    }
}
