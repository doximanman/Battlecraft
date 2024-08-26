using System;
using UnityEngine;

[Serializable]
public abstract class Interactable : MonoBehaviour
{
    public InteractableType type;

    enum ChoppingStatus { START, ONGOING, STOP }

    private protected float maxDistance;
    [SerializeField] protected ItemType requiredTool;
    [SerializeField] protected float chopDuration;
    private PlayerControl playerControl;

    public virtual void Start()
    {
        // default values
        playerControl=Player.current.GetComponent<PlayerControl>();
        maxDistance = InteractableConstants.maxInteractDistance;

        if(!Interactables.current.interactables.Contains(this))
            Interactables.current.interactables.Add(this);
    }

    // gives the ability to an interactable to save internal data
    /// <summary>
    /// save internal data of the interactable
    /// </summary>
    /// <returns>serialization of the internal data</returns>
    public virtual string SerializeData()
    {
        return "";
    }

    /// <summary>
    /// restore internal state from serialized internal data
    /// </summary>
    /// <param name="data">the serialized data</param>
    public virtual void DeserializeData(string data) { }

    public bool CloseEnough()
    {
        var playerPosition = Player.current.transform.position;
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return Vector2.Distance(playerPosition, mousePosition) <= maxDistance;
    }

    /// <summary>
    /// Whether the player is holding the appropriate
    /// tool to break this block.
    /// Treats "null" as "match any tool".
    /// </summary>
    public bool HoldingTool()
    {
        // treat null as "match any"
        if (requiredTool == null)
            return true;

        var selected = InventoryLogic.hotbar.SelectedSlot.GetStack();
        // if selected tool is null - we have determined
        // the required tool isnt null, so return false.
        if (selected == null)
            return false;
        return selected.Type == requiredTool;
    }

    /// <summary>
    /// click without holding
    /// </summary>
    public virtual void OnInteract() { }

    public abstract void OnFinishChopping();

    public void OnMouseDown()
    {
        timer = 0;
        status = ChoppingStatus.START;
        MetaLogic.mouseDownOnBlock = true;
    }

    private ChoppingStatus status;
    private float timer = -1;
    private string tool;
    public void OnMouseDrag()
    {

        // timer < 0 means the mouse wasn't clicked on this object
        // timer >= 0 means the mouse was clicked
        if (timer < 0) return;

        timer += Time.deltaTime;

        if (MetaLogic.paused)
        {
            timer = 0;
            status = ChoppingStatus.STOP;
            return;
        }

        // check player is close enough
        if (!CloseEnough())
        {
            timer = 0;
            status = ChoppingStatus.STOP;
            return;
        }

        // check player is holding the appropriate tool
        if (!HoldingTool())
        {
            timer = 0;
            status = ChoppingStatus.STOP;
            return;
        }

        // mouse was clicked and player is close enough -> start/keep counting.
        // change status to ongoing - chopping process is ongoing
        if (status == ChoppingStatus.START)
        {
            tool = requiredTool == null? "" : requiredTool.name;

            // play animation if player is holding an axe
            if (tool == "Axe")
            {
                var playerPosition = Player.current.transform.position;
                var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var direction = mousePosition.x - playerPosition.x > 0 ? Direction.RIGHT : Direction.LEFT;
                // send the direction the player should be facing
                playerControl.Chop(chopDuration, direction);
            }
            status = ChoppingStatus.ONGOING;
        }
        if (timer < chopDuration) return;

        // time has passed - chop the object
        MetaLogic.mouseDownOnBlock = false;
        OnFinishChopping();
    }

    public void OnMouseUp()
    {
        MetaLogic.mouseDownOnBlock = false;
        timer = -1;
        if (status == ChoppingStatus.ONGOING)
        {
            if(tool == "Axe")
                playerControl.CancelChop();
            status = ChoppingStatus.STOP;
            OnInteract();
        }
    }

    public void OnMouseExit()
    {
        // like onmouseup but without
        // activating interact
        MetaLogic.mouseDownOnBlock = false;
        timer = -1;
        if (status == ChoppingStatus.ONGOING)
        {
            if (tool == "Axe")
                playerControl.CancelChop();
            status = ChoppingStatus.STOP;
            //OnInteract();
        }
    }

    private void OnDestroy()
    {
        Interactables.current.OnRemove(this);
    }
}
