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

    public bool randomMovement;
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

        rightRotation= new(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
        leftRotation= new(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);

        waitUntilGrounded = new WaitUntil(() => grounded);
        waitForJumpTime = new WaitForSeconds(jumpTime);
        waitForFixedUpdate = new();
    }

    private Quaternion rightRotation;
    private Quaternion leftRotation;

    public void Launch(Vector2 force)
    {
        // move up so that the entity is off the ground
        Vector3 moveUp = new(0.05f * force.x, 0.05f * force.y, 0);
        transform.position += moveUp;
        stopUntilGrounded = true;
        // then apply the force, with random knockback
        System.Random rand = new();
        body.velocity = force * (float)(rand.NextDouble()+1);
    }


    private bool stopUntilGrounded = false;

    private bool grounded = false;
    void FixedUpdate()
    {
        grounded = Logic.IsGrounded(gameObject);
        if (grounded) stopUntilGrounded = false;

        // update animator x speed
        float velocity = (transform.position.x - prevXPosition) / Time.fixedDeltaTime;
        animator.SetFloat("Speed", Mathf.Abs(velocity));
        prevXPosition = transform.position.x;

        // flip sprite accordingly
        if (moving == Direction.RIGHT)
            transform.rotation = rightRotation;
        else if (moving == Direction.LEFT)
            transform.rotation = leftRotation;

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
            IEnumerator moveRoutine = right ? Move(Direction.RIGHT, moveTime, stayInBiome: true) : Move(Direction.LEFT, moveTime, stayInBiome: true);
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
    public IEnumerator Move(Direction direction, float duration, bool jump = true,bool stayInBiome=false)
    {
        // "move until the duration time has passed"
        float stopTime = Time.time + duration;
        return MoveUntil(direction, () => Time.time >= stopTime,jump,stayInBiome);
    }



    private WaitUntil waitUntilGrounded;
    private WaitForSeconds waitForJumpTime;
    private WaitForFixedUpdate waitForFixedUpdate;
    public IEnumerator DynamicMoveUntil(Func<Direction> direction, Func<bool> stop, bool jump = true)
    {
        isMoving = true;
        while (!stop())
        {
            if (stopUntilGrounded)
            {
                yield return waitUntilGrounded;
            }

            // evaluate speed according to current direction
            moving = direction();
            float xSpeed = moving == Direction.RIGHT ? movementSettings.xSpeed :
                (moving == Direction.LEFT ? -movementSettings.xSpeed :
                0);
            // jump if close to wall
            if (jump && Logic.WallClose(gameObject, moving))
            {
                Jump();
                yield return waitForJumpTime;
            }
            // if stunned - wait.
            if (stopUntilGrounded)
                yield return waitUntilGrounded;
            // move
            body.velocity = new(xSpeed, body.velocity.y);
            yield return waitForFixedUpdate;
        }
        // stop moving
        body.velocity = new(0, body.velocity.y);
        moving = Direction.ZERO;
        isMoving = false;
    }

    public IEnumerator FollowUntil(Func<bool> stop, Func<(float,Direction)> getDistance, float minDistanceFromTarget, bool jump = true)
    {
        StopAllCoroutines();
        yield return DynamicMoveUntil(() =>
        {
            // if target is to the right - move right.
            // if target is to the left - move left.
            // if target is close enough - stay in position.
            var distanceAndDirection = getDistance();
            float distance = distanceAndDirection.Item1;
            if (distance > minDistanceFromTarget) return distanceAndDirection.Item2;
            return Direction.ZERO;
        }, stop, jump);
        if (randomMovement) StartCoroutine(randomMovementCoroutine);
    }

    public IEnumerator MoveUntil(Direction initialDirection, Func<bool> stop, bool jump = true, bool stayInBiome = false)
    {
        Direction currentDirection = initialDirection;
        if (stayInBiome)
        {
            yield return StartCoroutine(DynamicMoveUntil(() =>
            {
                // if outside of biome - turn towards the biome
                if (Biomes.GetBiomeClose(transform.position.x, currentDirection) != biome)
                    currentDirection = Biomes.BiomeWhere(biome, transform.position.x);
                // if close to wall and can't jump - turn around
                // "can't jump" means jump is false or can't jump (Logic.CanJump)
                else if (Logic.WallClose(gameObject, currentDirection) && (!jump || !Logic.CanJump(gameObject, currentDirection)))
                    currentDirection = currentDirection == Direction.LEFT ? Direction.RIGHT :
                                        currentDirection == Direction.RIGHT ? Direction.LEFT :
                                        Direction.ZERO;
                return currentDirection;
            }, stop, jump));
        }
        else
        {
            // move without concern to biome
            // if close to wall and can't jump - turn around
            yield return StartCoroutine(DynamicMoveUntil(() =>
            {
                if (Logic.WallClose(gameObject, currentDirection) && (!jump || !Logic.CanJump(gameObject, currentDirection)))
                    currentDirection = currentDirection == Direction.LEFT ? Direction.RIGHT :
                                        currentDirection == Direction.RIGHT ? Direction.LEFT :
                                        Direction.ZERO;
                return currentDirection;
            }, stop, jump));
        }
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
