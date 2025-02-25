using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;


#if UNITY_EDITOR
using UnityEditor;
#endif


public class Entity : IHitListener
{

    public EntityType type;

    public string entityName;

    public StackData[] droppedItems;
    public Vector2 knockback;
    public bool hostile;
    public bool projectile;
    public float attackDamage;
    public float attackKnockback;
    public float attackDelay;
    public bool getsScared;
    public float runawayTime;
    public bool chasesPlayer;
    public float chaseRange;
    public float chaseUntilCloserThan;
    public int maxHealth;
    private float health=1;


    Rigidbody2D body;
    SpriteRenderer spriteRenderer;
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
    private Animator animator;
    private void Awake()
    {
        Health = maxHealth;
        movement = GetComponent<EntityMovement>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // random scale (= random size)
        Vector3 originalScale = transform.lossyScale;
        transform.localScale = originalScale * Logic.Random(minSize, maxSize);
    }

    private void Start()
    {
        Player.current.RegisterSwingListener(this);

        // move up a bit so the entity doesn't phase through the ground
        transform.position += Vector3.up * 0.1f;

        currentAttackDelay = attackDelay;

        Entities.current.entities.Add(this);
    }


    [SerializeField] float distanceToPlayer;
    [SerializeField] float currentAttackDelay;
    [SerializeField] bool chasing = false;
    private void FixedUpdate()
    {
        if(chasesPlayer || hostile)
            distanceToPlayer = body.Distance(Player.current.playerCollider).distance;

        if (chasesPlayer)
        {
            // if this entity is of the type that can chase
            // the player, then always evaluate distanceToPlayer.
            if (!chasing)
            {
                chasing = true;
                // follow the player position until too far away, or the
                // entity is no longer of the type that can follow the player.
                StartCoroutine(movement.FollowUntil(
                    () => !(chasing = distanceToPlayer < chaseRange && chasesPlayer),
                    () => (body.Distance(Player.current.playerCollider).distance,
                            Player.current.transform.position.x > transform.position.x ? Direction.RIGHT : Direction.LEFT),
                    chaseUntilCloserThan));
            }
        }

        if (hostile)
        {
            currentAttackDelay -= Time.fixedDeltaTime;
            if (currentAttackDelay < 0) currentAttackDelay = 0;

            float attackRange = chaseUntilCloserThan * 1.5f;

            if (distanceToPlayer < attackRange && currentAttackDelay == 0)
            {
                // attack!
                if (!attacking)
                {
                    if (!projectile || LineOfSight())
                    {
                        attacking = true;
                        animator.SetTrigger("Attack");
                    }
                }
            }
            else if(attacking)
            {
                attacking = false;
                animator.SetTrigger("ResetAnimation");
            }
        }
    }

    bool attacking = false;

    public void Attack()
    {
        Direction hitFrom = Player.current.transform.position.x - transform.position.x < 0 ? Direction.RIGHT : Direction.LEFT;
        float difficultyMultiplier = Settings.current.Difficulty.GetMultiplier();
        Player.current.HitFrom(attackDamage * difficultyMultiplier,
            attackKnockback * difficultyMultiplier,
            hitFrom);
        currentAttackDelay = attackDelay;
        attacking = false;
    }

    /// <summary>
    /// whether the entity has light of sight to the player
    /// </summary>
    public bool LineOfSight()
    {
        Vector3 playerPosition = Player.current.transform.position;
        Vector3 thisPosition = transform.position;
        float distance = Vector3.Distance(thisPosition, playerPosition);
        Vector3 direction = (playerPosition - thisPosition).normalized;
        // game layer is ground
        var result = Physics2D.RaycastAll(thisPosition, direction, distance,LayerMask.GetMask("Game"));
        // no ground in line of sight
        return result.Length == 0;
    }

    public void PlaySound(string soundName)
    {
        AudioManager.instance.PlayFrom(gameObject,entityName, soundName);
    }

    [SerializeField] private float hurtDuration;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material hurtMaterial;
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
            spriteRenderer.material = hurtMaterial;
            Invoke(nameof(EndHurt), hurtDuration);
        }
        // knockback
        if(body!=null)
        {

            bool isRight = Player.current.transform.position.x < transform.position.x;
            movement.Launch(new Vector3(isRight ? knockback.x : -knockback.x, knockback.y));

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

    private void EndHurt()
    {
        spriteRenderer.material = defaultMaterial;
    }

    private void Drop(IEnumerable<StackData> stacks)
    {
        foreach(StackData stack in stacks)
        {
            Drop(stack);
        }
    }

    private void Drop(StackData stack)
    {
        DroppedStacksManager.instance.Drop(stack, transform.position);
    }

    [ContextMenu("Death")]
    public void Death()
    {
        // entity died, drop the items
        Drop(droppedItems);
        Player.current.RemoveSwingListener(this);
        Invoke(nameof(KillEntity), 0.1f);
    }

    private void KillEntity()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Player.current.RemoveSwingListener(this);
        Entities.current.entities.Remove(this);
    }
}

#if UNITY_EDITOR
/// <summary>
/// Editor script to hide irrelavent properties
/// (for example when "getsScared" is off, the
/// "runawayTime" variable is irrelavent)
/// </summary>
[CustomEditor(typeof(Entity))]
public class EntityEditor : Editor
{
    SerializedProperty entityType;
    SerializedProperty entityName;
    SerializedProperty droppedItems;
    SerializedProperty knockback;
    SerializedProperty hostile;
    SerializedProperty projectile;
    SerializedProperty attackDamage;
    SerializedProperty attackKnockback;
    SerializedProperty attackDelay;
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
    SerializedProperty hurtDuration;
    SerializedProperty defaultMaterial;
    SerializedProperty hurtMaterial;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement returnValue= base.CreateInspectorGUI();
        entityType = serializedObject.FindProperty("type");
        entityName = serializedObject.FindProperty("entityName");
        droppedItems = serializedObject.FindProperty("droppedItems");
        knockback = serializedObject.FindProperty("knockback");
        hostile = serializedObject.FindProperty("hostile");
        projectile = serializedObject.FindProperty("projectile");
        attackDamage = serializedObject.FindProperty("attackDamage");
        attackKnockback = serializedObject.FindProperty("attackKnockback");
        attackDelay = serializedObject.FindProperty("attackDelay");
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
        hurtDuration = serializedObject.FindProperty("hurtDuration");
        defaultMaterial = serializedObject.FindProperty("defaultMaterial");
        hurtMaterial = serializedObject.FindProperty("hurtMaterial");
        return returnValue;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(entityType);
        EditorGUILayout.PropertyField(entityName);
        EditorGUILayout.PropertyField(hurtDuration);
        EditorGUILayout.PropertyField(defaultMaterial);
        EditorGUILayout.PropertyField(hurtMaterial);
        EditorGUILayout.PropertyField(droppedItems);
        EditorGUILayout.PropertyField(knockback);
        EditorGUILayout.PropertyField(minSize);
        EditorGUILayout.PropertyField(maxSize);

        EditorGUILayout.PropertyField(getsScared);
        if (getsScared.boolValue)
        {
            EditorGUILayout.PropertyField(runawayTime);
        }

        EditorGUILayout.PropertyField(hostile);

        if (hostile.boolValue)
        {
            EditorGUILayout.PropertyField(projectile);
            EditorGUILayout.PropertyField(attackDamage);
            EditorGUILayout.PropertyField(attackDelay);
            EditorGUILayout.PropertyField(attackKnockback);
        }

        EditorGUILayout.PropertyField(chasesPlayer);
        if (chasesPlayer.boolValue)
        {
            EditorGUILayout.PropertyField(chaseRange);
            EditorGUILayout.PropertyField(chaseUntilCloserThan);
        }
        EditorGUILayout.PropertyField(maxHealth);

        // read only stats
        EditorGUI.BeginDisabledGroup(true);

        if (chasesPlayer.boolValue)
        {
            EditorGUILayout.PropertyField(distanceToPlayer);
            EditorGUILayout.PropertyField(chasing);
        }

        EditorGUI.EndDisabledGroup();

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif