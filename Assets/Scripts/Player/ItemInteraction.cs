using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public float pickupRange;
    public Vector2 maxThrowSpeed;
    public Vector2 minThrowSpeed;

    void Start()
    {
        DroppedStacksManager.instance.RegisterCloseListener(gameObject, pickupRange, (droppedStack) =>
        {
            // try to add to inventory
            int remainder=InventoryLogic.personalInventory.AddItems(droppedStack.Stack.type, droppedStack.Stack.count);
            droppedStack.Stack.count = remainder;
        });

        PlayerControl control=GetComponent<PlayerControl>();
        System.Random rand = new();

        DroppedStacksManager.instance.dropPosition = () => transform.position;
        DroppedStacksManager.instance.dropVelocity = () =>
        {
            Direction facing = control.Facing;
            float xSpeed = GenerateFloat(rand, minThrowSpeed.x, maxThrowSpeed.x);
            if (xSpeed < 0 && facing == Direction.RIGHT
                || xSpeed > 0 && facing == Direction.LEFT)
                xSpeed = -xSpeed;
            return new(xSpeed,GenerateFloat(rand, minThrowSpeed.y, maxThrowSpeed.y));
        };
    }

    private float GenerateFloat(System.Random rand, float min, float max) => min + (max - min) * (float)rand.NextDouble();
}
