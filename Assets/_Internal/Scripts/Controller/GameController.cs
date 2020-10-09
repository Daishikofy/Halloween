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
    public CameraController cameraController;
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
        cameraController.Setup(currentRoom.type
            , currentRoom.cameraMin.position
            , currentRoom.cameraMax.position);

        LoadGame();
    }

    public void PlayerChangesRoom(RoomObject newRoom, Vector2 frontDoorPosition)
    {
        player.currentRoom.isPlayerInRoom = false;
        player.currentRoom = newRoom; 
        player.currentRoom.isPlayerInRoom = true;
        player.GoTo(frontDoorPosition);

        cameraController.GoTo(newRoom.type, newRoom.cameraMin.position, newRoom.cameraMax.position);
        foreach (var monster in monsters)
        {
            monster.CheckCurrentRoom();
        }
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
        SaveGame();
    }

    public void OnPlayerLose()
    {
        ui.OnGameEnded(false);
    }

    private void RestartGame()
    {
        ReloadLastSave();
    }

    public void SaveGame()
    {
        PlayerProperties.Instance.inventory = player.inventory;
        PlayerProperties.Instance.scene = SceneManager.GetActiveScene().name;
        PlayerProperties.Instance.SaveIntoJson();
    }

    public void LoadGame()
    {
        if (PlayerProperties.Instance.wasChanged)
            if (!PlayerProperties.Instance.LoadFromJson()) return;
        player.inventory = PlayerProperties.Instance.inventory;
    }

    public void ReloadLastSave()
    {
        if (!PlayerProperties.Instance.wasChanged)
            if (!PlayerProperties.Instance.LoadFromJson()) return;
        var scene = PlayerProperties.Instance.scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }
}
