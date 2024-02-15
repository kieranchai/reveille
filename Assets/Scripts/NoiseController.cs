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
    public bool isDeactivated = false;
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

    public IEnumerator ProduceNoiseMultiple(int numberOfTimes)
    {
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
    }

    public IEnumerator ProduceNoiseInfinitely()
    {
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

    public IEnumerator StopNoise()
    {
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
}
