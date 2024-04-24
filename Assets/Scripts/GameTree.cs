using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;


public class GameTree : MonoBehaviour
{
    enum ChoppingStatus {START,ONGOING,STOP}

    private float length;
    private float width;

    private void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        length = renderer.bounds.size.y;
        width = renderer.bounds.size.x;

        playerControl=Player.current.GetComponent<PlayerControl>();
    }

    private ChoppingStatus status;
    private float timer = 0;
    public void OnMouseDown()
    {
        timer = 0;
        status = ChoppingStatus.START;
    }

    private PlayerControl playerControl;
    public void OnMouseDrag()
    {
        // timer < 0 means the mouse wasn't clicked on this object
        // timer >= 0 means the mouse was clicked
        if (timer < 0) return;

        // check player is close enough
        var playerPosition=Player.current.transform.position;
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Vector2.Distance(playerPosition,mousePosition) > InteractableConstants.maxInteractDistance) return;

        // check player is holding an axe
        if (MetaLogic.hotbar.SelectedSlot.GetStack() == null ||
            !MetaLogic.hotbar.SelectedSlot.GetStack().Type.Equals(InteractableConstants.axe)) return;

        var duration = InteractableConstants.chopTime * length;
        // mouse was clicked and player is close enough -> start/keep counting.
        // start animation on first time
        timer += Time.deltaTime;
        if (status== ChoppingStatus.START)
        {
            // send the direction the player should be facing
            playerControl.Chop(duration,Mathf.Sign(mousePosition.x-playerPosition.x));
            status = ChoppingStatus.ONGOING;
        }
        if (timer < duration) return;

        // time has passed - break the tree and add wood and sticks to inventory.
        MetaLogic.personalInventory.AddItems(InteractableConstants.wood,
            Mathf.FloorToInt(length / InteractableConstants.lengthToWood));
        MetaLogic.personalInventory.AddItems(InteractableConstants.sticks,
            Mathf.FloorToInt(width / InteractableConstants.widthToSticks));
        
        // destroy the tree world object
        Destroy(gameObject);
    }

    private void OnMouseUp()
    {
        timer = -1;
        if(status== ChoppingStatus.ONGOING)
        {
            playerControl.CancelChop();
            status = ChoppingStatus.STOP;
        }
    }

    private void OnMouseExit()
    {
        OnMouseUp();
    }

}
