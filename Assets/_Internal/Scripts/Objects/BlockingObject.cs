using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlockingObject : MonoBehaviour, IDragable
{
    public int lifePoints;

    [HideInInspector]
    public UnityEvent destroyed;
    public bool OnDamaged()
    {
        lifePoints--;
        if (lifePoints <= 0)
        {
            DestroyObject();
            return true;
        }
        return false;
    }

    public void DestroyObject()
    {
        //TODO: Destroy animation
        GetComponent<Collider>().enabled = false;
    }

}
