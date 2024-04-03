using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    public PlayerControl player;
    [SerializeField] private Hotbar hotbar;

    public readonly List<KeyCode> moveRight = new() { KeyCode.D,KeyCode.RightArrow};
    public readonly List<KeyCode> moveLeft = new() { KeyCode.A,KeyCode.LeftArrow};
    public readonly List<KeyCode> jump = new() { KeyCode.Space};
    public readonly List<KeyCode> inventory = new() { KeyCode.E};
    public readonly KeyCode[] slotKeys = {KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3,KeyCode.Alpha4,KeyCode.Alpha5};

    public const KeyCode pause = KeyCode.Escape;

    private bool keysDisabled = false;

    private void Update()
    {
        // runs when keys are disabled 
        // (for example for tests)


        if (keysDisabled) return;


        // runs when pause menu is on


        if (Input.GetKeyDown(pause))
        {
            if (MetaLogic.pauseMenuEnabled)
            {
                MetaLogic.ClosePauseMenu();
            }
            else
            {
                MetaLogic.OpenPauseMenu();
            }
            return;
        }
        if (MetaLogic.pauseMenuEnabled) return;


        // runs when paused, but not when pause menu is on
        // (for example, inventory is open)


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


        // runs when not paused

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

        // slot selection
        var key=WhichKeyIsPressedDown(slotKeys);
        if (key != null)
        {
            hotbar.SelectedIndex = (int)key;
        }
    }



    public void DisableKeys()
    {
        keysDisabled = true;
    }

    public void EnableKeys()
    {
        keysDisabled = false;
    }

    private int? WhichKeyIsPressedDown(IEnumerable<KeyCode> keys)
    {
        for(int i = 0; i < keys.Count(); i++)
        {
            if (Input.GetKeyDown(keys.ElementAt(i)))
                return i;
        }
        return null;
    }

    private bool AnyKeyIsPressed(IEnumerable<KeyCode> keys)
    {
        return keys.Any(key => Input.GetKey(key));
    }

    private bool AnyKeyIsPressedDown(IEnumerable<KeyCode> keys)
    {
        return keys.Any(key => Input.GetKeyDown(key));
    }
}