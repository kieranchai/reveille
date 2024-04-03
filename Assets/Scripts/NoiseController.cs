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
    public bool isActivated = false;
    public bool isPulsing = false;
    public float pulseTimer;

    public AudioClip alarmSound;
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

    private void Update()
    {
        if (isPulsing)
        {
            pulseTimer += Time.deltaTime;

            if (pulseTimer >= 8.0f) DeactivateNoise();
        }

        if (GameController.instance.gameEnd && GetComponentInParent<AudioSource>().isPlaying) GetComponentInParent<AudioSource>().Stop();
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

    public void DeactivateNoise()
    {
        isActivated = false;
        if (GetComponentInParent<AudioSource>().isPlaying)
        {
            GetComponentInParent<AudioSource>().Stop();
        }
        StopCoroutine(ProduceNoiseTimer());
        StopNoise();
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

    public IEnumerator ProduceNoiseTimer()
    {
        _collider.enabled = true;
        isPulsing = true;
        isActivated = true;
        float noiseShrinkTime, noiseShrinkDuration, noiseShrinkLerpSpeed;

        while (isActivated)
        {
            float noiseSpreadTime = 0.0f;
            float noiseSpreadDuration = 0.8f;
            float noiseSpreadLerpSpeed = 12.0f * Time.deltaTime;
            while (noiseSpreadTime < noiseSpreadDuration)
            {
                noiseSpreadTime += Time.deltaTime;
                currentNoiseScale = Vector3.Lerp(currentNoiseScale, baseNoiseScale, noiseSpreadLerpSpeed);
                _transform.localScale = currentNoiseScale;
                yield return null;
            }

            if (!GetComponentInParent<AudioSource>().isPlaying)
            {
                GetComponentInParent<AudioSource>().clip = alarmSound;
                GetComponentInParent<AudioSource>().Play();
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
    }

    public void StopNoise()
    {
        pulseTimer = 0.0f;
        isPulsing = false;
        isActivated = false;
        currentNoiseScale = Vector3.zero;
        _transform.localScale = Vector3.zero;
        _collider.enabled = false;
    }
}
