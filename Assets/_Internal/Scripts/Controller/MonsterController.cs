using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public enum MonsterState
{
    Traped,
    Blocked,
    GoingToDoor,
    ChangingRoom,
    CheckingRoom,
    Patroling,    
    BackToPatrol,
    Chasing,
    Eating
}

public class MonsterController : MonoBehaviour
{
    public RoomObject[] patrolPath;

    public float normalSpeed = 1;
    public float chaseSpeed = 2;
    public float agility = 1;
    [Space]
    public float breakTime = 1;
    public float patrolCycleRate = 10;
    public float attackRate = 5;
    public float timeBeforeSpotted = 1;

    [Header("Object Setup")]
    public int id;
    public Rigidbody2D rb;
    public Collider2D monsterCollider;
    public PlayerController player;
    public RoomObject currentRoom;

    [Header("Runtime Variables")]
    public MonsterState state;
    public bool isChasingPlayer;

    private int currentPatrolRoomIndex;
    private Action currentMovement;
    private Queue<MonsterState> nextState;
    private Vector2 monsterMovement;
    private Vector2 monsterDirection;
    private float currentSpeed;
    private Vector2 targetPoint;

    private DoorObject targetDoor;
    private BlockingObject targetObject;

    private float breakTimeEnd;
    private float patrolCycleEnd;

    // Start is called before the first frame update
    void Start()
    {
        nextState = new Queue<MonsterState>();
        targetPoint = currentRoom.cameraMax.position;
        patrolCycleEnd = Time.time + patrolCycleRate;
        SetState(state);
    }

    // Update is called once per frame
    void Update()
    {
        currentMovement();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + (monsterMovement * currentSpeed) * Time.deltaTime);
    }

    private void SetState(MonsterState newState)
    {
        Debug.Log("State : " + newState);
        switch (newState)
        {
            case MonsterState.Traped:
                break;

            case MonsterState.Blocked:
                state = newState;
                currentSpeed = 0;
                GoToNextRoom();
                break;

            case MonsterState.Eating:
                break;


            case MonsterState.Chasing:
                currentSpeed = chaseSpeed;
                state = newState;
                currentMovement = () => ChasePlayer();
                break;

            case MonsterState.GoingToDoor:

                if (currentSpeed < normalSpeed)
                    currentSpeed = normalSpeed;
                
                if (targetDoor != null)
                {
                    targetPoint = targetDoor.frontDoors[0].room.id == currentRoom.id ?
                        targetDoor.frontDoors[0].transform.position :
                        targetDoor.frontDoors[1].transform.position;
                    
                    nextState.Enqueue(MonsterState.Blocked);
                    nextState.Enqueue(MonsterState.CheckingRoom);
                    nextState.Enqueue(state);
                    state = newState;
                    SetTargetPoint(targetPoint);
                    SetDirection(monsterMovement);
                    
                    currentMovement = () => WalkToTarget();
                }
                else 
                {
                    currentMovement = () => Patroling();
                }
                break;

            case MonsterState.ChangingRoom:
                state = newState;
                currentSpeed = normalSpeed;
                break;

            case MonsterState.CheckingRoom:
                state = newState;
                currentSpeed = 0;
                CheckCurrentRoom();
                break;
            case MonsterState.Patroling:
                state = newState;
                currentSpeed = normalSpeed;
                patrolCycleEnd = Time.time + patrolCycleRate;
                currentMovement = () => Patroling();
                
                break;

            case MonsterState.BackToPatrol:
                break;

            default:
                break;
        }
    }

    private void SetDirection(Vector2 direction)
    {
        if (direction == monsterDirection || direction.x + direction.y == 0)
            return;
        monsterDirection = direction;
    }

    public async void CheckCurrentRoom()
    {
        MonsterState next = MonsterState.Patroling;
        if (nextState.Count != 0)
             next = nextState.Dequeue();
        await Task.Delay((int)(timeBeforeSpotted * 1000));
        if (currentRoom.isPlayerInRoom && next != MonsterState.Chasing)
            SetState(MonsterState.Chasing);
        else if (!currentRoom.isPlayerInRoom && next == MonsterState.Chasing)
            SetState(MonsterState.Patroling);
        else
            SetState(next);
        Debug.Log("CheckCurrentRoom: " + state);
    }

    public async void GoToNextRoom()
    {
        while (targetDoor.blockingObjects.Count > 0)
        {
            float minDist = Vector2.Distance(transform.position, targetPoint);
            targetObject = null;
            foreach (var blocking in targetDoor.blockingObjects)
            {
                if (Math.Abs(Vector2.Distance(transform.position, blocking.transform.position) - minDist) < 1)
                {
                    targetObject = blocking;
                    targetPoint = (transform.position - blocking.transform.position).normalized * 0.5f;
                }
                else if (targetObject == null)
                {
                    targetObject = targetDoor.blockingObjects[0];
                    targetPoint = targetDoor.frontDoors[0].room.id == currentRoom.id ?
                        targetDoor.frontDoors[0].transform.position :
                        targetDoor.frontDoors[1].transform.position;
                }
            }

            await DestroyTargetObject();
        }

        if (targetDoor.OnInteraction(this))
        {
            SetState(MonsterState.ChangingRoom);  
        }
    }

    public void ChasePlayer()
    {
        if (player.currentRoom.id != currentRoom.id)
        {
            GameController.Instance.MonsterFollowsPlayer(id);
        }
        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
            targetPoint = NewTargetPlayerPoint();
        monsterMovement = targetPoint - (Vector2)transform.position;

        monsterMovement = monsterMovement.normalized;
        SetDirection(monsterMovement);
    }

    public void GoToRoom(int roomId)
    {
        targetDoor = currentRoom.GetDoorToAdjacentRoom(roomId);
        if (targetDoor == null) return;
        Debug.LogError("Target door to room " + roomId + " is: " + targetDoor.name);
        SetState(MonsterState.GoingToDoor);
    }

    public void Patroling()
    {
        if (Time.time > patrolCycleEnd)
        {
            currentPatrolRoomIndex = (currentPatrolRoomIndex + 1) % patrolPath.Length;

            targetDoor = currentRoom.GetDoorToAdjacentRoom(patrolPath[currentPatrolRoomIndex].id);
            SetState(MonsterState.GoingToDoor);
        }
        if (Time.time < breakTimeEnd)
            return;

        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            targetPoint = NewRandomTargetPoint();
            breakTimeEnd = Time.time + breakTime;
            monsterMovement = Vector2.zero;
        }
        else
        {
            monsterMovement = targetPoint - (Vector2)transform.position;

            monsterMovement = monsterMovement.normalized;
            SetDirection(monsterMovement);
        }
    }

    private async Task DestroyTargetObject()
    {
        while (targetObject.lifePoints > 0 && targetDoor.blockingObjects.Contains(targetObject))
        {
            await Task.Delay((int)(attackRate * 1000));
            targetObject.Damaged();
        }
    }

    public void SetTargetPoint(Vector2 point)
    {
        targetPoint = point;

        monsterMovement = targetPoint - (Vector2)transform.position;
        monsterMovement = monsterMovement.normalized;
    }
    public void WalkToTarget()
    {
        if (monsterMovement != Vector2.zero && Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            monsterMovement = Vector2.zero;
            SetState(nextState.Dequeue());
            Debug.Log("Walk to target: ");
        }
    }

    private Vector2 NewTargetPlayerPoint()
    {
        var targetDistance = 1 / agility;
        var M2P = player.transform.position - transform.position;
        if (M2P.magnitude > targetDistance)
            M2P = M2P.normalized * targetDistance;
        return transform.position + M2P;
    }

    private Vector2 NewRandomTargetPoint()
    {
        Vector2 target;
        float value = UnityEngine.Random.Range(-1,2);
        if (currentRoom.type == CameraMovement.Horizontal)
        {
            target.y = currentRoom.cameraMin.transform.position.y + value;
            target.x = UnityEngine.Random.Range(currentRoom.cameraMin.transform.position.x
                , currentRoom.cameraMax.transform.position.x);
        }
        else if (currentRoom.type == CameraMovement.Vertical)
        {
            target.x = currentRoom.cameraMin.transform.position.x + value;
            target.y = UnityEngine.Random.Range(currentRoom.cameraMin.transform.position.y
                , currentRoom.cameraMax.transform.position.y);
        }
        else
        {
            target.x = currentRoom.cameraMin.transform.position.x + value;
            target.y = currentRoom.cameraMin.transform.position .y + UnityEngine.Random.Range(-1, 2);
        }
        return target;
    }

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == MonsterState.GoingToDoor && collision.gameObject.CompareTag("BlockingObject"))
        {           
            if (Vector2.Distance(transform.position, targetPoint) < 1)
            {
                targetObject = collision.gameObject.GetComponent<BlockingObject>();
                float aux = currentSpeed;
                currentSpeed = 0;
                await DestroyTargetObject();
                currentSpeed = aux;
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            if(state == MonsterState.Chasing)
            {
                //TODO: The monster won
                currentSpeed = 0;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(targetPoint, 0.3f);
    }
}
