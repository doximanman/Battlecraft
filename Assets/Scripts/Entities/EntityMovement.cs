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
        originalGravityScale = body.gravityScale;

        if (randomMovement)
        {
            randomMovementCoroutine = MoveRandomly();
            StartCoroutine(MoveRandomly());
        }
    }

    // Update is called once per frame
    private bool grounded = false;
    void FixedUpdate()
    {
        grounded = Logic.IsGrounded(gameObject);
        // update animator x speed
        float velocity = (transform.position.x - prevXPosition) / Time.fixedDeltaTime;
        animator.SetFloat("Speed", Mathf.Abs(velocity));
        prevXPosition = transform.position.x;

        // flip sprite accordingly
        if (moving==Direction.RIGHT)
            transform.rotation = new(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
        else if(moving==Direction.LEFT)
            transform.rotation = new(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);

        if (grounded && !isMoving)
        {
            body.velocity = Vector2.zero;
            body.gravityScale = 0;
        }
        else
        {
            body.gravityScale = originalGravityScale;
        }
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
            IEnumerator moveRoutine = right ? Move(Direction.RIGHT, moveTime,stayInBiome:true) : Move(Direction.LEFT, moveTime,stayInBiome:true);
            yield return StartCoroutine(moveRoutine);
        }
    }

    [InspectorName("Time It Takes To Jump")]
    [SerializeField] private float jumpTime;

    public IEnumerator Run(float seconds, Direction direction, bool jump = true, bool stayInBiome = false)
    {
        // stop all other movement and run
        StopAllCoroutines();
        CancelInvoke();
        var oldSpeed = movementSettings.xSpeed;
        movementSettings.xSpeed = movementSettings.xRunSpeed;
        yield return StartCoroutine(Move(direction, seconds, jump, stayInBiome));
        movementSettings.xSpeed = oldSpeed;
        if (randomMovement)
            StartCoroutine(randomMovementCoroutine);
    }

    private bool isMoving = false;
    public IEnumerator Move(Direction direction, float duration, bool jump = true, bool stayInBiome = false)
    {
        // "move until the duration time has passed"
        float stopTime = Time.time + duration;
        return MoveUntil(direction, () => Time.time >= stopTime, jump, stayInBiome);
    }

    public IEnumerator MoveUntil(Direction direction, Func<bool> stop, bool jump = true, bool stayInBiome = false)
    {
        isMoving = true;
        float xSpeed = direction == Direction.RIGHT ? movementSettings.xSpeed :
            (direction == Direction.LEFT ? -movementSettings.xSpeed :
            0);
        moving = direction;
        while (!stop())
        {
            // if stayInBiome = false then move
            // otherside stayInBiome=true, so only move if not close to border
            // in the movement direction. if you are close to the border,
            // switch directions
            if (!stayInBiome || Biomes.GetBiome(transform.position.x + xSpeed/2) == biome)
            {
                // close to wall - jump
                if (jump && Logic.WallClose(gameObject, direction))
                {
                    Jump();
                    yield return new WaitForSeconds(jumpTime);
                }
                body.velocity = new(xSpeed, body.velocity.y);
            }
            else if(stayInBiome)
            {
                direction= direction==Direction.RIGHT ? Direction.LEFT : direction == Direction.LEFT ? Direction.RIGHT : direction;
                xSpeed = direction == Direction.RIGHT ? movementSettings.xSpeed :
                    (direction == Direction.LEFT ? -movementSettings.xSpeed :
                    0);
                moving = direction;
            }
            yield return new WaitForFixedUpdate();
        }
        body.velocity = new(0, body.velocity.y);
        moving = Direction.ZERO;
        isMoving = false;
    }

    Direction moving;

    [ContextMenu("Jump")]
    public void Jump()
    {
        if (grounded)
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
        StartCoroutine(Move(Direction.RIGHT, movementSettings.stepMoveTime, stayInBiome));
    }

    [ContextMenu("Move Left")]
    private void MoveLeft()
    {
        StartCoroutine(Move(Direction.LEFT, movementSettings.stepMoveTime, stayInBiome));
    }

    [ContextMenu("Stop Moving")]
    public void StopMoving()
    {
        body.velocity = body.velocity.y * Vector2.up;
        CancelInvoke();
    }
}
