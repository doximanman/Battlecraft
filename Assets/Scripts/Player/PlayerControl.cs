using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


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
        KeyInput.instance.onRight += MoveRight;
        KeyInput.instance.onLeft += MoveLeft;
        KeyInput.instance.noMovement += StopMoving;
        KeyInput.instance.onJump += Jump;
    }


    private float prevXPosition;
    private void FixedUpdate()
    {
        // calculates real velocity
        var velocity = (playerBody.position.x - prevXPosition) / Time.fixedDeltaTime;
        animator.SetFloat("SpeedX", Mathf.Abs(velocity));
        animator.SetBool("OnGround", Logic.IsGrounded(gameObject));

        prevXPosition = playerBody.position.x;
    }

    public void MoveRight()
    {

        playerBody.velocity = playerBody.velocity.y * Vector2.up + velocity * Vector2.right;

        playerSprite.flipX = false;
    }

    public void MoveLeft()
    {
        playerBody.velocity = playerBody.velocity.y * Vector2.up + velocity * Vector2.left;

        playerSprite.flipX = true;
    }


    public void StopMoving()
    {
        playerBody.velocity = playerBody.velocity.y * Vector2.up;
    }

    public void Jump()
    {
        if (Logic.IsGrounded(gameObject))
        {
            // jump by distance - add only the velocity needed to jump to the height jumpHeight
            // in time jumpDuration
            float playerGravity = originalGravityScale * Physics2D.gravity.y;

            if (playerGravity > 0) return;

            float newVelocity = Mathf.Sqrt(-2 * playerGravity * jumpHeight) - playerBody.velocity.y;

            if (newVelocity > zeroSpeed)
            {
                playerBody.velocity += newVelocity * Vector2.up;
                animator.SetBool("OnGround", false);
                animator.SetTrigger("Jump");
            }
        }

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
        return new(playerCollider.size.x * playerBody.transform.lossyScale.x-2*leniency, Logic.collisionDetection);
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
