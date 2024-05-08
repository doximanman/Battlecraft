using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class Entity : IHitListener
{
    public string animalName;
    public Sprite animalSprite;

    public StackData[] droppedItems;
    public Vector2 knockback;
    public bool getsScared;
    public float runawayTime;
    public int maxHealth;
    private float health=1;
    public float Health
    {
        get { return health; }
        set
        {
            if (value <= 0)
                Death();
            else
                health = value;
        }
    }

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite=animalSprite;
        Health = maxHealth;
        Player.current.RegisterSwingListener(this);
    }

    public override void OnHit(ItemType hitWith)
    {
        // entity was hit
        if (!hitWith.swingable)
        {
            Debug.Log("Hit with unswingable item??");
            return;
        }
        // hurt animation
        if(gameObject.TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger("Hurt");
        }
        // knockback
        if(gameObject.TryGetComponent<Rigidbody2D>(out var body))
        {
            bool isRight = Player.current.transform.position.x < transform.position.x;
            if (isRight) body.velocity += knockback;
            else body.velocity += new Vector2(-knockback.x, knockback.y);
        }
        // entity runs away
        if(getsScared && gameObject.TryGetComponent<EntityMovement>(out var movement))
        {
            bool isRight = Player.current.transform.position.x < transform.position.x;
            StartCoroutine(movement.Run(runawayTime,isRight ? Direction.RIGHT : Direction.LEFT));
        }
        // reduce health by weapon damage
        Health -= hitWith.stats.damage;
    }

    [ContextMenu("Death")]
    public void Death()
    {
        // entity died, just adds items to inventory
        InventoryLogic.personalInventory.AddItems(droppedItems);
        Player.current.RemoveSwingListener(this);
        Invoke(nameof(KillAnimal), 0.1f);
    }

    private void KillAnimal()
    {
        Destroy(gameObject);
    }
}
