using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody2D playerBody;
    public float velocity = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.RightArrow))
        {
            playerBody.velocity=playerBody.velocity.y*Vector2.up+velocity*Vector2.right;
        }
        else if(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.LeftArrow)){
            playerBody.velocity = playerBody.velocity.y * Vector2.up + velocity * Vector2.left;
        }
        else
        {
            playerBody.velocity=playerBody.velocity.y*Vector2.up;
        }
    }
}
