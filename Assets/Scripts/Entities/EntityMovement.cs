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

    // Start is called before the first frame update
    void Start()
    {
        prevXPosition= transform.position.x;
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        originalGravityScale = body.gravityScale;

        StartCoroutine(MoveRandomly());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float velocity = (transform.position.x - prevXPosition)/Time.fixedDeltaTime;
        animator.SetFloat("Speed", Mathf.Abs(velocity));
        prevXPosition = transform.position.x;

        if(Mathf.Abs(velocity) > zeroSpeed)
            sprite.flipX = velocity > 0;
    }

    private bool move = true;
    IEnumerator MoveRandomly()
    {
        while (move)
        {
            float waitTime = UnityEngine.Random.Range(movementSettings.minWaitTime, movementSettings.maxWaitTime);
            yield return new WaitForSeconds(waitTime);
            float moveTime = UnityEngine.Random.Range(movementSettings.minMoveTime, movementSettings.maxMoveTime);
            float randomNumber = UnityEngine.Random.Range(0f, 1f);
            Debug.Log(randomNumber + " less than " + movementSettings.chanceToGoRight);
            bool right = randomNumber < movementSettings.chanceToGoRight;
            if (right) MoveRight(); else MoveLeft();
            StartCoroutine(JumpRandomly(right ? 1 : -1));
            yield return new WaitForSeconds(moveTime);
            StopCoroutine(nameof(JumpRandomly));
            StopMoving();
        }
    }

    [InspectorName("Time Between Jumps")]
    [SerializeField] private float jumpDelay;
    [InspectorName("Time It Takes To Jump")]
    [SerializeField] private float jumpTime;
    // possibility to jump if next to wall
    // and moving towards the wall
    IEnumerator JumpRandomly(float direction)
    {
        // every second, try to jump
        while (move)
        {
            yield return new WaitForSeconds(jumpDelay);
            if (Logic.WallClose(gameObject, direction))
                if (UnityEngine.Random.Range(0, 1) < movementSettings.jumpChance)
                {
                    Jump();
                    // wait a bit for jump to happen
                    yield return new WaitForSeconds(jumpTime);
                    if (direction > 0) MoveRight(); else MoveLeft();
                }
            yield return null;
        }
    }

    [ContextMenu("Move Right")]
    public void MoveRight()
    {
        body.velocity = body.velocity.y * Vector2.up + movementSettings.xSpeed * Vector2.right;
    }

    [ContextMenu("Move Left")]
    public void MoveLeft()
    {
        body.velocity = body.velocity.y * Vector2.up + movementSettings.xSpeed * Vector2.left;
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

    [ContextMenu("Stop Moving")]
    public void StopMoving()
    {
        body.velocity=body.velocity.y * Vector2.up;
    }
}
