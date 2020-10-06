using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum MonsterState
{
    Traped,
    Blocked,
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
    public float breakTime;
    public float patrolCycleTime;

    [Header("Object Setup")]
    public Rigidbody2D rb;
    public PlayerController player;
    public RoomObject currentRoom;

    [Header("Runtime Variables")]
    public MonsterState state;
    public bool isChasingPlayer;

    private Action currentMovement;
    private Vector2 monsterMovement;
    private Vector2 monsterDirection;
    private float currentSpeed;
    private Vector2 targetPoint;

    private float breakTimeEnd;
    private float patrolCycleEnd;

    // Start is called before the first frame update
    void Start()
    {
        SetMovement(state);
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

    private void SetMovement(MonsterState movement)
    {
        switch (movement)
        {
            case MonsterState.Traped:
                break;
            case MonsterState.Blocked:
                break;
            case MonsterState.Patroling:
                currentSpeed = normalSpeed;
                currentMovement = () => Patroling();
                break;
            case MonsterState.BackToPatrol:
                break;
            case MonsterState.Chasing:
                currentSpeed = chaseSpeed;
                currentMovement = () => ChasePlayer();
                break;
            case MonsterState.Eating:
                break;
            default:
                break;
        }
    }

        public void TryNextPatrolRoom()
    {
        //If heading back to patrol, next made up room
        //else next patrol room
    }

    public void AttackDoor(DoorObject door)
    {
        //Animation, sound
        BlockingObject blockingObject = null;
        float minDistance = 1000;
        foreach (var blocking in door.blockingObjects)
        {
            float distance = Vector3.Distance(blocking.transform.position, this.transform.position);
            if (Vector3.Distance(blocking.transform.position, this.transform.position) < minDistance)
                blockingObject = blocking;
        }
        blockingObject.Damaged();
    }

    public void ChasePlayer()
    {
        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
            targetPoint = NewTargetPlayerPoint();
        monsterMovement = targetPoint - (Vector2)transform.position;

        monsterMovement = monsterMovement.normalized;
        SetDirection(monsterMovement);
    }

    public void Patroling()
    {
        if (Time.time < breakTimeEnd)
            return;

        monsterMovement = targetPoint - (Vector2)transform.position;

        monsterMovement = monsterMovement.normalized;
        SetDirection(monsterMovement);

        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            targetPoint = NewRandomTargetPoint();
            breakTimeEnd = Time.time + breakTime;
            monsterMovement = Vector2.zero;
        }
    }

    private void SetDirection(Vector2 direction)
    {
        if (direction == monsterDirection || direction.x + direction.y == 0)
            return;
        monsterDirection = direction;
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
            target.y = value;
            target.x = UnityEngine.Random.Range(currentRoom.cameraMin.transform.position.x
                , currentRoom.cameraMax.transform.position.x);
        }
        else if (currentRoom.type == CameraMovement.Vertical)
        {
            target.x = value;
            target.y = UnityEngine.Random.Range(currentRoom.cameraMin.transform.position.y
                , currentRoom.cameraMax.transform.position.y);
        }
        else
        {
            target.x = value;
            target.y = UnityEngine.Random.Range(-1, 2);
        }
        return target;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPoint, 0.3f);
    }
}
