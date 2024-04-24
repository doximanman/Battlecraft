using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;


public class GameTree : Interactable
{
    private float length;
    private float width;

    protected override void Start()
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

    protected override void OnFinishChopping()
    {
        // time has passed - break the tree and add wood and sticks to inventory.
        MetaLogic.personalInventory.AddItems(InteractableConstants.wood,
            Mathf.FloorToInt(length / InteractableConstants.lengthToWood));
        MetaLogic.personalInventory.AddItems(InteractableConstants.sticks,
            Mathf.FloorToInt(width / InteractableConstants.widthToSticks));

        // destroy the tree world object
        Destroy(gameObject);
    }

}
