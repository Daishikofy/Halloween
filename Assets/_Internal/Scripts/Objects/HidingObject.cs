using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingObject : DestroyableObject, IInteractable
{
    public bool isLocked;
    public bool OnInteraction(PlayerController player)
    {
        if (isLocked) 
            return false;
        player.Hides();
        destroyed.AddListener(player.Unhides);
        return true;
    }

    public bool OnInteraction(MonsterController monster)
    {
        throw new System.NotImplementedException();
    }
}
