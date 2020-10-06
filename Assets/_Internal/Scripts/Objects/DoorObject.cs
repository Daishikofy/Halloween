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
        return isBlocked;
    }

    private void AddBlockingObject(BlockingObject blockingObject)
    {
        isBlocked = true;
        blockingObject.destroyed.AddListener(RemoveBlockingObject);
        blockingObjects.Add(blockingObject);
    }

    private void RemoveBlockingObject(BlockingObject blockingObject)
    {
        if(blockingObjects.Contains(blockingObject))
        {
            blockingObject.destroyed.RemoveListener(RemoveBlockingObject);
            blockingObjects.Remove(blockingObject);
        }

        isBlocked = blockingObjects.Count != 0;
    }

    private async void OpenDoor(PlayerController player)
    {
        Debug.Log("Door is open");
        int index = player.currentRoom.id == frontDoors[0].room.id ? 1 : 0;
        GameController.Instance.PlayerChangesRoom(frontDoors[index].room, frontDoors[index].transform.position);
        //TODO: Animation + sound

        doorCollider.enabled = false;

        do { await Task.Delay(100); } 
        while (player.automaticMovement);

        Debug.Log("Door closed");
        doorCollider.enabled = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0, 0.4f, 0.5f);
        Gizmos.DrawCube(transform.position, doorCollider.bounds.size);
    }
}
