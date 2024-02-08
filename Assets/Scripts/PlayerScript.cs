using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Player Components
    private Rigidbody2D _rigidBody;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private NoiseController _noiseController;
    public Player _data;

    // Default Data
    private float baseMovementSpeed;
    private int inventoryWeightLimit;
    private float movementStateMultiplier;

    // Dynamic Data
    private Vector3 moveDir;
    public enum PLAYER_STATE
    {
        WALKING,
        SPRINTING,
        SNEAKING,
        HACKING
    }
    public PLAYER_STATE currentState;
    public float currentMovementSpeed;
    public int currentInventoryWeight;
    public float inventoryWeightPenalty;

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
            case PLAYER_STATE.WALKING:
            case PLAYER_STATE.SPRINTING:
            case PLAYER_STATE.SNEAKING:
                UpdateNoiseRadius();
                MovementInput();
                LookAtMouse();
                break;
            case PLAYER_STATE.HACKING:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case PLAYER_STATE.WALKING:
            case PLAYER_STATE.SPRINTING:
            case PLAYER_STATE.SNEAKING:
                currentMovementSpeed = baseMovementSpeed * movementStateMultiplier; // * inventoryWeightPenalty
                _rigidBody.velocity = moveDir * currentMovementSpeed;
                break;
        }
    }

    // Methods
    private void SetPlayerData(Player data)
    {
        this.baseMovementSpeed = data.baseMovementSpeed;
        this.inventoryWeightLimit = data.inventoryWeightLimit;
    }

    public void MovementInput()
    {
        float moveX = 0f;
        float moveY = 0f;
        if (Input.GetKey(KeyCode.W)) moveY = +1f;
        if (Input.GetKey(KeyCode.S)) moveY = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = +1f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentState = PLAYER_STATE.SPRINTING;
            movementStateMultiplier = 1.5f;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentState = PLAYER_STATE.SNEAKING;
            movementStateMultiplier = 0.8f;
        }
        else
        {
            currentState = PLAYER_STATE.WALKING;
            movementStateMultiplier = 1.0f;
        }
        moveDir = new Vector3(moveX, moveY).normalized;
    }

    public void LookAtMouse()
    {
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.up = (Vector3)(mousePos - new Vector2(transform.position.x, transform.position.y));
    }

    public void UpdateNoiseRadius()
    {
        _noiseController.UpdateNoiseRadius(movementStateMultiplier);
    }
}
