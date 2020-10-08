using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum collectibleType
{
    SmallKey,
    BigKey,
    Candy
}

public class CollectibleObject : MonoBehaviour, IInteractable
{
    public collectibleType type;
    public bool OnInteraction(PlayerController player)
    {
        player.CollectItem(type.ToString());
        Destroy(this.gameObject);
        return true;
    }

    public bool OnInteraction(MonsterController monster)
    {
        return false;
    }
}
