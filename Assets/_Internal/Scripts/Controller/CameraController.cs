using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public enum CameraMovement
{
    Fixed,
    Horizontal,
    Vertical,
    Automatic
}

public class CameraController : MonoBehaviour
{
    public CameraMovement cameraMovimentation = CameraMovement.Fixed;
    public float transitionSpeed;
    public Transform playerPosition;

    [Header("Runtime variables")]
    public float minPosition;
    public float maxPosition;

    private Vector3 cameraMovement;
    private Vector2 targetPoint;
    private float lastDistance;
    private Action currentMovement;
    // Start is called before the first frame update
    void Start()
    {
        cameraMovement = transform.position;
        SetMovement(cameraMovimentation);
    }

    // Update is called once per frame
    void Update()
    {
        currentMovement();
    }

    private void FixedMovement()
    {
        return;
    }
    private void AutomaticMovement()
    {
        var distance = Vector2.Distance(transform.position, targetPoint);
        if (lastDistance < distance)
        {
            cameraMovement = Vector2.zero;
            SetMovement(cameraMovimentation);
        }
        lastDistance = distance;
    }
    private void HorizontalMovement()
    {
        if (playerPosition.position.x < minPosition || playerPosition.position.x > maxPosition)
            return;
        cameraMovement.x = playerPosition.position.x;
        cameraMovement.z = -10;
        transform.position = cameraMovement;
    }
    private void VerticalMovement()
    {
        if (playerPosition.position.y < minPosition || playerPosition.position.y > maxPosition)
            return;
        cameraMovement.y = playerPosition.position.y;
        cameraMovement.z = -10;
        transform.position = cameraMovement;
    }
    private void SetMovement(CameraMovement movement)
    {
        switch (movement)
        {
            case CameraMovement.Fixed:
                currentMovement = () => FixedMovement();
                break;
            case CameraMovement.Automatic:
                currentMovement = () => AutomaticMovement();
                break;
            case CameraMovement.Horizontal:
                currentMovement = () => HorizontalMovement();
                break;
            case CameraMovement.Vertical:
                currentMovement = () => VerticalMovement();
                break;
            default:
                break;
        }
    }

    public void GoTo(CameraMovement movimentation, Vector2 min, Vector2 max)
    {
        //Get heading
        Vector2 heading = (max - min);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector2 lhs = (Vector2)transform.position - min;
        float dotP = Vector2.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);

        targetPoint = min + heading * dotP;        

        cameraMovimentation = movimentation;
        if (cameraMovimentation == CameraMovement.Horizontal)
        {
            minPosition = min.x;
            maxPosition = max.x;
        }
        else
        {
            minPosition = min.y;
            maxPosition = max.y;
        }
        if (minPosition > maxPosition)
        {
            var aux = minPosition;
            minPosition = maxPosition;
            maxPosition = minPosition;
        }
        /*
        float deltaX = math.abs(transform.position.x - targetPoint.x);
        float deltaY = math.abs(transform.position.y - targetPoint.y);
        */
        Vector2 startPoint = transform.position;
        /*if (deltaX > deltaY)
            startPoint.y = targetPoint.y;
        else
            startPoint.x = targetPoint.x;

        transform.position = startPoint;*/

        cameraMovement = (targetPoint - startPoint).normalized;
        lastDistance = Vector2.Distance(transform.position, targetPoint);

        //SetMovement(CameraMovement.Automatic);
        transform.LeanMove(targetPoint, (lastDistance / transitionSpeed));
    }
}
