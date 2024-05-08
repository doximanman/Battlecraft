using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityMovement : MonoBehaviour
{
    private float prevXPosition;
    private float originalGravityScale;
    private readonly float zeroSpeed = 0.01f;

    private Animator animator;
    private Rigidbody2D body;
    private SpriteRenderer sprite;

    [SerializeField] private bool randomMovement;
    [SerializeField] private MovementSettings movementSettings;

    public Biome biome;
    public bool stayInBiome;

    private IEnumerator randomMovementCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        prevXPosition = transform.position.x;
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        originalGravityScale = body.gravityScale;

        if (randomMovement)
        {
            randomMovementCoroutine = MoveRandomly();
            StartCoroutine(MoveRandomly());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // update animator x speed
        float velocity = (transform.position.x - prevXPosition) / Time.fixedDeltaTime;
        animator.SetFloat("Speed", Mathf.Abs(velocity));
        prevXPosition = transform.position.x;

        // flip sprite accordingly
        if (Mathf.Abs(velocity) > zeroSpeed)
            sprite.flipX = velocity > 0;
    }

    /// <summary>
    /// Moves randomly, stays inside biome.
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveRandomly()
    {
        while (true)
        {
            // wait for random time
            float waitTime = UnityEngine.Random.Range(movementSettings.minWaitTime, movementSettings.maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // move for random time, in a random direction
            float moveTime = UnityEngine.Random.Range(movementSettings.minMoveTime, movementSettings.maxMoveTime);
            float randomNumber = UnityEngine.Random.Range(0f, 1f);
            bool right = randomNumber < movementSettings.chanceToGoRight;
            IEnumerator moveRoutine = right ? Walk(moveTime,Direction.RIGHT) : Walk(moveTime,Direction.LEFT);
            StartCoroutine(moveRoutine);

            yield return new WaitForSeconds(moveTime);
            StopMoving();
        }
    }

    [InspectorName("Time Between Jumps")]
    [SerializeField] private float jumpDelay;
    [InspectorName("Time It Takes To Jump")]
    [SerializeField] private float jumpTime;
    // possibility to jump if next to wall,
    // and moving towards the wall
    IEnumerator JumpRandomly(Direction direction)
    {
        // every delay, try to jump if next to wall
        while (true)
        {
            if (Logic.WallClose(gameObject, direction))
                if (UnityEngine.Random.Range(0, 1) < movementSettings.jumpChance)
                {
                    Jump();
                    // wait a bit for jump to happen
                    yield return new WaitForSeconds(jumpTime);
                    Move(direction);
                }
            yield return new WaitForSeconds(jumpDelay);
        }
    }

    // jumps over obsticles if needed
    // moves continuously
    public IEnumerator Walk(float seconds, Direction direction,bool jump = true)
    {
        if (direction == Direction.RIGHT) InvokeRepeating(nameof(MoveRight), 0, Time.fixedDeltaTime);
        else InvokeRepeating(nameof(MoveLeft),0, Time.fixedDeltaTime);
        IEnumerator jumpRoutine=null;
        if (jump)
        {
            jumpRoutine = JumpRandomly(direction);
            StartCoroutine(jumpRoutine);
        }
        yield return new WaitForSeconds(seconds);
        if(jump) StopCoroutine(jumpRoutine);
        StopMoving();
    }

    public IEnumerator Run(float seconds, Direction direction, bool jump = true)
    {
        // stop all other movement and run
        StopAllCoroutines();
        CancelInvoke();
        var oldSpeed = movementSettings.xSpeed;
        movementSettings.xSpeed = movementSettings.xRunSpeed;
        yield return Walk(seconds, direction, jump);
        movementSettings.xSpeed = oldSpeed;
        if(randomMovement)
            StartCoroutine(randomMovementCoroutine);
    }

    /// <summary>
    /// Moves a bit in the given direction
    /// </summary>
    /// <param name="direction">Direction to move</param>
    /// <param name="stayInBiome">Whether to move past the entity's defined biome</param>
    public IEnumerator Move(Direction direction,bool stayInBiome=false)
    {
        float xSpeed = direction == Direction.RIGHT ? movementSettings.xSpeed : -movementSettings.xSpeed;
        if (stayInBiome)
        {
            // return if the movement would take the entity outside of the biome.
            if (Biomes.GetBiome(transform.position.x + xSpeed) != biome)
                yield break;
        }
        body.velocity = new(xSpeed, body.velocity.y);

        yield return new WaitForFixedUpdate();
        body.velocity = new(0, body.velocity.y);
    }

    [ContextMenu("Jump")]
    public void Jump()
    {
        if (Logic.IsGrounded(gameObject))
        {
            // jump by distance - add only the velocity needed to jump to the height jumpHeight
            // in time jumpDuration
            float playerGravity = originalGravityScale * Physics2D.gravity.y;

            if (playerGravity > 0) return;

            // simple kinematic problem
            float newVelocity = Mathf.Sqrt(-2 * playerGravity * movementSettings.jumpHeight) - body.velocity.y;

            if (newVelocity > zeroSpeed)
            {
                body.velocity += newVelocity * Vector2.up;
                //animator.SetBool("OnGround", false);
                //animator.SetTrigger("Jump");
            }
        }

    }

    // for inspector
    [ContextMenu("Move Right")]
    private void MoveRight()
    {
        StartCoroutine(Move(Direction.RIGHT,stayInBiome));
    }

    [ContextMenu("Move Left")]
    private void MoveLeft()
    {
        StartCoroutine(Move(Direction.LEFT,stayInBiome));
    }

    [ContextMenu("Stop Moving")]
    public void StopMoving()
    {
        body.velocity = body.velocity.y * Vector2.up;
        CancelInvoke();
    }
}
