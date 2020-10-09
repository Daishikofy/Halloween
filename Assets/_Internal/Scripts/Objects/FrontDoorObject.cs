using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FrontDoorObject : MonoBehaviour
{
    [HideInInspector]
    public DestroyableObjectEvent objectEnters;
    [HideInInspector]
    public DestroyableObjectEvent objectExits;

    public RoomObject room;

    private void Awake()
    {
        objectEnters = new DestroyableObjectEvent();
        objectExits = new DestroyableObjectEvent();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BlockingObject"))
        {
            objectEnters.Invoke(collision.GetComponent<BlockingObject>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BlockingObject"))
        {
            objectExits.Invoke(collision.GetComponent<BlockingObject>());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.9f, 0, 0.4f, 0.5f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
