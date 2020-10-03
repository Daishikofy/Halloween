using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController 
{
    public Dictionary<string, int> inventory;
    public InventoryController()
    {
        inventory = new Dictionary<string, int>();
    }

    public void GetObject(string name, int quantity)
    {
        int value;
        if (inventory.TryGetValue(name, out value))
        {
            inventory.Remove(name);
            inventory.Add(name, value + quantity);
        }
        else
        {
            inventory.Add(name, quantity);
        }
    }

    public bool UseObject(string name)
    {
        int value;
        if (inventory.TryGetValue(name, out value))
        {
            inventory.Remove(name);
            if ((value -= 1) > 0)
                inventory.Add(name, value);
            return true;
        }
        return false;
    }
}
