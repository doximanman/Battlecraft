using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    public Rigidbody2D playerBody;
    public SpriteRenderer playerSprite;
    public float velocity = 10;
    public float jumpStrength = 10;

    public bool allowJump = true;



    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {

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
        if (allowJump) {
            playerBody.AddForce(jumpStrength * Vector2.up,ForceMode2D.Impulse);
            DisableJump();
        }
        
    }

    public void EnableJump()
    {
        allowJump = true;
    }

    public void DisableJump()
    {
        allowJump = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {

            // check if collision is from below - if so, enable jump
            ContactPoint2D collisionPoint = collision.GetContact(0);

            // dot product of collision normal and (0,1) returns 1 if collision normal
            // has a "up" component.
            if (Vector2.Dot(collisionPoint.normal, Vector2.up) > 0.5)
            {
                EnableJump();
            }

            
        }
    }

}
