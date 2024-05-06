using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

[Serializable]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] enum ChoppingStatus { START, ONGOING, STOP }

    [SerializeField] protected float maxDistance;
    [SerializeField] protected ItemType requiredTool;
    [SerializeField] protected float chopDuration;
    [SerializeField] protected PlayerControl playerControl;

    public virtual void Start()
    {
        // default values
        playerControl=Player.current.GetComponent<PlayerControl>();
        maxDistance = InteractableConstants.maxInteractDistance;
    }

    public bool CloseEnough()
    {
        var playerPosition = Player.current.transform.position;
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return Vector2.Distance(playerPosition, mousePosition) <= maxDistance;
    }

    public bool HoldingTool()
    {
        var selected = InventoryLogic.hotbar.SelectedSlot.GetStack();
        if (selected == null || selected.Equals(null))
            return requiredTool == null || requiredTool.Equals(null);
        return selected.Type.Equals(requiredTool);
    }

    public abstract void OnFinishChopping();

    public void OnMouseDown()
    {
        timer = 0;
        status = ChoppingStatus.START;
    }

    private ChoppingStatus status;
    private float timer = -1;
    public void OnMouseDrag()
    {
        // timer < 0 means the mouse wasn't clicked on this object
        // timer >= 0 means the mouse was clicked
        if (timer < 0) return;

        // check player is close enough
        if (!CloseEnough()) return;

        // check player is holding an axe
        if (!HoldingTool()) return;

        // mouse was clicked and player is close enough -> start/keep counting.
        // start animation on first time
        timer += Time.deltaTime;
        if (status == ChoppingStatus.START)
        {
            var playerPosition = Player.current.transform.position;
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // send the direction the player should be facing
            playerControl.Chop(chopDuration, Mathf.Sign(mousePosition.x - playerPosition.x));
            status = ChoppingStatus.ONGOING;
        }
        if (timer < chopDuration) return;

        // time has passed - chop the object
        OnFinishChopping();
    }

    public void OnMouseUp()
    {
        timer = -1;
        if (status == ChoppingStatus.ONGOING)
        {
            playerControl.CancelChop();
            status = ChoppingStatus.STOP;
        }
    }

    public void OnMouseExit()
    {
        OnMouseUp();
    }
}
