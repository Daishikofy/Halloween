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
        int value;
        if (inventory.TryGetValue(name, out value))
        {
            inventory.Remove(name);
            inventory.Add(name, value + quantity);
        }
        inventory.Add(name, value + quantity);        
    }

    public bool UseObject(string name)
    {
        int value;
        if (!inventory.ContainsKey(name))
            return false;

        value = inventory[name];
        inventory.Remove(name);
        value--;
        if (value < 0)
        {
            value = 0;
            return false;
        }
        inventory.Add(name, value);
        return true;
    }
}
