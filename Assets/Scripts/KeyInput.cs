using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    public PlayerControl player;


    public readonly List<KeyCode> moveRight = new();
    public readonly List<KeyCode> moveLeft = new();
    public readonly List<KeyCode> jump = new();
    public readonly List<KeyCode> inventory = new();

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
        inventory.Add(KeyCode.E);
    }

    private void Update()
    {
        if (keysDisabled) return;

        // pause menu
        if (Input.GetKeyDown(pause))
        {
            if (MetaLogic.pauseMenu)
            {
                MetaLogic.ClosePauseMenu();
            }
            else
            {
                MetaLogic.OpenPauseMenu();
            }
            return;
        }
        // disable input during pause
        if (MetaLogic.pauseMenu) return;

        // inventory
        if (AnyKeyIsPressedDown(inventory))
        {
            if (MetaLogic.inventoryIsOpen)
            {
                MetaLogic.CloseInventory();
            }
            else
            {
                MetaLogic.OpenInventory();
            }
        }



        if (MetaLogic.paused) return;

        // movement
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

    private bool AnyKeyIsPressedDown(List<KeyCode> keys)
    {
        return keys.Any(key => Input.GetKeyDown(key));
    }
}