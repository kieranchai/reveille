using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Variables
    // Player Components
    private Rigidbody2D _rigidBody;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private NoiseController _noiseController;
    public Player _data;

    // Default Data
    private float baseMovementSpeed;
    private int inventoryWeightLimit;
    private float movementSpeedMultiplier;
    private float noiseSizeMultiplier;

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
    #endregion

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _noiseController = transform.Find("Noise").GetComponent<NoiseController>();
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
                break;
            case PLAYER_STATE.HACKING:
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
                currentMovementSpeed = baseMovementSpeed * movementSpeedMultiplier; // * inventoryWeightPenalty
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
            // Toggle door
            // Toggle hack
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
    }
    #endregion

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
        inventory.Add(foodItem);
    }

    public void RemoveFoodFromInventory(Food foodItem)
    {
        currentInventoryWeight -= foodItem.weight;
        inventory.Remove(foodItem);
    }
}
