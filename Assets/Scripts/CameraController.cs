using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Camera follow target
    private Transform target;

    Vector3 velocity = Vector3.zero;

    private float smoothTime;

    // Camera offset
    public Vector3 positionOffset;

    // Camera X and Y bounds
    public Vector2 xLimit;
    public Vector2 yLimit;

    private float initialZoom;
    private float playerSmoothTime = 0.3f;
    private float introSmoothTime = 1f;
    private Transform foodSpawnArea;

    private void Start()
    {
        initialZoom = Camera.main.orthographicSize;
        foodSpawnArea = GameObject.Find("Food Spawn Area").transform;

        if (GameController.instance.isPanning) StartCoroutine(PanMap());
    }

    private void Update()
    {
        if (GameController.instance.isPanning)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) SkipPanning();
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

        GameObject foodPanda = GameObject.Find("Foodpanda");
        Vector3 initialFoodPandaPos = foodPanda.transform.position;
        target = foodSpawnArea;
        while (Vector3.SqrMagnitude(foodPanda.transform.position - foodSpawnArea.position) >= 0.05f)
        {
            foodPanda.transform.position = Vector3.MoveTowards(foodPanda.transform.position, foodSpawnArea.position, 5 * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        GameController.instance.currentLevelController.SpawnFood();
        yield return new WaitForSeconds(0.5f);
        while (Vector3.SqrMagnitude(foodPanda.transform.position - initialFoodPandaPos) >= 0.05f)
        {
            foodPanda.transform.position = Vector3.MoveTowards(foodPanda.transform.position, initialFoodPandaPos, 5 * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        target = PlayerManager.instance.gameObject.transform;
        yield return new WaitForSeconds(2.5f);
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
        StartGame();
    }

    private void StartGame()
    {
        GameController.instance.isPanning = false;
        GameController.instance.currentUIManager.ShowAllUI();
    }
}