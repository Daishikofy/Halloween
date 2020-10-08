using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Unity.Mathematics;

[Serializable]
public enum PlayerMovement
{
    Continuous,
    Horizontal,
    Vertical,
    Automatic
}

public class PlayerController : MonoBehaviour {
    [Header("Physics setup")]
    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private PlayerMovement playerMovimentation = PlayerMovement.Continuous;
    [SerializeField]
    private RelativeJoint2D joint = null;

    [Header("Runtime variables")]
    public RoomObject currentRoom;
    public bool automaticMovement;

    //Physics and movement
    private Rigidbody2D rb;
    private Vector2 playerMovement;
    private Vector2 playerDirection;
    private Action currentMovement;

    private Vector2 targetPoint;
    private float lastDistance;

    private Boolean isDragingObject;

    public InventoryController inventory;

    // Use this for initialization
    void Start () {
        
        rb = GetComponent<Rigidbody2D>();
        SetMovement(playerMovimentation);
        SetDirection(new Vector2(0, -1));

        //Desable isDraging
        joint.connectedBody = null;
        joint.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E))
            Interact();
        else if (Input.GetKeyUp(KeyCode.E))
            ReleaseInteraction();

        currentMovement();
	}
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + (playerMovement * (speed / rb.mass)) * Time.deltaTime);
    }

    #region "MOVEMENTS"
    // MOVEMENTS BEGIN
    private void ContinuousMovement()
    {
        playerMovement.x = Input.GetAxisRaw("Horizontal");
        playerMovement.y = Input.GetAxisRaw("Vertical");
        if (playerMovement.x != 0 && playerMovement.y != 0)
            playerMovement.y = 0;
        SetDirection(playerMovement);
    }

    private void HorizontalMovement()
    {
        playerMovement.x = Input.GetAxisRaw("Horizontal");
        playerMovement.y = 0;
        SetDirection(playerMovement);
    }
    private void VerticalMovement()
    {
        playerMovement.x = 0;
        playerMovement.y = Input.GetAxisRaw("Vertical");
        SetDirection(playerMovement);
    }

    private void AutomaticMovement()
    {
        var distance = Vector2.Distance(transform.position, targetPoint);
        if (lastDistance < distance)
        {
            playerMovement = Vector2.zero;
            automaticMovement = false;
            SetMovement(PlayerMovement.Continuous);
        }
        lastDistance = distance;
    }

    private void SetDirection(Vector2 direction)
    {
        if (direction == playerDirection || direction.x + direction.y == 0)
            return;
        playerDirection = direction;
    }

    private void SetMovement(PlayerMovement movement)
    {
        playerMovimentation = movement;
        switch (movement)
        {
            case PlayerMovement.Continuous:
                currentMovement = () => ContinuousMovement();
                break;
            case PlayerMovement.Automatic:
                currentMovement = () => AutomaticMovement();
                break;
            case PlayerMovement.Horizontal:
                currentMovement = () => HorizontalMovement();
                break;
            case PlayerMovement.Vertical:
                currentMovement = () => VerticalMovement();
                break;
            default:
                break;
        }
    }

    public void GoTo(Vector2 destination)
    {
        targetPoint = destination; 

        float deltaX = math.abs(rb.position.x - destination.x);
        float deltaY = math.abs(rb.position.y - destination.y);

        Vector2 startPoint = rb.position;
        if (deltaX > deltaY)
            startPoint.y = destination.y;
        else
            startPoint.x = destination.x;

        transform.position = startPoint;

        playerMovement = (destination - startPoint).normalized;
        lastDistance = Vector2.Distance(transform.position, targetPoint);

        SetDirection(playerMovement);
        automaticMovement = true;
        SetMovement(PlayerMovement.Automatic);
    }

    // MOVEMENTS END
    #endregion 

    private void Interact()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection, 0.5f);
        //Debug.Log("fraction: " + hit.fraction);
        Debug.DrawRay(transform.position, playerDirection, Color.red, 0.5f);

        if (hit.collider == null)
        {
            //TODO: Drop some candies maybe
            return;
        }
        var other = hit.collider.gameObject;
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.OnInteraction(this);
        }
        else if (other.GetComponent<IDragable>() != null)
        {
            GrabObject(other);
        }
    }

    private void ReleaseInteraction()
    {
        if (isDragingObject)
            ReleaseObject();
    }

    private void GrabObject(GameObject other)
    {
        var otherRb = other.GetComponent<Rigidbody2D>();

        otherRb.bodyType = RigidbodyType2D.Dynamic;
        isDragingObject = true;
        rb.mass += otherRb.mass;
        joint.connectedBody = otherRb;
        joint.enabled = true;

        if (playerDirection.x != 0)
            SetMovement(PlayerMovement.Horizontal);
        else
            SetMovement(PlayerMovement.Vertical);
    }

    private void ReleaseObject()
    {
        var otherRb = joint.connectedBody;

        otherRb.bodyType = RigidbodyType2D.Kinematic;
        isDragingObject = false;
        rb.mass -= otherRb.mass;
        joint.connectedBody = null;
        joint.enabled = false;
        otherRb.velocity = Vector2.zero;

        SetMovement(PlayerMovement.Continuous);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            GameController.Instance.OnPlayerLose();
        }
    }

}

