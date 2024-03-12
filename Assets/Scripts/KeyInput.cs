using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    public PlayerControl player;


    public List<KeyCode> moveRight = new List<KeyCode>();
    public List<KeyCode> moveLeft = new List<KeyCode>();
    public List<KeyCode> jump = new List<KeyCode>();

    public const KeyCode pause = KeyCode.Escape;

    private bool keysDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

        moveRight.Add(KeyCode.D);
        moveRight.Add(KeyCode.RightArrow);
        moveLeft.Add(KeyCode.A);
        moveLeft.Add(KeyCode.LeftArrow);
        jump.Add(KeyCode.Space);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pause))
        {
            if(MetaLogic.paused)
                MetaLogic.Unpause();
            else
                MetaLogic.Pause();
            return;
        }

        if (MetaLogic.paused) return;


        if (keysDisabled) return;

        if (AnyKeyIsPressed(moveRight))
        {
            player.MoveRight();
        }
        else if (AnyKeyIsPressed(moveLeft))
        {
            player.MoveLeft();
        }
        else
        {
            player.StopMoving();
        }

        if (AnyKeyIsPressed(jump))
        {
            player.Jump();
        }
    }



    public void disableKeys()
    {
        keysDisabled = true;
    }

    public void enableKeys()
    {
        keysDisabled = false;
    }

    private bool AnyKeyIsPressed(List<KeyCode> keys)
    {
        return keys.Any(key => Input.GetKey(key));
    }
}