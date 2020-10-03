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
    public BlockingObject blockingObject;

    public bool OnInteraction(PlayerController player)
    {
        if (isBlocked)
            return false;
        else if (isLocked)
        {
            if(player.inventory.UseObject(objectToUnlock))
            {
                isLocked = false;
                OpenDoor();
                return true;
            }
            else
            {
                return false;
            }
        }
        OpenDoor();
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
            blockingObject = collision.GetComponent<BlockingObject>();
            blockingObject.destroyed.AddListener(RemoveBlockingObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BlockingObject"))
        {
            RemoveBlockingObject();
        }
    }

    private void RemoveBlockingObject()
    {
        isBlocked = false;
        blockingObject.destroyed.RemoveListener(RemoveBlockingObject);
        blockingObject = null;
    }

    private async void OpenDoor()
    {
        //TODO: Animation + sound
        doorCollider.enabled = false;
        await Task.Delay(1000);
        doorCollider.enabled = true;
    }
}
