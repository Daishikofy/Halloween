using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DoorObject : MonoBehaviour, IInteractable
{
    public Collider2D doorCollider;
    public RoomObject room1;
    public RoomObject room2;

    public bool isLocked;
    public string objectToUnlock;

    [Header("Run time variables")]
    public bool isBlocked;
    public List<BlockingObject> blockingObjects;

    private void Start()
    {
        blockingObjects = new List<BlockingObject>();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BlockingObject"))
        {
            isBlocked = true;
            var blockingObject = collision.GetComponent<BlockingObject>();
            blockingObject.destroyed.AddListener(RemoveBlockingObject);
            blockingObjects.Add(blockingObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BlockingObject"))
        {
            RemoveBlockingObject(collision.GetComponent<BlockingObject>());
        }
    }

    private void RemoveBlockingObject(BlockingObject blockingObject)
    {
        if(blockingObjects.Contains(blockingObject))
        {
            blockingObject.destroyed.RemoveListener(RemoveBlockingObject);
            blockingObjects.Remove(blockingObject);
        }
        /*
        foreach (var obj in blockingObjects)
        {
            if (obj.gameObject.GetInstanceID() == blockingObject.GetInstanceID())
            {
                obj.destroyed.RemoveListener(RemoveBlockingObject);
                blockingObjects.Remove(obj);
            }
        }*/

        isBlocked = blockingObjects.Count != 0;
    }

    private async void OpenDoor(PlayerController player)
    {
        Debug.Log("Door is open");
        player.currentRoom.isPlayerInRoom = false;
        player.currentRoom = player.currentRoom.roomId == room1.roomId ? room2 : room1;
        player.currentRoom.isPlayerInRoom = true;
        //TODO: Animation + sound
        doorCollider.enabled = false;
        await Task.Delay(1000);
        Debug.Log("Door closed");
        doorCollider.enabled = true;
    }
}
