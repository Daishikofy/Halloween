using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public enum PlayerMovement
{
    Continuous,
    Inverted,
    Horizontal,
    Vertical
}

public class PlayerController : MonoBehaviour {



    [Header("Physics setup")]
    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private PlayerMovement playerMovimentation = PlayerMovement.Continuous;
    [SerializeField]
    private RelativeJoint2D joint = null;

    //Physics and movement
    private Rigidbody2D rb;
    private Vector2 playerMovement;
    private Vector2 playerDirection;
    private Action currentMovement;

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
        playerMovement *= (speed / rb.mass);
        rb.MovePosition(rb.position + playerMovement * Time.deltaTime);
    }

    #region "MOVEMENTS"
    // MOVEMENTS BEGIN
    private void ContinuousMovement()
    {
        playerMovement.x = Input.GetAxisRaw("Horizontal");
        playerMovement.y = Input.GetAxisRaw("Vertical");
        if (playerMovement.x > 0 && playerMovement.y > 0)
            playerMovement.y = 0;
        SetDirection(playerMovement);
    }

    private void InvertedMovement()
    {
        playerMovement.x = -Input.GetAxisRaw("Horizontal");
        playerMovement.y = -Input.GetAxisRaw("Vertical");
        if (playerMovement.x > 0 && playerMovement.y > 0)
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

    private void SetDirection(Vector2 direction)
    {
        if (direction == playerDirection || direction.x + direction.y == 0)
            return;
        playerDirection = direction;
    }

    private void SetMovement(PlayerMovement movement)
    {
        switch (movement)
        {
            case PlayerMovement.Continuous:
                currentMovement = () => ContinuousMovement();
                break;
            case PlayerMovement.Inverted:
                currentMovement = () => InvertedMovement();
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

    // MOVEMENTS END
    #endregion 

    private void Interact()
    {
        Vector3 startPoint = transform.position + Vector3.up * 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(startPoint, playerDirection, 0.5f);
        Debug.Log("fraction: " + hit.fraction);
        Debug.DrawRay(startPoint, playerDirection, Color.red, 0.5f);

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

        isDragingObject = false;
        rb.mass -= otherRb.mass;
        joint.connectedBody = null;
        joint.enabled = false;
        otherRb.velocity = Vector2.zero;

        SetMovement(PlayerMovement.Continuous);
    }

}

