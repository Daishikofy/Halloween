using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BlockingObject : MonoBehaviour, IDragable
{
    public int lifePoints;
    public Collider2D collider;
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
        collider.enabled = false;
        destroyed.Invoke(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, (float)lifePoints / 10);
    }
}
