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
    public CollectibleType objectToUnlock;

    [Header("SFX")]
    public string openDoorSFX;
    public string closeDoorSFX;

    [Header("Animations")]
    public string openDoorAnim;
    public string closeDoorAnim;

    [Header("Run time variables")]
    public bool isBlocked;
    public List<DestroyableObject> destroyableObjects;

    private int doorUsers = 0;

    private void Awake()
    {
        Setup();
    }

    private void Start()
    {
        destroyableObjects = new List<DestroyableObject>();
        foreach (var frontDoor in frontDoors)
        {
            frontDoor.objectEnters.AddListener(AddDestroyableObject);
            frontDoor.objectExits.AddListener(RemoveDestroyableObject);
        }
    }

    private void Setup()
    {
        frontDoors[0].room.doors.Add(this);
        frontDoors[1].room.doors.Add(this);
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
            if (player.inventory.UseObject(objectToUnlock.ToString()))
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

    private void AddDestroyableObject(DestroyableObject destroyableObject)
    {
        isBlocked = true;
        destroyableObject.destroyed.AddListener(RemoveDestroyableObject);
        destroyableObjects.Add(destroyableObject);
    }

    private void RemoveDestroyableObject(DestroyableObject destroyableObject)
    {
        Debug.Log("Remove blocking object");
        if(destroyableObjects.Contains(destroyableObject))
        {
            destroyableObject.destroyed.RemoveListener(RemoveDestroyableObject);
            destroyableObjects.Remove(destroyableObject);
        }

        isBlocked = destroyableObjects.Count != 0;
    }

    private async void OpenDoor(PlayerController player)
    {
        //Debug.Log("Player : Door is open");
        int index = player.currentRoom.id == frontDoors[0].room.id ? 1 : 0;
        GameController.Instance.PlayerChangesRoom(frontDoors[index].room, frontDoors[index].transform.position);

        OpenDoorVisuals();
        doorCollider.enabled = false;
        doorUsers += 1;
        do {
            await Task.Delay(100); 
        } 
        while (player.automaticMovement);

        doorUsers--;
        if(doorUsers == 0)
        {
            CloseDoorVisuals();
            doorCollider.enabled = true;
        }
    }

    private async void OpenDoor(MonsterController monster)
    {
        int index = monster.currentRoom.id == frontDoors[0].room.id ? 1 : 0;
        GameController.Instance.MonsterChangesRoom(monster.id,frontDoors[index].room, frontDoors[index].transform.position);
        OpenDoorVisuals();
        doorUsers += 1;
        doorCollider.enabled = false;
        //Debug.Log("Monster : Door open");
        do {
            await Task.Delay(100); 
        }
        while (monster.state == MonsterState.ChangingRoom);

        doorUsers--;
        if (doorUsers == 0)
        {
            CloseDoorVisuals();
            doorCollider.enabled = true;
        }
        
    }

    private void OpenDoorVisuals()
    {
        if (doorUsers > 0) return;
            //Animation
        FMODUnity.RuntimeManager.PlayOneShot(openDoorSFX, transform.position);
    }
    private void CloseDoorVisuals()
    {
        //Animation
        FMODUnity.RuntimeManager.PlayOneShot(closeDoorSFX, transform.position);
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
        if(frontDoors[0].room != null && frontDoors[1].room != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 1f);
            Gizmos.DrawSphere(frontDoors[0].room.transform.position, 0.1f);
            Gizmos.DrawSphere(frontDoors[1].room.transform.position, 0.1f);
            Gizmos.DrawLine(frontDoors[1].room.transform.position, frontDoors[0].room.transform.position);
        }
        else
        {
            Gizmos.color = new Color(0.5f, 0, 0.4f, 0.5f);
            Gizmos.DrawCube(transform.position, doorCollider.bounds.size);
        }
    }
}
