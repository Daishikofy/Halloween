using UnityEngine;
using System;
using Unity.Mathematics;

[Serializable]
public enum PlayerMovement
{
    Continuous,
    Horizontal,
    Vertical,
    Automatic,
    None
}

public class PlayerController : MonoBehaviour {
    [Header("Object setup")]
    public GameObject[] candyPrefabs;
    [Header("Physics setup")]
    [SerializeField]
    private float walkingSpeed = 1;
    [SerializeField]
    private float runningSpeed = 3;
    [SerializeField]
    private float stamina = 5;
    [SerializeField]
    private PlayerMovement playerMovimentation = PlayerMovement.Continuous;
    [SerializeField]
    private RelativeJoint2D joint = null;
    public Transform raycastStart;

    [Header("Runtime variables")]
    public RoomObject currentRoom;
    public bool isHidding;
    public bool automaticMovement;

    //Physics and movement
    private float currentStamina;
    private float currentSpeed;
    private Rigidbody2D rb;
    private Vector2 playerMovement;
    private Vector2 playerDirection;
    private Action currentMovement;

    private Vector2 targetPoint;
    private float lastDistance;

    private bool isDragingObject;
    private bool hasStamina = true;

    public InventoryController inventory;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        SetMovement(playerMovimentation);
        SetDirection(new Vector2(0, -1));

        //Desable isDraging
        joint.connectedBody = null;
        joint.enabled = false;

        inventory = new InventoryController();
        currentStamina = stamina;
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
        rb.MovePosition(rb.position + (playerMovement * (currentSpeed / rb.mass)) * Time.deltaTime);
    }

    #region "MOVEMENTS"
    // MOVEMENTS BEGIN
    private void ContinuousMovement()
    {
        playerMovement.x = Input.GetAxisRaw("Horizontal");
        playerMovement.y = Input.GetAxisRaw("Vertical");
        if (playerMovement.x != 0 && playerMovement.y != 0)
            playerMovement.y = 0;
        if (Input.GetKey(KeyCode.LeftShift) && hasStamina)
        {
            currentSpeed = runningSpeed;
            currentStamina -= Time.deltaTime;
            if (currentStamina <= 0)
                hasStamina = false;
        }
        else
        {
            currentSpeed = walkingSpeed;
            if (!hasStamina)
            {
                currentStamina += Time.deltaTime;
                if (currentStamina >= stamina)
                    hasStamina = true;
            }
        }
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

    public void SetMovement(PlayerMovement movement)
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
            case PlayerMovement.None:
                currentMovement = () => { playerMovement = Vector2.zero; };
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
        if (isHidding)
        {
            Unhides(null);            
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(raycastStart.transform.position, playerDirection, 0.5f);
        //Debug.Log("fraction: " + hit.fraction);
        Debug.DrawRay(raycastStart.transform.position, playerDirection, Color.red, 0.5f);

        if (hit.collider == null)
        {
            Debug.Log("No interactions");
            DropCandy();
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

    public void CollectItem(CollectibleObject item)
    {
        //TODO: Animations
        inventory.GetObject(item.name, 1);
        currentRoom.RemoveItem(item);
    }

    private void DropCandy()
    {
        if (inventory.UseObject(CollectibleType.Candy.ToString()))
        {
            //TODO: Animations drop candy
            var index = UnityEngine.Random.Range(0, candyPrefabs.Length);
            var obj = Instantiate(candyPrefabs[index], transform.position, quaternion.identity);
            currentRoom.objectsInRoom.Enqueue(obj);
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

    public void Hides()
    {
        isHidding = true;
        currentRoom.isPlayerInRoom = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        SetMovement(PlayerMovement.None);
    }
    public void Unhides(DestroyableObject destroyable)
    {
        isHidding = false;
        currentRoom.isPlayerInRoom = true;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        GetComponent<SpriteRenderer>().enabled = true;
        playerDirection *= -1;
        if (destroyable)
            transform.position = destroyable.transform.position;
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

