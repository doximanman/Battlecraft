using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player : IHitListener
{
    [SerializeField] private SpriteRenderer playerSprite;

    public Vector3 spawnPoint;

    public static Player current;

    private Animator animator;
    public BoxCollider2D playerCollider;
    private void Start()
    {
        current = this;
        animator = GetComponent<Animator>();

        spawnPoint= transform.position;

        GetComponent<StatManager>().GetStat("Food").OnValueChanged += (value) =>
        {
            if (value == 0)
                Death();
        };

        GetComponent<StatManager>().GetStat("Health").OnValueChanged += (value) =>
        {
            if (value == 0)
                Death();
        };

        Hotbar.instance.OnItemUse += (stack) =>
        {
            if (stack.Type.swingable && swingTimer > swingDelay)
            {
                StartSwing(stack.Type);
                swingTimer = 0;
            }
            else if (stack.Type.food)
                Consume(stack);
        };
    }

    public float swingDelay;
    private float swingTimer=0;
    private void Update()
    {
        swingTimer += Time.deltaTime;
/*
        if (Input.GetMouseButtonDown(0) && timer> swingDelay)
        {
            TrySwing();
            timer = 0;
        }*/

    }

    public void PlaySound(string name)
    {
        AudioManager.instance.Play("Player",name);
    }


    public void StopSound(string name)
    {
        AudioManager.instance.Stop("Player",name);
    }

    public void Consume(ItemStack stack)
    {
        PlaySound("Eat");
        Stat food = GetComponent<StatManager>().GetStat("Food");
        Stat health = GetComponent<StatManager>().GetStat("Health");
        food.Value += stack.Type.foodStats.saturation;
        health.Value += stack.Type.foodStats.heal;
        stack.ItemCount--;
    }

    private readonly List<IHitListener> hitListeners=new();

    public void StartSwing(ItemType type)
    {
        if (InventoryLogic.inventoryIsOpen) return;

        swingItem = type;
        // hit animation
        // the animation triggers swing
        animator.SetTrigger("Attack");
    }

    private ItemType swingItem;
    private Vector2 startPoint=new();
    private Vector2 size= new();
    private Vector2 direction= new();
    public void Swing()
    {

        //Vector2 startPoint;
        //Vector2 size;
        //Vector2 direction;
        // raycast the swing
        // raycast parameters
        if (transform.rotation.y < 0.1f)
        {
            // facing left
            startPoint = new(playerCollider.bounds.min.x-0.1f, playerCollider.bounds.min.y);
            size = new Vector2(swingItem.stats.range, playerCollider.bounds.size.y);
            direction = Vector2.left + Vector2.up;
        }
        else
        {
            // facing right
            startPoint = new(playerCollider.bounds.max.x+0.1f, playerCollider.bounds.min.y);
            size = new Vector2(swingItem.stats.range, playerCollider.bounds.size.y);
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
                listener.OnHit(swingItem);
        }
    }

    public void StopAnimations()
    {
        animator.SetTrigger("ResetAnimation");
    }

    // cache - only get once
    private Stat health = null;
    public override void OnHit(ItemType hitWith)
    {
        if (health == null)
            health = GetComponent<StatManager>().GetStat(StatManager.Health);
        float dmg = hitWith.swingable ? 0 : hitWith.stats.damage;
        health.Value -= dmg;
        
    }

    [SerializeField] private float hurtDuration;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material hurtMaterial;
    public void Hurt()
    {
        PlaySound("Hurt");
        // can't walk while hurt (looks bad)
        animator.SetBool("Hurt", true);
        playerSprite.material = hurtMaterial;
        Invoke(nameof(Unhurt), hurtDuration);
    }

    public void Unhurt()
    {
        animator.SetBool("Hurt", false);
        playerSprite.material = defaultMaterial;
    }

    // hit the player from which direction
    // and with what damage
    PlayerControl control = null;
    public void HitFrom(float dmg, float knockback, Direction direction)
    {
        Hurt();
        if (control == null)
            control = GetComponent<PlayerControl>();
        Vector2 launchDirection = new(direction == Direction.LEFT ? 1 : direction == Direction.RIGHT ? -1 : 0, 1);
        control.Launch(knockback * launchDirection);
        if(health == null)
            health = GetComponent<StatManager>().GetStat(StatManager.Health);
        health.Value -= dmg;
    }

    [ContextMenu("Death")]
    public void Death()
    {
        ItemInterations inventory = GetComponent<ItemInterations>();
        inventory.DropInventory(false);
        transform.position = spawnPoint;
        GetComponent<StatManager>().ResetStats();
    }

    // makes swing hitbox bigger
    public Vector2 hitLeniency;
    private (Vector3,Vector3) BoxSize()
    {
        var center = new Vector3(startPoint.x+size.x*direction.x/2,startPoint.y+size.y*direction.y/2);
        var boxSize = new Vector3(size.x+hitLeniency.x,size.y+hitLeniency.y);
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
