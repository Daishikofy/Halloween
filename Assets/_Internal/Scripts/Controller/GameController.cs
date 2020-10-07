using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Instance { get { return instance; } }

    public GameUIController ui;
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
        for (int i = 0; i < monsters.Length; i++)
        {
            monsters[i].id = i;
        }

        ui.onRestart.AddListener(RestartGame);
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

    public void MonsterChangesRoom(int monsterId, RoomObject newRoom, Vector2 frontDoorPosition)
    {
        monsters[monsterId].currentRoom.isMonsterInRoom = true;
        monsters[monsterId].currentRoom = newRoom;
        monsters[monsterId].currentRoom.isMonsterInRoom = true;
        monsters[monsterId].SetTargetPoint(frontDoorPosition);
    }

    public void MonsterFollowsPlayer(int monsterId)
    {
        monsters[monsterId].GoToRoom(player.currentRoom.id);
    }

    public void OnPlayerWin()
    {
        ui.OnGameEnded(true);
    }

    public void OnPlayerLose()
    {
        ui.OnGameEnded(false);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
