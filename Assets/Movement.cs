using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    public Rigidbody2D playerBody;
    public SpriteRenderer playerSprite;
    public float HorizontalVelocity = 10;
    public float VerticalVelocity = 10;

    public bool allowJump = true;

    private float RoundToZero = 0.00001f;



    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            playerBody.velocity = playerBody.velocity.y * Vector2.up + HorizontalVelocity * Vector2.right;
            playerSprite.flipX = false;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            playerBody.velocity = playerBody.velocity.y * Vector2.up + HorizontalVelocity * Vector2.left;
            playerSprite.flipX = true;
        }
        else
        {
            playerBody.velocity = playerBody.velocity.y * Vector2.up;
        }

        if (allowJump && Input.GetKey(KeyCode.Space))
        {
            playerBody.velocity += VerticalVelocity * Vector2.up;
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
