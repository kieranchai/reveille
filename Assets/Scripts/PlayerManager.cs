using System;
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
    public Animator _anim;
    private AudioSource _audio;
    private AudioSource _footstepsAudio;

    // Default Data
    private float baseMovementSpeed;
    private int inventoryWeightLimit;
    private float movementSpeedMultiplier;
    private float noiseSizeMultiplier;
    public int thrownFoodPointsDeduction;

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
    public int points;
    public int timePoints;
    public List<GameObject> nearbyFood = new List<GameObject>();
    private float throwTimer = 0.0f;
    private float throwInterval = 0.5f;
    private float initialChargeTime = 0.0f;
    private float totalChargeTime = 0.0f;
    private bool isCharging = false;
    public GameObject hackingTarget;
    public GameObject foodDropOffTarget;
    public bool inventoryOpen = false;
    #endregion

    #region Audio Clips
    [Header("Player Audio Clips")]
    public AudioClip playerWalk;
    public AudioClip pickUpFood;
    public AudioClip dropFood;
    public AudioClip throwFood;
    public AudioClip deliverFood;
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
        _anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        _footstepsAudio = transform.Find("Footsteps SFX").GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetPlayerData(_data);
    }

    private void Update()
    {
        if (GameController.instance.isPanning) return;

        switch (currentState)
        {
            case PLAYER_STATE.STILL:
            case PLAYER_STATE.WALKING:
            case PLAYER_STATE.SPRINTING:
            case PLAYER_STATE.SNEAKING:
                if (GameController.instance.isPaused) return;
                UpdateNoiseRadius();
                MovementInput();
                LookAtMouse();
                MouseSelectFood();
                InteractInput();
                MouseThrow();
                MouseDrop();
                ToggleInventory();
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
        GameController.instance.currentUIManager.UpdateWeightCount(currentInventoryWeight, inventoryWeightLimit);
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
            _anim.SetBool("isWalking", false);
            currentState = PLAYER_STATE.STILL;
            movementSpeedMultiplier = 1.0f;
            noiseSizeMultiplier = 0.0f;

            _footstepsAudio.Stop();
        }
        else
        {
            // Player press SHIFT to Sprint
            if (Input.GetKey(KeyCode.LeftShift))
            {
                GameController.instance.currentUIManager.SetSprintKeyPressed();
                GameController.instance.currentUIManager.SetSneakKeyDefault();
                _anim.speed = 1.5f;
                currentState = PLAYER_STATE.SPRINTING;
                movementSpeedMultiplier = 1.5f;
                noiseSizeMultiplier = 1.5f;

                _footstepsAudio.pitch = 1.5f;
                _footstepsAudio.volume = 1f;
            }
            // Player press CTRL to Sneak
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                GameController.instance.currentUIManager.SetSneakKeyPressed();
                GameController.instance.currentUIManager.SetSprintKeyDefault();
                _anim.speed = 0.5f;
                currentState = PLAYER_STATE.SNEAKING;
                movementSpeedMultiplier = 0.5f;
                noiseSizeMultiplier = 0.5f;

                _footstepsAudio.pitch = 0.5f;
                _footstepsAudio.volume = 0.2f;
            }
            // Player moving normally
            else
            {
                GameController.instance.currentUIManager.SetSprintKeyDefault();
                GameController.instance.currentUIManager.SetSneakKeyDefault();
                currentState = PLAYER_STATE.WALKING;
                movementSpeedMultiplier = 1.0f;
                noiseSizeMultiplier = 1.0f;

                _footstepsAudio.pitch = 1f;
                _footstepsAudio.volume = 0.5f;
            }
            _anim.SetBool("isWalking", true);

            if (!_audio.isPlaying)
            {
                _footstepsAudio.clip = playerWalk;
                _footstepsAudio.Play();
            }
        }

        moveDir = new Vector3(moveX, moveY).normalized;
    }

    public void InteractInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameController.instance.currentUIManager.SetInteractKeyPressed();

            // Pickup food higher priority
            if (nearbyFood.Count > 0)
            {
                // Pickup Food
                PickUpNearestFood();
                return;
            }

            // Toggle hack
            AttemptHack();

            // Deliver food
            DropOffFood();
        }
        else
        {
            GameController.instance.currentUIManager.SetInteractKeyDefault();
        }
    }

    public void ToggleInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryOpen = !inventoryOpen;

            AudioManager.instance.PlaySFX(AudioManager.instance.openInventory);

            if (inventoryOpen) GameController.instance.currentUIManager.DisplayInventory();
            else GameController.instance.currentUIManager.HideInventory();
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

            AudioManager.instance.PlaySFX(AudioManager.instance.scrollSfx);
            GameController.instance.currentUIManager.UpdateCurrentFood();
            GameController.instance.currentUIManager.UpdateInventorySelectedFood();
            CancelThrowFood();
        }

        // Scroll Down
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            --currentSelectedFood;
            if (currentSelectedFood < 0) currentSelectedFood = inventory.Count - 1;

            AudioManager.instance.PlaySFX(AudioManager.instance.scrollSfx);
            GameController.instance.currentUIManager.UpdateCurrentFood();
            GameController.instance.currentUIManager.UpdateInventorySelectedFood();
            CancelThrowFood();
        }
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
        }

        if (Input.GetMouseButton(0))
        {
            if (!isCharging)
            {
                initialChargeTime = Time.time;
                isCharging = true;
            }
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = 2;

            int mask1 = 1 << LayerMask.NameToLayer("Obstacles");
            /*            // For more layers
                        int mask2 = 1 << LayerMask.NameToLayer("Enemies");
                        int combinedMask = mask1 | mask2;*/
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, CalculateThrowRange(), mask1);
            if (hit.collider != null)
            {
                _lineRenderer.SetPosition(0, new Vector3(0f, 0f, -0.1f));
                _lineRenderer.SetPosition(1, transform.InverseTransformPoint(hit.point));
            }
            else
            {
                _lineRenderer.SetPosition(0, new Vector3(0f, 0f, -0.1f));
                _lineRenderer.SetPosition(1, transform.InverseTransformPoint(GetFinalThrowPosition()));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ThrowFood();
        }
    }

    public void MouseDrop()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DropFood();
            CancelThrowFood();
        }
    }

    public void ExitHack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            hackingTarget.GetComponent<Terminal>().StopHacking();
        }
    }
    #endregion

    public Vector3 CurrentPosition()
    {
        return transform.position;
    }

    public Vector3 CurrentVelocity()
    {
        return _rigidBody.velocity;
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

        GameController.instance.currentUIManager.UpdateWeightCount(currentInventoryWeight, inventoryWeightLimit);
        GameController.instance.currentUIManager.UpdateCurrentFood();
        GameController.instance.currentUIManager.UpdateInventorySelectedFood();
    }

    public void RemoveFoodFromInventory(Food foodItem)
    {
        currentInventoryWeight -= foodItem.weight;
        UpdateInventoryWeightPenalty();
        inventory.Remove(foodItem);
        --currentSelectedFood;
        if (currentSelectedFood < 0) currentSelectedFood = 0;
        GameController.instance.currentUIManager.UpdateCurrentFood();
        GameController.instance.currentUIManager.UpdateInventorySelectedFood();
        GameController.instance.currentUIManager.UpdateWeightCount(currentInventoryWeight, inventoryWeightLimit);
    }

    public void UpdateInventoryWeightPenalty()
    {
        if (currentInventoryWeight == 0)
        {
            inventoryWeightPenalty = 0;
            return;
        }

        inventoryWeightPenalty = (float)((1.0 * Mathf.Log(currentInventoryWeight)) / 5);
    }

    public void PickUpNearestFood()
    {
        _audio.clip = pickUpFood;
        _audio.Play();

        AddFoodToInventory(nearbyFood[0].GetComponent<FoodManager>()._data);
        Destroy(nearbyFood[0]);
        //Nearest Food is removed from nearbyFood in FoodManager OnTriggerExit
    }

    public void ThrowFood()
    {
        _audio.clip = throwFood;
        _audio.Play();

        inventory[currentSelectedFood].currentPoints -= thrownFoodPointsDeduction;
        GameObject thrownFood = Instantiate(Resources.Load<GameObject>("Prefabs/Food"), transform.position, Quaternion.identity);
        thrownFood.GetComponent<FoodManager>().SetFoodData(inventory[currentSelectedFood]);
        thrownFood.GetComponent<FoodManager>().Throw(CalculateThrowRange());
        RemoveFoodFromInventory(inventory[currentSelectedFood]);
        CancelThrowFood();
    }

    public float CalculateThrowRange()
    {
        totalChargeTime = Time.time - initialChargeTime;
        float minChargeTime = 0.5f; // Minimum charge time in seconds
        float maxChargeTime = 2f; // Maximum charge time in seconds
        float minRange = 1.0f; // Minimum food range
        float maxRange = 150 / Mathf.Sqrt(inventory[currentSelectedFood].weight); // Maximum food range

        // Clamp totalChargeTime to be within the specified range
        totalChargeTime = Mathf.Clamp(totalChargeTime, minChargeTime, maxChargeTime);

        // Calculate the speed using linear interpolation
        float finalRange = Mathf.Lerp(minRange, maxRange, (totalChargeTime - minChargeTime) / (maxChargeTime - minChargeTime));

        return finalRange;
    }

    public Vector3 GetFinalThrowPosition()
    {
        Vector3 finalPosition = transform.position + transform.up * (CalculateThrowRange() + 0.2f);
        return finalPosition;
    }

    public void DropFood()
    {
        if (inventory.Count == 0) return;

        //_audio.clip = dropFood;
        //_audio.Play();

        GameObject droppedFood = Instantiate(Resources.Load<GameObject>("Prefabs/Food"), transform.position, Quaternion.identity);
        droppedFood.GetComponent<FoodManager>().SetFoodData(inventory[currentSelectedFood]);
        RemoveFoodFromInventory(inventory[currentSelectedFood]);
    }

    public void CancelThrowFood()
    {
        throwTimer = 0.0f;
        _lineRenderer.positionCount = 0;
        _lineRenderer.enabled = false;
        isCharging = false;
    }

    public void AttemptHack()
    {
        if (!hackingTarget) return;
        if (!hackingTarget.GetComponent<Terminal>().playable) return;

        currentState = PLAYER_STATE.HACKING;
        _anim.SetBool("isWalking", false);
        noiseSizeMultiplier = 0.0f;
        hackingTarget.GetComponent<Terminal>().StartHacking();
    }

    public void DropOffFood()
    {
        if (!foodDropOffTarget) return;
        if (inventory.Count < 1) return;
        if (foodDropOffTarget.GetComponent<DropOff>().DepositFood(inventory[currentSelectedFood]))
        {
            //_audio.clip = deliverFood;
            //_audio.Play();

            UpdatePoints(inventory[currentSelectedFood].currentPoints);
            RemoveFoodFromInventory(inventory[currentSelectedFood]);
        }
    }

    public void UpdatePoints(int foodCurrentPoints)
    {
        points += foodCurrentPoints;
        GameController.instance.currentUIManager.UpdateTotalPoints(points);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Terminal"))
        {
            hackingTarget = collision.gameObject.transform.parent.gameObject;
            GameController.instance.currentUIManager.UpdateDisplayInteractables();
        }

        if (collision.gameObject.CompareTag("Dropoff"))
        {
            foodDropOffTarget = collision.gameObject.transform.parent.gameObject;
            GameController.instance.currentUIManager.UpdateDisplayInteractables();
            Animator animator = collision.gameObject.transform.parent.gameObject.GetComponent<Animator>();
            animator.SetTrigger("Open");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Terminal"))
        {
            hackingTarget = null;
            GameController.instance.currentUIManager.UpdateDisplayInteractables();
        }

        if (collision.gameObject.CompareTag("Dropoff"))
        {
            foodDropOffTarget = null;
            GameController.instance.currentUIManager.UpdateDisplayInteractables();
            Animator animator = collision.gameObject.transform.parent.gameObject.GetComponent<Animator>();
            animator.SetTrigger("Close");
        }
    }
}
