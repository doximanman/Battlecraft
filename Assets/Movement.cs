using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
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
            playerBody.AddForce(jumpStrength * Vector2.up);
            allowJump = false;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            allowJump = true;
        }
    }

}
