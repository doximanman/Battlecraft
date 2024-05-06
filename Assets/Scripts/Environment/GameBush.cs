using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class GameBush : Interactable
{
    [SerializeField] private float length;

    public override void Start()
    {
        base.Start();

        // physical real length of the bush
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        length = renderer.bounds.size.y;

        // axe to chop, chop duration proportional to bush size.
        requiredTool = InteractableConstants.axe;
        chopDuration = InteractableConstants.bushChopTime * length;
    }

    public override void OnFinishChopping()
    {
        // add sticks to inventory
        // proportional to size of bush
        InventoryLogic.personalInventory.AddItems(InteractableConstants.sticks,
            Mathf.CeilToInt(InteractableConstants.lengthToSticks*length));

        // delete the bush world object
        Destroy(gameObject);
    }
}
