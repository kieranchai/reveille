using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Default Data
    public float baseMovementSpeed;
    public int inventoryWeightLimit;

    // Player Components
    public Rigidbody2D _rigidBody;
    public Collider2D _collider;
    public SpriteRenderer _spriteRenderer;
    public Player _data;

    // Dynamic Data
    private Vector3 moveDir;

    // Player State
    public enum PLAYER_STATE
    {
        WALKING,
        SPRINTING,
        SNEAKING
    }

    public PLAYER_STATE currentState;

    private void Start()
    {
        SetPlayerData(_data);
    }

    private void Update()
    {
        switch (currentState)
        {
            case PLAYER_STATE.WALKING:
                HandleInput();
                LookAtMouse();
                break;
            case PLAYER_STATE.SPRINTING:
                break;
            case PLAYER_STATE.SNEAKING:
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
                _rigidBody.velocity = moveDir * (baseMovementSpeed); // * movementSpeedMultiplier
                break;
        }
    }

    // Methods
    private void SetPlayerData(Player data)
    {
        this.baseMovementSpeed = data.baseMovementSpeed;
        this.inventoryWeightLimit = data.inventoryWeightLimit;
    }

    public void HandleInput()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = +1f;
        }

        moveDir = new Vector3(moveX, moveY).normalized;
    }

    public void LookAtMouse()
    {
        Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.up = (Vector3)(mousePos - new Vector2(transform.position.x, transform.position.y));
    }
}
