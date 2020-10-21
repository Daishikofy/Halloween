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

    public void GetObject(CollectibleType item, int quantity)
    {
        int value = 0;
        var name = item.ToString();
        if (inventory.TryGetValue(name, out value))
        {
            inventory.Remove(name);
        }
        inventory.Add(name, value + quantity);        
    }

    public bool UseObject(CollectibleType item)
    {
        int value = 0;
        var name = item.ToString();
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
