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
        Debug.Log(animalName + " was hit by " + hitWith);
        // reduce health by weapon damage
        Health -= hitWith.stats.damage;
    }

    [ContextMenu("Death")]
    public void Death()
    {
        // entity died, just adds items to inventory
        InventoryLogic.personalInventory.AddItems(droppedItems);
        Player.current.RemoveSwingListener(this);
        Destroy(gameObject);
    }
}
