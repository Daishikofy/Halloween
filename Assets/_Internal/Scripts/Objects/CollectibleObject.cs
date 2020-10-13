using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum CollectibleType
{
    None,
    SmallKey,
    BigKey,
    Candy
}

public class CollectibleObject : MonoBehaviour, IInteractable
{
    public CollectibleType type;
    public bool OnInteraction(PlayerController player)
    {
        player.CollectItem(this);
        Destroy(this.gameObject);
        return true;
    }

    public bool OnInteraction(MonsterController monster)
    {
        return false;
    }
}
