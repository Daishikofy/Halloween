using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public enum CameraMovement
{
    Fixed,
    Horizontal,
    Vertical
}

public class CameraController : MonoBehaviour
{
    public Transform playerPosition;
    public float transitionSpeed;
    public CameraMovement cameraMovimentation = CameraMovement.Fixed;
    [Space]
    [Header("Runtime variables")]
    public float minPosition;
    public float maxPosition;

    private Vector3 cameraMovement;
    private Vector2 targetPoint;
    private float distance;
    private Action currentMovement;
    // Start is called before the first frame update
    void Start()
    {
        cameraMovement = transform.position;
        SetMovement(cameraMovimentation);
    }

    public void Setup(CameraMovement movement, Vector2 min, Vector2 max)
    {
        cameraMovimentation = movement;
        SetupMinMax(min, max);
        var aux = Utils.NearestPointOnSegment(transform.position, min, max);
        transform.position = new Vector3(aux.x, aux.y, -10);
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

    public async void GoTo(CameraMovement movimentation, Vector2 min, Vector2 max)
    {
        targetPoint = Utils.NearestPointOnSegment(transform.position, min, max);        

        cameraMovimentation = movimentation;
        Vector2 startPoint = transform.position;

        cameraMovement = (targetPoint - startPoint).normalized;
        distance = Vector2.Distance(transform.position, targetPoint);

        float time = distance / transitionSpeed;
        transform.LeanMove(targetPoint, time);
        await Task.Delay((int)time*1000);
        SetupMinMax(min, max);
        SetMovement(cameraMovimentation);
    }

    private void SetupMinMax(Vector2 min, Vector2 max)
    {
        if (cameraMovimentation == CameraMovement.Horizontal)
        {
            cameraMovement.y = min.y;
            minPosition = min.x;
            maxPosition = max.x;
        }
        else
        {
            cameraMovement.x = min.x;
            minPosition = min.y;
            maxPosition = max.y;
        }
    }
}
