using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DoorObject : MonoBehaviour, IInteractable
{
    public Collider2D doorCollider;
    public FrontDoorObject[] frontDoors;

    public bool isLocked;
    public string objectToUnlock;

    [Header("Run time variables")]
    public bool isBlocked;
    public List<BlockingObject> blockingObjects;

    private int doorUsers = 0;

    private void Start()
    {
        blockingObjects = new List<BlockingObject>();
        foreach (var frontDoor in frontDoors)
        {
            frontDoor.objectEnters.AddListener(AddBlockingObject);
            frontDoor.objectExits.AddListener(RemoveBlockingObject);
        }
    }

    public bool OnInteraction(PlayerController player)
    {
        if (isBlocked)
        {
            Debug.Log("Door is blocked");
            return false;
        }
        else if (isLocked)
        {
            Debug.Log("Door is locked");
            if (player.inventory.UseObject(objectToUnlock))
            {
                Debug.Log("You unlocked the door");
                isLocked = false;
                OpenDoor(player);
                return true;
            }
            else
            {
                return false;
            }
        }
        OpenDoor(player);
        return true;
    }

    public bool OnInteraction(MonsterController monster)
    {
        if (isBlocked)
            return false;
        OpenDoor(monster);
        return true;
    }

    private void AddBlockingObject(BlockingObject blockingObject)
    {
        isBlocked = true;
        blockingObject.destroyed.AddListener(RemoveBlockingObject);
        blockingObjects.Add(blockingObject);
    }

    private void RemoveBlockingObject(BlockingObject blockingObject)
    {
        Debug.Log("Remove blocking object");
        if(blockingObjects.Contains(blockingObject))
        {
            blockingObject.destroyed.RemoveListener(RemoveBlockingObject);
            blockingObjects.Remove(blockingObject);
        }

        isBlocked = blockingObjects.Count != 0;
    }

    private async void OpenDoor(PlayerController player)
    {
        //Debug.Log("Player : Door is open");
        int index = player.currentRoom.id == frontDoors[0].room.id ? 1 : 0;
        GameController.Instance.PlayerChangesRoom(frontDoors[index].room, frontDoors[index].transform.position);
        //TODO: Animation + sound
        doorCollider.enabled = false;
        doorUsers += 1;
        do {
            await Task.Delay(100); 
        } 
        while (player.automaticMovement);

        //Debug.Log("Player : Door closed");
        if(doorUsers == 1)
        {
            doorUsers--;
            doorCollider.enabled = true;
        }
    }

    private async void OpenDoor(MonsterController monster)
    {
        int index = monster.currentRoom.id == frontDoors[0].room.id ? 1 : 0;
        GameController.Instance.MonsterChangesRoom(monster.id,frontDoors[index].room, frontDoors[index].transform.position);
        //TODO: Animation + sound
        doorUsers += 1;
        doorCollider.enabled = false;
        //Debug.Log("Monster : Door open");
        do {
            await Task.Delay(100); 
        }
        while (monster.state == MonsterState.ChangingRoom);

        //.Log("Monster : Door closed");
        doorCollider.enabled = true;
        doorUsers--;
        if (doorUsers == 0)  
            doorCollider.enabled = true;
        
    }
    public bool IsAdjacent(int roomId)
    {
        if (roomId == frontDoors[0].room.id)
            return true;
        if (roomId == frontDoors[1].room.id)
                return true;
        return false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0, 0.4f, 0.5f);
        Gizmos.DrawCube(transform.position, doorCollider.bounds.size);
    }
}
