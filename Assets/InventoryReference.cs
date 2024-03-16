using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryReference : MonoBehaviour
{
    private Inventory inventory;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    public void SetInventory(Inventory newInventory)
    {

    }
}
