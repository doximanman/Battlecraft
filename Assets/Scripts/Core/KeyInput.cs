using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    public static KeyInput instance;

    public delegate void KeyDownHandler();
    public delegate void KeyNumHandler(int number);

    public readonly List<KeyCode> moveRight = new() { KeyCode.D,KeyCode.RightArrow};
    public readonly List<KeyCode> moveLeft = new() { KeyCode.A,KeyCode.LeftArrow};
    public readonly List<KeyCode> jump = new() { KeyCode.Space};
    public readonly List<KeyCode> inventory = new() { KeyCode.E};
    public readonly KeyCode[] slotKeys = {KeyCode.Alpha1,KeyCode.Alpha2,KeyCode.Alpha3,KeyCode.Alpha4,KeyCode.Alpha5};

    public KeyDownHandler onRight;
    public KeyDownHandler onLeft;
    public KeyDownHandler noMovement;
    public KeyDownHandler onJump;
    public KeyDownHandler onInventory;
    public KeyNumHandler onNum;
    public KeyDownHandler onEscape;


    public const KeyCode escape = KeyCode.Escape;

    private bool keysDisabled = false;

    private void Start()
    {
        instance = this;
    }


    public bool paused = false;
    private void Update()
    {
        // runs when keys are disabled 
        // (for example for tests)

        if (keysDisabled) return;


        // runs when pause menu is on

        if (Input.GetKeyDown(escape)) onEscape();

        if (MetaLogic.pauseMenuEnabled) return;


        // runs when paused but pause menu is off
        // for example, inventory is open.
        if (AnyKeyIsPressedDown(inventory)) onInventory();

        if (MetaLogic.paused) return;


        // runs when not paused

        // movement
        if (AnyKeyIsPressed(moveRight)) onRight();
        else if (AnyKeyIsPressed(moveLeft)) onLeft();
        else noMovement();

        if (AnyKeyIsPressed(jump)) onJump();

        // slot selection
        var key=WhichKeyIsPressedDown(slotKeys);
        if (key != null) onNum((int)key);
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