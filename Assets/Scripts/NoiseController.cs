using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseController : MonoBehaviour
{
    #region Variables
    // Noise Components
    private Transform _transform;

    // Default data
    private Vector3 baseNoiseScale;
    public float lerpSpeed;

    // Dynamic data
    public Vector3 currentNoiseScale;
    #endregion

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        baseNoiseScale = _transform.localScale;
        currentNoiseScale = baseNoiseScale;
        lerpSpeed = 1.0f * Time.deltaTime;
    }

    public void UpdateNoiseRadius(float radiusMultiplier)
    {
        currentNoiseScale = Vector3.Lerp(currentNoiseScale, baseNoiseScale * radiusMultiplier, lerpSpeed);
        _transform.localScale = currentNoiseScale;
    }

    public void ProduceNoiseOnce()
    {

    }

    public void ProduceNoiseContinuous()
    {

    }
}
