using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    public int id;
    public List<DoorObject> doors = new List<DoorObject>();
    public CameraMovement type;
    [Tooltip("Max value between which the camera can move when RoomType is Vertical or Horizontal")]
    public Transform cameraMin;
    [Tooltip("Min value between which the camera can move when RoomType is Vertical or Horizontal")]
    public Transform cameraMax;
    [Header("Runtime variables")]
    public List<GameObject> objectsInRoom;
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
        objectsInRoom = new List<GameObject>();
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

    public List<RoomObject> GetAdjacentRooms()
    {
        var rooms = new List<RoomObject>();
        foreach (var door in doors)
        {
            var room = door.frontDoors[0].room.id == id ? door.frontDoors[1].room : door.frontDoors[0].room;
            rooms.Add(room);
        }
        return rooms;
    }

    public void UpdateObjects()
    {
        foreach (var obj in objectsInRoom)
        {
            if (obj == null)
            {
                objectsInRoom.Remove(obj);
                break;
            }    
        }
    }
}
