using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Instance { get { return instance; } }

    public PlayerController player;
    public CameraController camera;
    public MonsterController[] monsters;

    public RoomObject[] rooms;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        for (int i = 0; i < rooms.Length; i++)
        {
            rooms[i].id = i;
        }
    }

    private void Start()
    {
        foreach (var room in rooms)
        {
            if (player.currentRoom.id == room.id)
            {
                room.isPlayerInRoom = true;
                break;
            }
        }
        var currentRoom = player.currentRoom;
        camera.Setup(currentRoom.type
            , currentRoom.cameraMin.position
            , currentRoom.cameraMax.position);
    }

    public void PlayerChangesRoom(RoomObject newRoom, Vector2 frontDoorPosition)
    {
        player.currentRoom.isPlayerInRoom = false;
        player.currentRoom = newRoom; 
        player.currentRoom.isPlayerInRoom = true;
        player.GoTo(frontDoorPosition);

        camera.GoTo(newRoom.type, newRoom.cameraMin.position, newRoom.cameraMax.position);
    }

    public void MonsterChangesRoom(RoomObject newRoom)
    {

    }
}
