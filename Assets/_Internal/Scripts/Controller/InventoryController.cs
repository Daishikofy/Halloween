using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryController 
{
    public Dictionary<string, int> inventory;
    public InventoryController()
    {
        inventory = new Dictionary<string, int>();
    }

    public void GetObject(string name, int quantity)
    {
        int value = 0;

        if (inventory.TryGetValue(name, out value))
        {
            inventory.Remove(name);
        }
        inventory.Add(name, value + quantity);        
    }

    public bool UseObject(string name)
    {
        int value = 0;

        if (inventory.TryGetValue(name, out value))
        {
            if (value > 0)
            {
                value--;
                inventory.Remove(name);
                inventory.Add(name, value);
                return true;
            }
        }
        return false;
    }
}
