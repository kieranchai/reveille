using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Camera follow target
    public Transform target;

    Vector3 velocity = Vector3.zero;

    private float smoothTime;

    // Camera offset
    public Vector3 positionOffset;

    // Camera X and Y bounds
    public Vector2 xLimit;
    public Vector2 yLimit;

    [Header("Foodpanda Variables")]
    public GameObject foodPanda;
    public AudioSource _audioSourceFoodPanda;
    public AudioClip foodpandaFootsteps;

    private float initialZoom;
    private float playerSmoothTime = 0.3f;
    private float introSmoothTime = 1f;
    private Transform foodSpawnArea;
    private Vector3 initialFoodPandaPos;

    private void Start()
    {
        initialZoom = Camera.main.orthographicSize;
        foodSpawnArea = GameObject.Find("Food Spawn Area").transform;

        foodPanda = GameObject.Find("Foodpanda");
        _audioSourceFoodPanda = foodPanda.GetComponent<AudioSource>();

        if (GameController.instance.isPanning) StartCoroutine(PanMap());
    }

    private void Update()
    {
        if (GameController.instance.isPanning)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) SkipPanning();
        }

        if (foodPanda.GetComponent<Animator>().GetBool("isWalking"))
        {
            if (!_audioSourceFoodPanda.isPlaying)
            {
                _audioSourceFoodPanda.clip = foodpandaFootsteps;
                _audioSourceFoodPanda.Play();
            }
        }
        else
        {
            _audioSourceFoodPanda.Stop();
        }
    }

    private void FixedUpdate()
    {
        if (target)
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

    IEnumerator ZoomInCamera()
    {
        float elapsed = 0;
        float currentZoom = Camera.main.orthographicSize;
        while (elapsed <= 1)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 1);
            Camera.main.orthographicSize = Mathf.Lerp(currentZoom, initialZoom, t);
            yield return null;
        }
    }

    IEnumerator PanMap()
    {
        smoothTime = introSmoothTime;
        Camera.main.orthographicSize = 13;
        GameController.instance.currentUIManager.HideAllUI();

        initialFoodPandaPos = foodPanda.transform.position;
        target = foodSpawnArea;

        while (Vector3.SqrMagnitude(foodPanda.transform.position - foodSpawnArea.position) >= 0.05f)
        {
            foodPanda.GetComponent<Animator>().SetBool("isWalking", true);
            var dir = foodSpawnArea.position - foodPanda.transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            foodPanda.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            foodPanda.transform.position = Vector3.MoveTowards(foodPanda.transform.position, foodSpawnArea.position, 8 * Time.deltaTime);
            yield return null;
        }
        foodPanda.GetComponent<Animator>().SetBool("isWalking", false);
        yield return new WaitForSeconds(0.5f);
        GameController.instance.currentLevelController.SpawnFood();
        yield return new WaitForSeconds(0.5f);
        while (Vector3.SqrMagnitude(foodPanda.transform.position - initialFoodPandaPos) >= 0.05f)
        {
            foodPanda.GetComponent<Animator>().SetBool("isWalking", true);
            var dir = initialFoodPandaPos - foodPanda.transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            foodPanda.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            foodPanda.transform.position = Vector3.MoveTowards(foodPanda.transform.position, initialFoodPandaPos, 8 * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        _audioSourceFoodPanda.Stop();

        target = PlayerManager.instance.gameObject.transform;
        yield return new WaitForSeconds(1.0f);
        yield return ZoomInCamera();
        smoothTime = playerSmoothTime;
        StartGame();
    }

    private void SkipPanning()
    {
        StopAllCoroutines();
        smoothTime = playerSmoothTime;
        target = PlayerManager.instance.gameObject.transform;
        Camera.main.orthographicSize = initialZoom;
        GameController.instance.currentLevelController.SpawnFood();
        GameObject.Find("Foodpanda").transform.position = initialFoodPandaPos;

        _audioSourceFoodPanda.Stop();

        StartGame();
    }

    private void StartGame()
    {
        GameController.instance.isPanning = false;
        GameController.instance.currentUIManager.ShowAllUI();
        if (GameController.instance.isTutorialScene) GameController.instance.StartTutorial();
    }
}