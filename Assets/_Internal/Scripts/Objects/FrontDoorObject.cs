﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FrontDoorObject : MonoBehaviour
{
    [HideInInspector]
    public BlockingObjectEvent objectEnters;
    [HideInInspector]
    public BlockingObjectEvent objectExits;

    public RoomObject room;

    private void Awake()
    {
        objectEnters = new BlockingObjectEvent();
        objectExits = new BlockingObjectEvent();
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
        Gizmos.color = new Color(0.1f, 1, 0.4f, 0.5f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
