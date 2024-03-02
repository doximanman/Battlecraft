using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    public Rigidbody2D playerBody;
    public float HorizontalVelocity = 10;
    public float VerticalVelocity = 10;

    private float RoundToZero=0.00001f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow))
        {
            playerBody.velocity=playerBody.velocity.y*Vector2.up+ HorizontalVelocity * Vector2.right;
        }
        else if(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)){
            playerBody.velocity = playerBody.velocity.y * Vector2.up + HorizontalVelocity * Vector2.left;
        }
        else
        {
            playerBody.velocity=playerBody.velocity.y*Vector2.up;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (playerBody.velocity.y <= RoundToZero)
            {
                playerBody.velocity += VerticalVelocity*Vector2.up;
            }
        }
    }
}
