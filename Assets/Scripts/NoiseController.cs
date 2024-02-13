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
    private Vector3 currentNoiseScale;
    #endregion

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        baseNoiseScale = _transform.localScale;
        _transform.localScale = Vector3.zero;
        currentNoiseScale = _transform.localScale;
        lerpSpeed = 1.0f * Time.deltaTime;
    }

    public void UpdateNoiseRadius(float radiusMultiplier)
    {
        currentNoiseScale = Vector3.Lerp(currentNoiseScale, baseNoiseScale * radiusMultiplier, lerpSpeed);
        _transform.localScale = currentNoiseScale;
    }

    public IEnumerator ProduceNoiseOnce()
    {
        while (currentNoiseScale.sqrMagnitude < baseNoiseScale.sqrMagnitude)
        {
            currentNoiseScale = Vector3.Lerp(currentNoiseScale, baseNoiseScale, lerpSpeed);
            _transform.localScale = currentNoiseScale;
            yield return null;
        }

        while (currentNoiseScale.sqrMagnitude > Vector3.zero.sqrMagnitude)
        {
            currentNoiseScale = Vector3.Lerp(currentNoiseScale, Vector3.zero, lerpSpeed);
            _transform.localScale = currentNoiseScale;
            yield return null;
        }
    }

    public void ProduceNoiseContinuous()
    {

    }
}
