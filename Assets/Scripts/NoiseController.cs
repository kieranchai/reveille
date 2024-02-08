using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseController : MonoBehaviour
{
    // Noise Components
    private CircleCollider2D _collider;

    // Default data
    private float baseColliderRadius;
    public float lerpSpeed;

    // Dynamic data
    public float currentColliderRadius;

    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
        baseColliderRadius = _collider.radius;
        currentColliderRadius = baseColliderRadius;
        lerpSpeed = 3.0f * Time.deltaTime;
    }

    public void UpdateNoiseRadius(float radiusMultiplier)
    {
        currentColliderRadius = Mathf.Lerp(currentColliderRadius, baseColliderRadius * radiusMultiplier, lerpSpeed);
        _collider.radius = currentColliderRadius;
    }

    public void ProduceNoiseOnce()
    {

    }

    public void ProduceNoiseContinuous()
    {

    }

    //On Trigger
}
