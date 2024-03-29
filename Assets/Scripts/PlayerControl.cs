using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    public float collisionDetection = 0.1f;
    public float velocity = 10;
    public float jumpHeight = 10;
    public float jumpDuration = 1f;
    public float leniency = 0.05f;

    private Logic logic;
    private GameObject player;
    private Rigidbody2D playerBody;
    private SpriteRenderer playerSprite;
    private BoxCollider2D playerCollider;
    private float previousYVelocity;
    private float epsilon = 0.01f;
    private float originalGravityScale = 0;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerBody = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<BoxCollider2D>();
        originalGravityScale = playerBody.gravityScale;
        logic=GameObject.FindGameObjectWithTag("Logic").GetComponent<Logic>();

        playerBody.freezeRotation = true;
        previousYVelocity = playerBody.velocity.y;
    }

    void Update()
    {
    }

    /*private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            EnableJump();
        }
    }*/

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
        if (IsGrounded())
        {
            // jump by distance - add only the velocity needed to jump to the height jumpHeight
            // in time jumpDuration
            float playerGravity = originalGravityScale * Physics2D.gravity.y;

            if (playerGravity > 0) return;

            float newVelocity = Mathf.Sqrt(-2 * playerGravity * jumpHeight) - playerBody.velocity.y;

            if (newVelocity > 0)
            {
                playerBody.velocity += newVelocity * Vector2.up;
            }
        }

    }

    private Vector2 BoxPosition()
    {
        return new(playerCollider.bounds.center.x, playerCollider.bounds.min.y - collisionDetection / 2);
    }

    private Vector2 BoxSize()
    {
        return new(playerCollider.size.x * playerBody.transform.lossyScale.x-2*leniency, collisionDetection);
    }

    public bool IsGrounded()
    {
        // create a box to see if there is ground below the player.
        Vector2 boxPosition = BoxPosition();
        Vector2 boxSize = BoxSize();

        var Colliders = Physics2D.OverlapBoxAll(boxPosition, boxSize, 0);

        

        return Colliders.Any(collider => logic.canJumpOn(collider));
    }



    private void OnDrawGizmos()
    {
        if (playerCollider == null) return;
        Vector2 boxPosition = BoxPosition();
        Vector2 boxSize = BoxSize() ;

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(boxPosition, boxSize);


        Vector2 newVelocity = playerBody.velocity.y * Vector2.up + velocity * Vector2.right;
        if (Mathf.Abs(playerBody.velocity.y) < epsilon) newVelocity = velocity * Vector2.right;
        Vector2 centerBottom = new Vector2(playerCollider.bounds.max.x, playerCollider.bounds.min.y);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(centerBottom, newVelocity.normalized*epsilon*10);
    }


}
