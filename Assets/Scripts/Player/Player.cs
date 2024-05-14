using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player current;

    private Animator animator;
    private SpriteRenderer playerSprite;
    private BoxCollider2D playerCollider;
    private void Start()
    {
        current = this;
        animator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerCollider=GetComponent<BoxCollider2D>();
    }

    public float swingDelay;
    public float hitDelay;
    private float timer=0;
    private void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && timer> swingDelay)
        {
            StartSwing();
            timer = 0;
        }

    }

    public void PlaySound(string name)
    {
        AudioManager.instance.Play("Player",name);
    }

    public void StopSound(string name)
    {
        AudioManager.instance.Stop("Player",name);
    }

    private readonly List<IHitListener> hitListeners=new();
    public void StartSwing()
    {
        // item must be swingable
        var item = InventoryLogic.hotbar.SelectedSlot.GetStack() == null ? null : InventoryLogic.hotbar.SelectedSlot.GetStack().Type;
        if (item == null || !item.swingable) return;

        // hit animation
        animator.SetTrigger("Attack");
        // delayed hit
        Invoke(nameof(Swing), hitDelay);
        
    }


    private Vector2 startPoint=new();
    private Vector2 size= new();
    private Vector2 direction= new();
    public void Swing()
    {
        // selected item
        var item = InventoryLogic.hotbar.SelectedSlot.GetStack() == null ? null : InventoryLogic.hotbar.SelectedSlot.GetStack().Type;

        // item must be swingable
        if (item == null || !item.swingable) return;

        //Vector2 startPoint;
        //Vector2 size;
        //Vector2 direction;
        // raycast the swing
        // raycast parameters
        if (transform.rotation.y < 0.1f)
        {
            // facing left
            startPoint = new(playerCollider.bounds.min.x-0.1f, playerCollider.bounds.min.y);
            size = new Vector2(item.stats.range, playerCollider.bounds.size.y);
            direction = Vector2.left + Vector2.up;
        }
        else
        {
            // facing right
            startPoint = new(playerCollider.bounds.max.x+0.1f, playerCollider.bounds.min.y);
            size = new Vector2(item.stats.range, playerCollider.bounds.size.y);
            direction = Vector2.right + Vector2.up;
        }

        // raycast swing
        var boxSize=BoxSize();
        var swingHit = Physics2D.BoxCastAll(boxSize.Item1,boxSize.Item2, 0, direction,0);
        var hitObjects = swingHit.Select(collision => collision.collider.gameObject);

        // check each listener if they were hit (inside of hitObjects array)
        // create new list in case the listeners list changes
        var listeners = new List<IHitListener>(hitListeners);
        foreach (var listener in listeners)
        {
            if (hitObjects.Contains(listener.gameObject))
                listener.OnHit(item);
        }
    }

    private (Vector3,Vector3) BoxSize()
    {
        var center = new Vector3(startPoint.x+size.x*direction.x/2,startPoint.y+size.y*direction.y/2);
        var boxSize = new Vector3(size.x,size.y);
        return (center, boxSize);
    }

    public void RegisterSwingListener(IHitListener listener)
    {
        hitListeners.Add(listener);
    }

    public void RemoveSwingListener(IHitListener listener)
    {
        hitListeners.Remove(listener);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawCube(BoxSize().Item1,BoxSize().Item2);
    }
}
