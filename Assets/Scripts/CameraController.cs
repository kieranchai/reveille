using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Camera follow target
    public Transform target;

    Vector3 velocity = Vector3.zero;

    [Range(0f, 1f)]
    public float smoothTime;

    // Camera offset
    public Vector3 positionOffset;

    // Camera X and Y bounds
    public Vector2 xLimit;
    public Vector2 yLimit;

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + positionOffset;
        targetPosition = new Vector3(

            // X bounds
            Mathf.Clamp(targetPosition.x, xLimit.x, xLimit.y), 

            // Y Bounds
            Mathf.Clamp(targetPosition.y, yLimit.x, yLimit.y), -10);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}