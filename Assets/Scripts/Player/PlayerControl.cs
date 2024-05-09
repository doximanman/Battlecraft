using Codice.CM.Client.Differences;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Codice.Client.BaseCommands.BranchExplorer.Layout.BrExLayout;


public class PlayerControl : MonoBehaviour
{
    public float velocity = 10;
    public float jumpHeight = 10;
    public float leniency = 0.05f;

    private Animator animator;
    private Rigidbody2D playerBody;
    private SpriteRenderer playerSprite;
    private BoxCollider2D playerCollider;
    private readonly float zeroSpeed = 0.01f;
    private float originalGravityScale = 0;


    private float epsilon = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        animator= GetComponent<Animator>();
        playerBody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<BoxCollider2D>();
        originalGravityScale = playerBody.gravityScale;

        playerBody.freezeRotation = true;
        prevXPosition= playerBody.position.x;

        // key inputs
        bool rightHeld = false;
        bool leftHeld = false;
        KeyInput.instance.onRight += (down,held,_) =>
        {
            rightHeld = held;
            // once key is first pressed - start moving until it is not pressed
            if (down || (held && !isMoving))
            {
                var moveRight = MoveUntil(Direction.RIGHT, () => !rightHeld);
                StartCoroutine(moveRight);
            }
        };
        KeyInput.instance.onLeft += (down,held,_) =>
        {
            leftHeld = held;
            if (down || (held && !isMoving))
            {
                var moveLeft = MoveUntil(Direction.LEFT, () => !leftHeld);
                StartCoroutine(moveLeft);
            }
        };
        
        bool jump = true;
        KeyInput.instance.onJump += (down,held,up) => {
            // jump logic: at first you can just jump.
            // if you were able to jump (i.e. you're grounded)
            // then you have to let go of the jump key before the next jump.
            // if you let go mid-jump and pressed again, then that jump will be "queued"
            // and you will jump as soon as you hit the ground, if you're still holding.
            if (jump && held) {
                jump = !Jump();
            }
            if (up)
                jump = true;

        };
    }

    private float prevXPosition;
    private bool grounded = false;
    private void FixedUpdate()
    {
        grounded = Logic.IsGrounded(gameObject);
        // calculates real velocity
        var velocity = (playerBody.position.x - prevXPosition) / Time.fixedDeltaTime;
        animator.SetFloat("SpeedX", Mathf.Abs(velocity));
        animator.SetBool("OnGround", grounded);

        prevXPosition = playerBody.position.x;

        if (moving == Direction.RIGHT)
            transform.rotation = new(transform.rotation.x,180,transform.rotation.z,transform.rotation.w);
        else if (moving == Direction.LEFT)
            transform.rotation = new(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);

        if (grounded && !isMoving)
        {
            playerBody.velocity = Vector2.zero;
            playerBody.gravityScale = 0;
        }
        else
        {
            playerBody.gravityScale = originalGravityScale;
        }
    }

    private bool isMoving = false;
    public IEnumerator Move(Direction direction, float duration)
    {
        isMoving = true;
        float xSpeed = direction == Direction.RIGHT ? velocity : (direction == Direction.LEFT ? -velocity : 0);
        float timer = 0;
        moving = direction;
        while (timer < duration && moving == direction) {
            playerBody.velocity = new(xSpeed, playerBody.velocity.y);
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
        }
        playerBody.velocity = new(0, playerBody.velocity.y);
        moving = Direction.ZERO;
        isMoving = false;
    }

    public IEnumerator MoveUntil(Direction direction,Func<bool> stop)
    {
        isMoving = true;
        float xSpeed = direction == Direction.RIGHT ? velocity : (direction == Direction.LEFT ? -velocity : 0);
        float timer = 0;
        moving = direction;
        while (!stop())
        {
            playerBody.velocity = new(xSpeed, playerBody.velocity.y);
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
        }
        playerBody.velocity = new(0, playerBody.velocity.y);
        moving = Direction.ZERO;
        isMoving = false;
    }

    Direction moving;
    public void StepRight()
    {
        StartCoroutine(Move(Direction.RIGHT,Time.fixedDeltaTime+epsilon));
        //playerBody.velocity = playerBody.velocity.y * Vector2.up + velocity * Vector2.right;

        //playerSprite.flipX = false;
    }

    public void StepLeft()
    {
        StartCoroutine(Move(Direction.LEFT,Time.fixedDeltaTime+ epsilon));
        //playerBody.velocity = playerBody.velocity.y * Vector2.up + velocity * Vector2.left;

        //playerSprite.flipX = true;
    }

    // returns: if jump was executed
    public bool Jump()
    {
        if (grounded)
        {
            Debug.Log(1);
            isMoving = true;
            // jump by distance - add only the velocity needed to jump to the height jumpHeight
            // in time jumpDuration
            float playerGravity = originalGravityScale * Physics2D.gravity.y;

            if (playerGravity > 0) return false;

            float newVelocity = Mathf.Sqrt(-2 * playerGravity * jumpHeight) - playerBody.velocity.y;

            if (newVelocity > zeroSpeed)
            {
                playerBody.velocity += newVelocity * Vector2.up;
                animator.SetBool("OnGround", false);
                animator.SetTrigger("Jump");
            }
            // wait a bit to get off the ground
            Invoke(nameof(JumpHelper), 0.1f);
            return true;
        }
        return false;
    }

    private void JumpHelper()
    {
        isMoving = false;
    }

    public void CancelChop()
    {
        if (chopping)
        {
            animator.SetTrigger("ResetAnimation");
            CancelInvoke();
            chopping = false;
        }
    }

    private bool chopping = false;
    // chop(0) cancels the current chop
    public void Chop(float duration,float direction = 1)
    {
        // make sure player is rotated correctly
        playerSprite.flipX = direction<0;
        animator.SetTrigger("Chop");
        chopping = true;
        Invoke(nameof(CancelChop), duration);
    }

    private Vector2 BoxPosition()
    {
        return new(playerCollider.bounds.center.x, playerCollider.bounds.min.y - Logic.collisionDetection / 2);
    }

    private Vector2 BoxSize()
    {
        return new(playerCollider.bounds.size.x - Logic.collisionDetection, Logic.collisionDetection);
        //return new(playerCollider.size.x * playerBody.transform.lossyScale.x-2*leniency, Logic.collisionDetection);
    }


    private void OnDrawGizmos()
    {
        if (playerCollider == null) return;
        Vector2 boxPosition = BoxPosition();
        Vector2 boxSize = BoxSize() ;

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(boxPosition, boxSize);


        Vector2 newVelocity = playerBody.velocity.y * Vector2.up + velocity * Vector2.right;
        if (Mathf.Abs(playerBody.velocity.y) < zeroSpeed) newVelocity = velocity * Vector2.right;
        Vector2 centerBottom = new(playerCollider.bounds.max.x, playerCollider.bounds.min.y);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(centerBottom, 10 * zeroSpeed * newVelocity.normalized);
    }


}
