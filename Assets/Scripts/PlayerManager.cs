using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Variables
    public static PlayerManager instance { get; private set; }

    // Player Components
    private Rigidbody2D _rigidBody;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private NoiseController _noiseController;
    private LineRenderer _lineRenderer;
    public Player _data;

    // Default Data
    private float baseMovementSpeed;
    private int inventoryWeightLimit;
    private float movementSpeedMultiplier;
    private float noiseSizeMultiplier;
    [SerializeField] private int thrownFoodPointsDeduction;

    // Dynamic Data
    private Vector3 moveDir;
    public enum PLAYER_STATE
    {
        STILL,
        WALKING,
        SPRINTING,
        SNEAKING,
        HACKING
    }
    public PLAYER_STATE currentState = PLAYER_STATE.STILL;
    public float currentMovementSpeed;
    public int currentInventoryWeight;
    public float inventoryWeightPenalty;
    public List<Food> inventory = new List<Food>();
    public int currentSelectedFood;
    public List<GameObject> nearbyFood = new List<GameObject>();
    private float throwTimer = 0.0f;
    private float throwInterval = 0.5f;
    private float initialChargeTime = 0.0f;
    private float totalChargeTime = 0.0f;
    private bool isCharging = false;
    public GameObject hackingTarget;
    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _noiseController = transform.Find("Noise").GetComponent<NoiseController>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        SetPlayerData(_data);
    }

    private void Update()
    {
        switch (currentState)
        {
            case PLAYER_STATE.STILL:
            case PLAYER_STATE.WALKING:
            case PLAYER_STATE.SPRINTING:
            case PLAYER_STATE.SNEAKING:
                UpdateNoiseRadius();
                MovementInput();
                LookAtMouse();
                MouseSelectFood();
                InteractInput();
                MouseThrow();
                MouseDrop();
                break;
            case PLAYER_STATE.HACKING:
                UpdateNoiseRadius();
                ExitHack();
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case PLAYER_STATE.STILL:
            case PLAYER_STATE.WALKING:
            case PLAYER_STATE.SPRINTING:
            case PLAYER_STATE.SNEAKING:
                currentMovementSpeed = (baseMovementSpeed * movementSpeedMultiplier) - inventoryWeightPenalty;
                _rigidBody.velocity = moveDir * currentMovementSpeed;
                break;
        }
    }

    private void SetPlayerData(Player data)
    {
        this.baseMovementSpeed = data.baseMovementSpeed;
        this.inventoryWeightLimit = data.inventoryWeightLimit;
    }

    #region Controls
    public void MovementInput()
    {
        float moveX = 0f;
        float moveY = 0f;

        // WASD
        if (Input.GetKey(KeyCode.W)) moveY = +1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = +1f;

        // Player not moving
        if (moveX == 0f && moveY == 0f)
        {
            currentState = PLAYER_STATE.STILL;
            movementSpeedMultiplier = 1.0f;
            noiseSizeMultiplier = 0.0f;
        }
        else
        {
            // Player press SHIFT to Sprint
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentState = PLAYER_STATE.SPRINTING;
                movementSpeedMultiplier = 1.5f;
                noiseSizeMultiplier = 1.5f;
            }
            // Player press CTRL to Sneak
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                currentState = PLAYER_STATE.SNEAKING;
                movementSpeedMultiplier = 0.5f;
                noiseSizeMultiplier = 0.5f;
            }
            // Player moving normally
            else
            {
                currentState = PLAYER_STATE.WALKING;
                movementSpeedMultiplier = 1.0f;
                noiseSizeMultiplier = 1.0f;
            }
        }

        moveDir = new Vector3(moveX, moveY).normalized;
    }

    public void InteractInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Pickup food higher priority
            if (nearbyFood.Count > 0)
            {
                // Pickup Food
                PickUpNearestFood();
                return;
            }

            // Toggle hack
            AttemptHack();
        }
    }

    public void LookAtMouse()
    {
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.up = (Vector3)(mousePos - new Vector2(transform.position.x, transform.position.y));
    }

    public void MouseSelectFood()
    {
        if (inventory.Count <= 1) return;

        // Scroll Up
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            ++currentSelectedFood;
            if (currentSelectedFood >= inventory.Count) currentSelectedFood = 0;
        }

        // Scroll Down
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            --currentSelectedFood;
            if (currentSelectedFood < 0) currentSelectedFood = inventory.Count - 1;
        }

        // Cancel Aiming Food
    }

    public void MouseThrow()
    {
        throwTimer += Time.deltaTime;
        if (inventory.Count < 1) return;
        if (throwTimer < throwInterval) return;

        if (Input.GetMouseButtonDown(0))
        {
            initialChargeTime = Time.time;
            isCharging = true;
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = 1;
            _lineRenderer.SetPosition(0, Vector3.zero);
        }

        if (Input.GetMouseButton(0))
        {
            if (!isCharging)
            {
                initialChargeTime = Time.time;
                isCharging = true;
            }
            _lineRenderer.positionCount = 2;

            int mask1 = 1 << LayerMask.NameToLayer("Obstacles");
            /*            // For more layers
                        int mask2 = 1 << LayerMask.NameToLayer("Enemies");
                        int combinedMask = mask1 | mask2;*/
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, CalculateThrowRange(), mask1);
            if (hit.collider != null)
            {
                _lineRenderer.SetPosition(1, transform.InverseTransformPoint(hit.point));
            }
            else
            {
                _lineRenderer.SetPosition(1, new Vector3(0, (0 + CalculateThrowRange()) / 2));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ThrowFood();
            _lineRenderer.positionCount = 0;
            _lineRenderer.enabled = false;
        }
    }

    public void MouseDrop()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DropFood();
        }
    }

    public void ExitHack()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
        {
            GameObject minigameClone = hackingTarget.transform.Find("Minigame(Clone)").gameObject;
            if (minigameClone != null)
            {
                Destroy(minigameClone);
            }
            currentState = PLAYER_STATE.STILL;
        }
    }
    #endregion

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void UpdateNoiseRadius()
    {
        _noiseController.UpdateNoiseRadius(noiseSizeMultiplier);
    }

    public void AddFoodToInventory(Food foodItem)
    {
        // Check if current inventory weight have enough space for food to be added
        if (currentInventoryWeight >= inventoryWeightLimit) return;
        if (currentInventoryWeight + foodItem.weight > inventoryWeightLimit) return;

        currentInventoryWeight += foodItem.weight;
        UpdateInventoryWeightPenalty();
        inventory.Add(foodItem);
    }

    public void RemoveFoodFromInventory(Food foodItem)
    {
        currentInventoryWeight -= foodItem.weight;
        UpdateInventoryWeightPenalty();
        inventory.Remove(foodItem);
    }

    public void UpdateInventoryWeightPenalty()
    {
        if (currentInventoryWeight == 0)
        {
            inventoryWeightPenalty = 0;
            return;
        }

        inventoryWeightPenalty = (float)((0.8 * Mathf.Log(currentInventoryWeight)) / 5);
    }

    public void PickUpNearestFood()
    {
        AddFoodToInventory(nearbyFood[0].GetComponent<FoodManager>()._data);
        Destroy(nearbyFood[0]);
        //Nearest Food is removed from nearbyFood in FoodManager OnTriggerExit
    }

    public void ThrowFood()
    {
        inventory[currentSelectedFood].currentPoints -= thrownFoodPointsDeduction;
        GameObject thrownFood = Instantiate(Resources.Load<GameObject>("Prefabs/Food"), transform.position, Quaternion.identity);
        thrownFood.GetComponent<FoodManager>().SetFoodData(inventory[currentSelectedFood]);
        thrownFood.GetComponent<FoodManager>().Throw(CalculateThrowRange());
        RemoveFoodFromInventory(inventory[currentSelectedFood]);
        throwTimer = 0.0f;
    }

    public float CalculateThrowRange()
    {
        totalChargeTime = Time.time - initialChargeTime;
        float minChargeTime = 0.5f; // Minimum charge time in seconds
        float maxChargeTime = 2f; // Maximum charge time in seconds
        float minRange = 1.0f; // Minimum food range
        float maxRange = 200 / Mathf.Sqrt(inventory[currentSelectedFood].weight); // Maximum food range

        // Clamp totalChargeTime to be within the specified range
        totalChargeTime = Mathf.Clamp(totalChargeTime, minChargeTime, maxChargeTime);

        // Calculate the speed using linear interpolation
        float finalRange = Mathf.Lerp(minRange, maxRange, (totalChargeTime - minChargeTime) / (maxChargeTime - minChargeTime));

        return finalRange;
    }

    public void DropFood()
    {
        if (inventory.Count < 1) return;

        GameObject droppedFood = Instantiate(Resources.Load<GameObject>("Prefabs/Food"), transform.position, Quaternion.identity);
        droppedFood.GetComponent<FoodManager>().SetFoodData(inventory[currentSelectedFood]);
        RemoveFoodFromInventory(inventory[currentSelectedFood]);
    }

    public void AttemptHack()
    {
        if (!hackingTarget) return;
        if (!hackingTarget.GetComponent<Terminal>().playable) return;

        currentState = PLAYER_STATE.HACKING;
        noiseSizeMultiplier = 0.0f;
        Instantiate(hackingTarget.GetComponent<Terminal>().minigame, hackingTarget.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Terminal"))
        {
            hackingTarget = collision.gameObject.transform.parent.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Terminal"))
        {
            hackingTarget = null;
        }
    }
}
