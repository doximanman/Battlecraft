using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Entity : IHitListener
{
    public string entityName;

    public StackData[] droppedItems;
    public Vector2 knockback;
    public bool getsScared;
    public float runawayTime;
    public bool chasesPlayer;
    public float chaseRange;
    public float chaseUntilCloserThan;
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

    [DefaultValue(1)]
    [SerializeField] private float minSize;
    [DefaultValue(1.5f)]
    [SerializeField] private float maxSize=1.5f;

    private EntityMovement movement;
    private void Start()
    {
        Health = maxHealth;
        Player.current.RegisterSwingListener(this);
        movement=GetComponent<EntityMovement>();

        // random scale (= random size)
        System.Random rand = new();
        Vector3 originalScale = transform.lossyScale;
        transform.localScale = originalScale * RandomFloat(rand,minSize,maxSize);
        // move up a bit so the entity doesn't phase through the ground
        transform.position += Vector3.up * 0.1f;
    }

    private float RandomFloat(System.Random rand, float min, float max) => (float) rand.NextDouble() * (max - min) + min;

    [SerializeField] float distanceToPlayer;
    [SerializeField] bool chasing = false;
    private void FixedUpdate()
    {
        if (chasesPlayer)
        {
            // if this entity is of the type that can chase
            // the player, then always evaluate distanceToPlayer.
            distanceToPlayer = Mathf.Abs(Player.current.transform.position.x - transform.position.x);
            if (!chasing)
            {
                chasing = true;
                // follow the player position until too far away, or the
                // entity is no longer of the type that can follow the player.
                StartCoroutine(movement.FollowUntil(
                    () => !(chasing = distanceToPlayer < chaseRange && chasesPlayer),
                    () => Player.current.transform.position.x,
                    chaseUntilCloserThan));
            }
        }
    }

    public void PlaySound(string soundName)
    {
        AudioManager.instance.PlayFrom(gameObject,entityName, soundName);
    }

    public override void OnHit(ItemType hitWith)
    {
        EntityMovement movement=GetComponent<EntityMovement>();
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
        if(gameObject.TryGetComponent<Rigidbody2D>(out var _))
        {

            bool isRight = Player.current.transform.position.x < transform.position.x;
            movement.Launch(new Vector3(isRight ? knockback.x : -knockback.x, knockback.y,0));

        }
        // entity runs away
        if(getsScared && movement!=null)
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
        Invoke(nameof(KillEntity), 0.1f);
    }

    private void KillEntity()
    {
        Destroy(gameObject);
    }
}

/// <summary>
/// Editor script to hide irrelavent properties
/// (for example when "getsScared" is off, the
/// "runawayTime" variable is irrelavent)
/// </summary>
[CustomEditor(typeof(Entity))]
public class EntityEditor : Editor
{
    SerializedProperty entityName;

    SerializedProperty droppedItems;
    SerializedProperty knockback;
    SerializedProperty getsScared;
    SerializedProperty runawayTime;
    SerializedProperty chasesPlayer;
    SerializedProperty chaseRange;
    SerializedProperty maxHealth;
    SerializedProperty chasing;
    SerializedProperty distanceToPlayer;
    SerializedProperty chaseUntilCloserThan;
    SerializedProperty minSize;
    SerializedProperty maxSize;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement returnValue= base.CreateInspectorGUI();
        entityName = serializedObject.FindProperty("entityName");
        droppedItems = serializedObject.FindProperty("droppedItems");
        knockback = serializedObject.FindProperty("knockback");
        getsScared = serializedObject.FindProperty("getsScared");
        runawayTime = serializedObject.FindProperty("runawayTime");
        chasesPlayer = serializedObject.FindProperty("chasesPlayer");
        chaseRange = serializedObject.FindProperty("chaseRange");
        maxHealth = serializedObject.FindProperty("maxHealth");
        chasing = serializedObject.FindProperty("chasing");
        distanceToPlayer = serializedObject.FindProperty("distanceToPlayer");
        chaseUntilCloserThan = serializedObject.FindProperty("chaseUntilCloserThan");
        minSize = serializedObject.FindProperty("minSize");
        maxSize = serializedObject.FindProperty("maxSize");
        return returnValue;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(entityName);
        EditorGUILayout.PropertyField(droppedItems);
        EditorGUILayout.PropertyField(knockback);
        EditorGUILayout.PropertyField(getsScared);
        EditorGUILayout.PropertyField(minSize);
        EditorGUILayout.PropertyField(maxSize);

        if(getsScared.boolValue)
        {
            EditorGUILayout.PropertyField(runawayTime);
        }
        EditorGUILayout.PropertyField(chasesPlayer);
        if(chasesPlayer.boolValue)
        {
            EditorGUILayout.PropertyField(chaseRange);
            EditorGUILayout.PropertyField(chaseUntilCloserThan);
        }
        EditorGUILayout.PropertyField(maxHealth);

        if (chasesPlayer.boolValue)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(distanceToPlayer);
            EditorGUILayout.PropertyField(chasing);
            EditorGUI.EndDisabledGroup();
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
