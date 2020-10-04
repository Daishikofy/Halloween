using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlockingObject : MonoBehaviour, IDragable
{
    public int lifePoints;

    [HideInInspector]
    public BlockingObjectEvent destroyed;

    private void Awake()
    {
        destroyed = new BlockingObjectEvent();
    }

    public bool Damaged()
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
        destroyed.Invoke(this);
    }

}
