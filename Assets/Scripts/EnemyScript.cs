using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    #region Variables

    // Enemy Components
    private Rigidbody2D _rigidBody;
    private Collider2D _collider;
    public Enemy _data;
    private NavMeshAgent _agent;

    // Default Data
    private int id;
    private string enemyName;
    private float walkSpeed;
    private float runSpeed;
    private float lineOfSight;
    private float fieldOfView;

    // Dynamic Data
    public Vector3 currentPathingTarget;
    private Quaternion finalRotation;
    private Vector3 playerLastSeenPosition;
    public List<Transform> patrolPoints = new List<Transform>();
    public int currentPatrolPoint;
    public float playerSeenTimer;
    public float confusedTimer;
    public float chaseTimer;
    public float predictTimer;
    public float alertTimer;
    public LayerMask blockedLayers;
    [SerializeField] private Transform pfFieldOfView; // fov prefab
    private FieldOfView fov;
    public enum ENEMY_STATE
    {
        PATROL,
        ALERTED,
        CONFUSED,
        CHASE
    };
    public ENEMY_STATE currentState = ENEMY_STATE.PATROL;
    public bool isTurning = false;
    public bool isDoneLooking = false;
    private bool isLookingLeft = true;
    private bool isLookingRight = false;
    private bool isLookingRightR = false;
    private bool hasSetLookDirections = false;
    private Vector3 initialUp;
    private Vector3 initialLeft;
    private Vector3 initialRight;
    public float rotationTime;
    #endregion

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    private void Start()
    {
        SetEnemyData(_data);
        currentPathingTarget = patrolPoints[currentPatrolPoint].position;
        _agent.SetDestination(currentPathingTarget);
    }

    private void Update()
    {
        switch (currentState)
        {
            case ENEMY_STATE.PATROL:
                PatrolState();
                break;
            case ENEMY_STATE.CONFUSED:
                ConfusedState();
                break;
            case ENEMY_STATE.ALERTED:
                AlertState();
                break;
            case ENEMY_STATE.CHASE:
                ChaseState();
                break;
            default:
                break;
        }
        FovPos();
    }

    public void SetEnemyData(Enemy data)
    {
        this.lineOfSight = data.lineOfSight;
        this.fieldOfView = data.fieldOfView;
        this.id = data.id;
        this.enemyName = data.enemyName;
        this.walkSpeed = data.walkSpeed;
        this.runSpeed = data.runSpeed;

        _agent.speed = this.walkSpeed;

        fov = Instantiate(pfFieldOfView, null).GetComponent<FieldOfView>();
        fov.SetFOV(fieldOfView);
        fov.SetLOS(lineOfSight);
    }

    #region Enemy State Methods
    private void PatrolState()
    {
        _agent.speed = walkSpeed;

        // Face target only when not turning
        if (!isTurning) transform.up = currentPathingTarget - new Vector3(transform.position.x, transform.position.y);

        // If at current patrol point, set and go towards next patrol point
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            // Immediately set next patrol point then set turning true
            if (!isTurning) SetNextPatrolPoint();
            isTurning = true;

            if (isTurning)
            {
                // Stop the enemy movement when turning
                _agent.ResetPath();

                if (RotateToNextPoint(patrolPoints[currentPatrolPoint].position))
                {
                    ResetRotationVariables();

                    // Go to next pathing target
                    currentPathingTarget = patrolPoints[currentPatrolPoint].position;
                    _agent.SetDestination(currentPathingTarget);
                }
            }
        }

        // If see player, become confused "Huh?!"
        if (PlayerInSight())
        {
            ResetRotationVariables();
            UpdatePlayerLastSeenPosition();
            currentPathingTarget = playerLastSeenPosition;
            _agent.ResetPath();
            currentState = ENEMY_STATE.CONFUSED;
        }
    }

    private void ConfusedState()
    {
        _agent.speed = walkSpeed;
        confusedTimer += Time.deltaTime;

        isTurning = true;
        if (isTurning)
        {
            if (RotateToNextPoint(currentPathingTarget))
            {
                // After confused for 1 second, go to alerted
                if (confusedTimer >= 1.0f)
                {
                    ResetRotationVariables();
                    confusedTimer = 0.0f;
                    currentState = ENEMY_STATE.ALERTED;
                }

                // If confused but still see player after 0.5 second, start chasing "HEY!"
                if (PlayerInSight() && confusedTimer >= 0.5f)
                {
                    ResetRotationVariables();
                    confusedTimer = 0.0f;
                    currentState = ENEMY_STATE.CHASE;
                }
            }
        }

        if (!isTurning) transform.up = currentPathingTarget - new Vector3(transform.position.x, transform.position.y);
    }

    private void AlertState()
    {
        _agent.speed = walkSpeed;

        if (!isTurning) transform.up = new Vector3(_agent.steeringTarget.x, _agent.steeringTarget.y) - new Vector3(transform.position.x, transform.position.y);
        if (!isTurning) _agent.SetDestination(currentPathingTarget);

        if (isTurning && !isDoneLooking)
        {
            // Stop the enemy movement when turning
            _agent.ResetPath();

            if (RotateToNextPoint(currentPathingTarget))
            {
                ResetRotationVariables();
            }
        }

        // At current target
        if (_agent.remainingDistance != 0 && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (!isDoneLooking)
            {
                if (LookAround())
                {
                    isDoneLooking = true;
                    rotationTime = 0.0f;
                }
            }

            if (isDoneLooking)
            {
                // Go back to patrolling
                isTurning = true;
                currentPathingTarget = patrolPoints[currentPatrolPoint].position;
                if (isTurning)
                {
                    if (RotateToNextPoint(currentPathingTarget))
                    {
                        _agent.SetDestination(currentPathingTarget);
                        currentState = ENEMY_STATE.PATROL;
                        ResetRotationVariables();
                    }
                }
            }
        }

        // If see player, start chasing player
        if (PlayerInSight())
        {
            ResetRotationVariables();
            alertTimer = 0.0f;
            UpdatePlayerLastSeenPosition();
            currentState = ENEMY_STATE.CHASE;
        }
    }

    private void ChaseState()
    {
        _agent.speed = runSpeed;

        if (!isTurning) transform.up = new Vector3(_agent.steeringTarget.x, _agent.steeringTarget.y) - new Vector3(transform.position.x, transform.position.y);
        if (!isTurning) _agent.SetDestination(currentPathingTarget);

        // If see player, start chasing player
        if (PlayerInSight())
        {
            UpdatePlayerLastSeenPosition();

            chaseTimer = 0.0f;
            predictTimer = 0.0f;

            currentPathingTarget = PlayerManager.instance.CurrentPosition();
        }
        // If not, start going to player's predicted path
        else
        {
            chaseTimer += Time.deltaTime;
            predictTimer += Time.deltaTime;

            // Method 1: Go to player's position every 2 second?

            // Method 2: Predict player's path from last seen position every 1 second
            if (predictTimer >= 1.0f)
            {
                float timeToPlayer = Vector3.Distance(PlayerManager.instance.CurrentPosition(), transform.position) / _agent.speed;
                if (timeToPlayer > 0.5f)
                {
                    timeToPlayer = 1.0f;
                }
                Vector3 predictedPosition = playerLastSeenPosition + PlayerManager.instance.CurrentVelocity() * timeToPlayer;
                Vector3 directionToTarget = (predictedPosition - transform.position).normalized;
                Vector3 directionToPlayer = (PlayerManager.instance.CurrentPosition() - transform.position).normalized;
                float dot = Vector3.Dot(directionToPlayer, directionToTarget);
                if (dot < 0.4f)
                {
                    predictedPosition = PlayerManager.instance.CurrentPosition();
                }
                currentPathingTarget = predictedPosition;
                predictTimer = 0.0f;
            }
        }

        // If player not in sight for 5 seconds, look around
        if (chaseTimer >= 5.0f)
        {
            _agent.ResetPath();

            // Look around

            // Go back to patrolling
            alertTimer += Time.deltaTime;
            if (alertTimer >= 1.0f)
            {
                isTurning = true;
                currentPathingTarget = patrolPoints[currentPatrolPoint].position;
                if (isTurning)
                {
                    // Stop the enemy movement when turning
                    _agent.ResetPath();

                    if (RotateToNextPoint(currentPathingTarget))
                    {
                        alertTimer = 0.0f;
                        chaseTimer = 0.0f;
                        predictTimer = 0.0f;
                        ResetRotationVariables();
                        _agent.SetDestination(currentPathingTarget);
                        currentState = ENEMY_STATE.PATROL;
                    }
                }
            }
        }
    }
    #endregion

    private void SetNextPatrolPoint()
    {
        ++currentPatrolPoint;
        if (currentPatrolPoint > patrolPoints.Count - 1) currentPatrolPoint = 0;
    }

    public void FovPos()
    {
        fov.SetAimDirection(transform.up);
        fov.SetOrigin(transform.position);
    }

    private bool PlayerInSight()
    {
        // Check if player is within Line of Sight
        bool inLOS = Vector3.Distance(PlayerManager.instance.CurrentPosition(), transform.position) <= lineOfSight;

        // Check if player is within Field of View
        float sightAngle = Vector2.Angle(PlayerManager.instance.CurrentPosition() - transform.position, transform.up);

        // Check if there is a wall between enemy and player
        RaycastHit2D hit = Physics2D.Linecast(transform.position, PlayerManager.instance.CurrentPosition(), blockedLayers);
        if (hit.collider != null)
        {
            // Returns false if there is a wall
            return false;
        }

        // Returns true if both conditions are met
        return inLOS && (sightAngle < fieldOfView / 2);
    }

    private void UpdatePlayerLastSeenPosition()
    {
        playerLastSeenPosition = PlayerManager.instance.CurrentPosition();
    }

    public bool RotateToNextPoint(Vector3 nextPoint)
    {
        // Calculate angle between next pathing target and current position
        float angle = Mathf.Atan2((nextPoint - transform.position).y, (nextPoint - transform.position).x) * Mathf.Rad2Deg;
        finalRotation = Quaternion.AngleAxis(angle - 90.0f, Vector3.forward);
        rotationTime += Time.deltaTime * 0.1f;

        // Method 1: Lerp enemy's rotation to to next pathing target
        /*transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation, rotationTime);*/

        // Method 2: Lerp enemy's transform.up to vector towards next pathing target
        transform.up = Vector3.Lerp(transform.up, new Vector3((nextPoint - transform.position).normalized.x, (nextPoint - transform.position).normalized.y, 0), rotationTime);

        // Once rotation complete return true
        if (Quaternion.Angle(transform.rotation, finalRotation) <= 0.0f)
        {
            return true;
        }

        return false;
    }

    public bool LookAround()
    {
        if (!hasSetLookDirections)
        {
            hasSetLookDirections = true;
            initialUp = new Vector3(transform.up.x, transform.up.y, 0);
            initialLeft = new Vector3(-transform.right.x, -transform.right.y, 0);
            initialRight = new Vector3(transform.right.x, transform.right.y, 0);
        }

        rotationTime += Time.deltaTime * 0.8f;

        if (isLookingLeft) transform.up = Vector3.Lerp(transform.up, initialLeft, rotationTime);

        if (isLookingLeft && Vector3.SqrMagnitude(transform.up - initialLeft) <= 0.001f)
        {
            transform.up = initialLeft;
            rotationTime = 0.0f;
            isLookingLeft = false;
            isLookingRight = true;
        }

        if (isLookingRight)
        {
            transform.up = Vector3.Lerp(initialLeft, initialUp, rotationTime);
        }

        if (isLookingRight && Vector3.SqrMagnitude(transform.up - initialUp) <= 0.001f)
        {
            transform.up = initialUp;
            rotationTime = 0.0f;
            isLookingLeft = false;
            isLookingRight = false;
            isLookingRightR = true;
        }

        if (isLookingRightR)
        {
            transform.up = Vector3.Lerp(initialUp, initialRight, rotationTime);
        }

        if (isLookingRightR && Vector3.SqrMagnitude(transform.up - initialRight) <= 0.001f)
        {
            transform.up = initialRight;
            return true;
        }

        return false;
    }

    public void ResetRotationVariables()
    {
        // Reset rotation time and set turning false
        rotationTime = 0.0f;
        isTurning = false;
        hasSetLookDirections = false;
        isLookingLeft = true;
        isLookingRight = false;
        isLookingRightR = false;
        initialLeft = Vector3.zero;
        initialRight = Vector3.zero;
        initialUp = Vector3.zero;
        isDoneLooking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Noise"))
        {
            // If currently chasing player, don't get confused
            if (currentState == ENEMY_STATE.CHASE) return;

            // If already confused, don't get confused again
            if (currentState == ENEMY_STATE.CONFUSED) return;

            // If hear noise when alerted, stay alerted but go to new noise position 
            if (currentState == ENEMY_STATE.ALERTED)
            {
                ResetRotationVariables();
                isTurning = true;

                alertTimer = 0.0f;
                currentPathingTarget = collision.gameObject.transform.position;
                currentState = ENEMY_STATE.ALERTED;
            }

            // If hear noise when patrolling, get confused for 1 second
            if (currentState == ENEMY_STATE.PATROL)
            {
                ResetRotationVariables();

                alertTimer = 0.0f;
                currentPathingTarget = collision.gameObject.transform.position;
                _agent.ResetPath();
                currentState = ENEMY_STATE.CONFUSED;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Game Over!");
        }
    }
}
