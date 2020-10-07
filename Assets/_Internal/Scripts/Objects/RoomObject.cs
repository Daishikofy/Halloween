using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    public int id;
    public DoorObject[] doors;
    public CameraMovement type;
    [Tooltip("Max value between which the camera can move when RoomType is Vertical or Horizontal")]
    public Transform cameraMin;
    [Tooltip("Min value between which the camera can move when RoomType is Vertical or Horizontal")]
    public Transform cameraMax;
    [Header("Runtime variables")]
    public bool isPlayerInRoom;
    public bool isMonsterInRoom;
    // Start is called before the first frame update
    void Start()
    {
        if (type == CameraMovement.Horizontal)
        {
            if (cameraMin.position.x > cameraMax.position.x)
            {
                var aux = cameraMin;
                cameraMin = cameraMax;
                cameraMax = cameraMin;
            }
        }
        else if (type == CameraMovement.Vertical)
        {
            if (cameraMin.position.y > cameraMax.position.y)
            {
                var aux = cameraMin;
                cameraMin = cameraMax;
                cameraMax = cameraMin;
            }
        }
    }

    public bool isAdjacent(int roomId)
    {
        foreach (var door in doors)
        {
            int index = door.frontDoors[0].room.id == roomId ? 0 : 1;
            if (door.frontDoors[index].room.id == roomId)
                return true;
        }
        return false;
    }
    /// <summary>
    ///Returns the door to the adjacent room with the corresponding RoomId 
    ///Returns NULL if none is found
    /// </summary>
    /// <param name="roomId">Id of the room you want to obtain</param>
    /// <returns></returns>
    public DoorObject GetDoorToAdjacentRoom(int roomId)
    {
        foreach (var door in doors)
        {
            int index = door.frontDoors[0].room.id == roomId ? 0 : 1;
            if (door.frontDoors[index].room.id == roomId)
            {
                return door;
            }
        }
        return null;
    }
}
