using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

[Serializable]
public class GameTree : Interactable
{
    private float length;
    private float width;

    public override void Start()
    {
        base.Start();

        // physical length and width
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        length = renderer.bounds.size.y;
        width = renderer.bounds.size.x;

        // axe to chop, chopping time proportional to length
        requiredTool = InteractableConstants.axe;
        chopDuration = InteractableConstants.woodChopTime * length;
    }

    public override void OnFinishChopping()
    {
        // time has passed - break the tree and add wood and sticks to inventory.
        InventoryLogic.personalInventory.AddItems(InteractableConstants.wood,
            Mathf.FloorToInt(length / InteractableConstants.lengthToWood));
        InventoryLogic.personalInventory.AddItems(InteractableConstants.sticks,
            Mathf.FloorToInt(width / InteractableConstants.widthToSticks));

        // destroy the tree world object
        Destroy(gameObject);
    }

}
