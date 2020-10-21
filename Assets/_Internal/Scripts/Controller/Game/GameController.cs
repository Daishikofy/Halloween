using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Instance { get { return instance; } }

    public PlayerController player;
    public MonsterController[] monsters;
    public RoomObject[] rooms;
    [Scene]
    public string nextScene;
    [Header("Object Setup")]
    public GameUIController ui;
    public CameraController cameraController;

    private string currentScene;

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
    }

    public void BlockPlayer(bool value)
    {
        if (value)
            player.SetMovement(PlayerMovement.None);
        else
            player.SetMovement(PlayerMovement.Continuous);
    }

    public void PlayerEarnsItems(CollectibleType item,int quantity)
    {
        player.inventory.GetObject(item, quantity);
    }

    public void PlayerMissedPuzzle()
    {
        foreach (var monster in monsters)
        {
            if (monster.currentRoom.isAdjacent(player.currentRoom.id) && monster.currentRoom != player.currentRoom)
                MonsterFollowsPlayer(monster.id);
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

    public Queue<int> BackToRoom(int currentRoom, int destination)
    {
        var exploderRooms = new List<int>();
        var path = new List<int>();
        var res = BFS(path, rooms[destination], currentRoom);
        Debug.Log("PATH FROM " + currentRoom + " TO " + destination);
        foreach (var em in res)
        {
            Debug.Log(em);
        }

        var newPath = new Queue<int>();
        foreach (var index in res)
        {
            newPath.Enqueue(index);
        }
        newPath.Dequeue();
        return newPath;
    }
    
    private List<int> BFS(List<int> path, RoomObject currentRoom, int destination)
    {
        Queue<int> queue = new Queue<int>();
        bool[] visited = new bool[rooms.Length];
        int[] pred = new int[rooms.Length];

        for (int i = 0; i < rooms.Length; i++)
        {
            visited[i] = false;
            pred[i] = -1;
        }

        visited[currentRoom.id] = true;
        queue.Enqueue(currentRoom.id);

        // bfs Algorithm 
        while (queue.Count > 0)
        {
            int u = queue.Dequeue();
            var adj = rooms[u].GetAdjacentRooms();
            foreach (var room in adj)
            {
                if (visited[room.id] == false)
                {
                    visited[room.id] = true;
                    pred[room.id] = u;
                    queue.Enqueue(room.id);

                    // stopping condition (when we find 
                    // our destination) 
                    if (room.id == destination)
                    {
                        int crawl = destination;
                        path.Add(crawl);
                        while (pred[crawl] != -1)
                        {
                            path.Add(pred[crawl]);
                            crawl = pred[crawl];
                        }
                        return path;
                    }
                }
            }
        }
        return null;
    }

    public void OnPlayerWin()
    {
        ui.OnGameEnded(true);
        currentScene = nextScene;
        SaveGame();
        if (nextScene != "")
            SceneManager.LoadScene(nextScene);
        else
            SceneManager.LoadScene(0);
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
        PlayerProperties.Instance.scene = currentScene;
        PlayerProperties.Instance.Save();
    }

    public void LoadGame()
    {
        if (PlayerProperties.Instance.wasChanged)
            if (!PlayerProperties.Instance.Load()) return;
        player.inventory = PlayerProperties.Instance.inventory;
        currentScene = PlayerProperties.Instance.scene;
        Debug.Log("player.inventory.candy: " + player.inventory.inventory["Candy"]);
    }

    public void ReloadLastSave()
    {
        if (!PlayerProperties.Instance.wasChanged)
            if (!PlayerProperties.Instance.Load()) return;
        var scene = PlayerProperties.Instance.scene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene);
    }
}
