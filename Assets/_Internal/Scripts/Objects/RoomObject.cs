using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum RoomType
{
    SquareRoom,
    HorizontalRoom,
    VerticalRoom
}

public class RoomObject : MonoBehaviour
{
    public int roomId;
    public DoorObject[] doors;
    public RoomType type;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
