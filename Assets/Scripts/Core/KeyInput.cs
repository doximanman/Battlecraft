using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class KeyInput : MonoBehaviour
{
    public static KeyInput instance;

    public delegate void KeyDownHandler(bool down,bool held,bool up);
    public delegate void KeyNumHandler(int number);

    public readonly List<KeyCode> moveRight = new() { KeyCode.D, KeyCode.RightArrow };
    public readonly List<KeyCode> moveLeft = new() { KeyCode.A, KeyCode.LeftArrow };
    public readonly List<KeyCode> jump = new() { KeyCode.Space };
    public readonly List<KeyCode> inventory = new() { KeyCode.E };
    public readonly List<KeyCode> save = new() { KeyCode.F5 };
    public readonly KeyCode[] slotKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };

    // every handler has a (bool,bool,bool) variable
    // so that the key handlers can pass a reference
    public KeyDownHandler onRight;
    public KeyDownHandler onLeft;
    public KeyDownHandler onJump;
    public KeyDownHandler onInventory;
    public KeyNumHandler onNum;
    public KeyDownHandler onEscape;
    public KeyDownHandler onSave;


    public const KeyCode escape = KeyCode.Escape;

    private bool keysDisabled = false;

    private void Awake()
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
        var pressed = KeyIsPressed(escape);
        onEscape?.Invoke(pressed.Item1, pressed.Item2, pressed.Item3);
        //onEscape?.Invoke(KeyPressed(escape));

        if (MetaLogic.pauseMenuEnabled) return;


        // runs when paused but pause menu is off
        // for example, inventory is open.
        pressed = AnyKeyIsPressed(inventory);
        onInventory?.Invoke(pressed.Item1, pressed.Item2, pressed.Item3);
        //onInventory?.Invoke(AnyKeyIsPressedDown(inventory));

        if (MetaLogic.paused) return;


        // runs when not paused

        // movement
        pressed = AnyKeyIsPressed(moveRight);
        onRight?.Invoke(pressed.Item1, pressed.Item2, pressed.Item3);
        pressed = AnyKeyIsPressed(moveLeft);
        onLeft?.Invoke(pressed.Item1, pressed.Item2, pressed.Item3);

        pressed = AnyKeyIsPressed(jump);
        onJump?.Invoke(pressed.Item1, pressed.Item2, pressed.Item3);
        //onJump?.Invoke(AnyKeyIsPressed(jump));

        // slot selection
        var key = WhichKeyIsPressedDown(slotKeys);
        if (key != null) onNum?.Invoke((int)key);

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
        for (int i = 0; i < keys.Count(); i++)
        {
            if (Input.GetKeyDown(keys.ElementAt(i)))
                return i;
        }
        return null;
    }

    private (bool,bool,bool) KeyIsPressed(KeyCode key)
    {
        bool down = Input.GetKeyDown(key);
        bool held = Input.GetKey(key);
        bool up = Input.GetKeyUp(key);
        return (down, held, up);
    }

    private (bool,bool,bool) AnyKeyIsPressed(IEnumerable<KeyCode> keys)
    {
        bool down = keys.Any(key => Input.GetKeyDown(key));
        bool held = keys.Any(key => Input.GetKey(key));
        bool up = keys.Any(key => Input.GetKeyUp(key));
        return (down, held, up);
    }

    /*private bool AnyKeyIsPressed(IEnumerable<KeyCode> keys)
    {
        return keys.Any(key => Input.GetKey(key));
    }

    private bool AnyKeyIsPressedDown(IEnumerable<KeyCode> keys)
    {
        return keys.Any(key => Input.GetKeyDown(key));
    }*/
}