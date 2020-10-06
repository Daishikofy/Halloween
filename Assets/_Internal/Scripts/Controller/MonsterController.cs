using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    public RoomObject[] patrolPath;
    public RoomObject currentRoom;

    public bool isHeadingBackToPatrol;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TryNextPatrolRoom()
    {
        //If heading back to patrol, next made up room
        //else next patrol room
    }

    public void AttackDoor(DoorObject door)
    {
        //Animation, sound
        BlockingObject blockingObject = null;
        float minDistance = 1000;
        foreach (var blocking in door.blockingObjects)
        {
            float distance = Vector3.Distance(blocking.transform.position, this.transform.position);
            if (Vector3.Distance(blocking.transform.position, this.transform.position) < minDistance)
                blockingObject = blocking;
        }
        blockingObject.Damaged();
    }
}
