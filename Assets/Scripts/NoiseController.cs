using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseController : MonoBehaviour
{
    #region Variables
    // Noise Components
    private Transform _transform;
    private Collider2D _collider;

    // Default data
    private Vector3 baseNoiseScale;
    public float lerpSpeed;

    // Dynamic data
    private Vector3 currentNoiseScale;
    public bool isDeactivated = false;
    #endregion

    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _collider = GetComponent<Collider2D>();
        baseNoiseScale = _transform.localScale;
        _transform.localScale = Vector3.zero;
        _collider.enabled = false;
        currentNoiseScale = _transform.localScale;
        lerpSpeed = 1.0f * Time.deltaTime;
    }

    public void ResetNoise()
    {
        _transform.localScale = Vector3.zero;
        _collider.enabled = false;
        currentNoiseScale = _transform.localScale;
    }

    public void UpdateNoiseRadius(float radiusMultiplier)
    {
        _collider.enabled = true;
        currentNoiseScale = Vector3.Lerp(currentNoiseScale, baseNoiseScale * radiusMultiplier, lerpSpeed);
        _transform.localScale = currentNoiseScale;
    }

    public IEnumerator ProduceNoiseOnce()
    {
        _collider.enabled = true;
        float noiseSpreadTime = 0.0f;
        float noiseSpreadDuration = 0.3f;
        float noiseSpreadLerpSpeed = 12.0f * Time.deltaTime;
        while (noiseSpreadTime < noiseSpreadDuration)
        {
            noiseSpreadTime += Time.deltaTime;
            currentNoiseScale = Vector3.Lerp(currentNoiseScale, baseNoiseScale, noiseSpreadLerpSpeed);
            _transform.localScale = currentNoiseScale;
            yield return null;
        }

        float noiseShrinkTime = 0.0f;
        float noiseShrinkDuration = 0.1f;
        float noiseShrinkLerpSpeed = 9.0f * Time.deltaTime;
        while (noiseShrinkTime < noiseShrinkDuration)
        {
            noiseShrinkTime += Time.deltaTime;
            currentNoiseScale = Vector3.Lerp(currentNoiseScale, Vector3.zero, noiseShrinkLerpSpeed);
            _transform.localScale = currentNoiseScale;
            yield return null;
        }
        _transform.localScale = Vector3.zero;
        _collider.enabled = false;
    }

    public IEnumerator ProduceNoiseMultiple(int numberOfTimes)
    {
        _collider.enabled = true;
        for (int i = 0; i < numberOfTimes; i++)
        {
            float noiseSpreadTime = 0.0f;
            float noiseSpreadDuration = 0.3f;
            float noiseSpreadLerpSpeed = 12.0f * Time.deltaTime;
            while (noiseSpreadTime < noiseSpreadDuration)
            {
                noiseSpreadTime += Time.deltaTime;
                currentNoiseScale = Vector3.Lerp(currentNoiseScale, baseNoiseScale, noiseSpreadLerpSpeed);
                _transform.localScale = currentNoiseScale;
                yield return null;
            }

            float noiseShrinkTime = 0.0f;
            float noiseShrinkDuration = 0.1f;
            float noiseShrinkLerpSpeed = 9.0f * Time.deltaTime;
            while (noiseShrinkTime < noiseShrinkDuration)
            {
                noiseShrinkTime += Time.deltaTime;
                currentNoiseScale = Vector3.Lerp(currentNoiseScale, Vector3.zero, noiseShrinkLerpSpeed);
                _transform.localScale = currentNoiseScale;
                yield return null;
            }
            _transform.localScale = Vector3.zero;
        }
        _collider.enabled = false;
    }

    public IEnumerator ProduceNoiseInfinitely()
    {
        _collider.enabled = true;
        float noiseShrinkTime, noiseShrinkDuration, noiseShrinkLerpSpeed;

        while (!isDeactivated)
        {
            float noiseSpreadTime = 0.0f;
            float noiseSpreadDuration = 0.3f;
            float noiseSpreadLerpSpeed = 12.0f * Time.deltaTime;
            while (noiseSpreadTime < noiseSpreadDuration)
            {
                noiseSpreadTime += Time.deltaTime;
                currentNoiseScale = Vector3.Lerp(currentNoiseScale, baseNoiseScale, noiseSpreadLerpSpeed);
                _transform.localScale = currentNoiseScale;
                yield return null;
            }

            noiseShrinkTime = 0.0f;
            noiseShrinkDuration = 0.1f;
            noiseShrinkLerpSpeed = 9.0f * Time.deltaTime;
            while (noiseShrinkTime < noiseShrinkDuration)
            {
                noiseShrinkTime += Time.deltaTime;
                currentNoiseScale = Vector3.Lerp(currentNoiseScale, Vector3.zero, noiseShrinkLerpSpeed);
                _transform.localScale = currentNoiseScale;
                yield return null;
            }
            _transform.localScale = Vector3.zero;
        }

        yield return StopNoise();
    }

    public IEnumerator StopNoise()
    {
        _collider.enabled = true;
        float noiseShrinkTime = 0.0f;
        float noiseShrinkDuration = 0.1f;
        float noiseShrinkLerpSpeed = 9.0f * Time.deltaTime;
        while (noiseShrinkTime < noiseShrinkDuration)
        {
            noiseShrinkTime += Time.deltaTime;
            currentNoiseScale = Vector3.Lerp(currentNoiseScale, Vector3.zero, noiseShrinkLerpSpeed);
            _transform.localScale = currentNoiseScale;
            yield return null;
        }
        _transform.localScale = Vector3.zero;
        _collider.enabled = false;
    }
}
