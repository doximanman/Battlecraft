using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICloseInventoryListener
{
    void OnCloseInventory();
}

public interface IOpenInventoryListener
{
    void OnOpenInventory();
}

public interface IPauseMenuListener
{
    void OnPauseMenu();
}