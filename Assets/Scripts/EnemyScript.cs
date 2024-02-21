using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
/*using static EnemyScript;*/

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
    private bool hasSetNextPatrol;
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

        transform.up = currentPathingTarget - new Vector3(transform.position.x, transform.position.y);

        // If at current patrol point, set and go towards next patrol point
        if (_agent.remainingDistance <= _agent.stoppingDistance && !hasSetNextPatrol)
        {
            hasSetNextPatrol = true;
            SetNextPatrolPoint();
            currentPathingTarget = patrolPoints[currentPatrolPoint].position;
            _agent.SetDestination(currentPathingTarget);
        }

        // If see player, become confused "Huh?!"
        if (PlayerInSight())
        {
            UpdatePlayerLastSeenPosition();
            currentPathingTarget = playerLastSeenPosition;
            _agent.ResetPath();
            currentState = ENEMY_STATE.CONFUSED;

        }
    }

    private void ConfusedState()
    {
        _agent.speed = walkSpeed;

        transform.up = currentPathingTarget - new Vector3(transform.position.x, transform.position.y);

        confusedTimer += Time.deltaTime;

        // After confused for 1 second, go to alerted
        if (confusedTimer >= 1.0f)
        {
            confusedTimer = 0.0f;
            currentState = ENEMY_STATE.ALERTED;
        }

        // If confused but still see player after 0.5 second, start chasing "HEY!"
        if (PlayerInSight() && confusedTimer >= 0.5f)
        {
            confusedTimer = 0.0f;
            currentState = ENEMY_STATE.CHASE;
        }

    }

    private void AlertState()
    {
        _agent.speed = walkSpeed;

        transform.up = currentPathingTarget - new Vector3(transform.position.x, transform.position.y);
        _agent.SetDestination(currentPathingTarget);

        // If at current target, look around
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            alertTimer += Time.deltaTime;

            // After looking around for 1 second, go back to patrolling
            if (alertTimer >= 1.0f)
            {
                alertTimer = 0.0f;
                currentPathingTarget = patrolPoints[currentPatrolPoint].position;
                _agent.SetDestination(currentPathingTarget);
                currentState = ENEMY_STATE.PATROL;
            }
        }

        // If see player, start chasing player
        if (PlayerInSight())
        {
            alertTimer = 0.0f;
            UpdatePlayerLastSeenPosition();
            currentState = ENEMY_STATE.CHASE;
        }
    }

    private void ChaseState()
    {
        _agent.speed = runSpeed;

        // If see player, start chasing player
        if (PlayerInSight())
        {
            UpdatePlayerLastSeenPosition();

            chaseTimer = 0.0f;
            predictTimer = 0.0f;

            currentPathingTarget = PlayerManager.instance.CurrentPosition();
            transform.up = currentPathingTarget - new Vector3(transform.position.x, transform.position.y);
            _agent.SetDestination(currentPathingTarget);
        }
        // If not, start going to player's predicted path
        else
        {
            chaseTimer += Time.deltaTime;
            predictTimer += Time.deltaTime;

            // Predict player's path from last seen position every 1 second
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

            transform.up = currentPathingTarget - new Vector3(transform.position.x, transform.position.y);
            _agent.SetDestination(currentPathingTarget);
        }

        // If player not in sight for 5 seconds, look around
        if (chaseTimer >= 5.0f)
        {
            _agent.ResetPath();

            // After looking around for 1 second, go back to patrolling
            alertTimer += Time.deltaTime;
            if (alertTimer >= 1.0f)
            {
                alertTimer = 0.0f;
                chaseTimer = 0.0f;
                predictTimer = 0.0f;

                currentPathingTarget = patrolPoints[currentPatrolPoint].position;
                _agent.SetDestination(currentPathingTarget);
                currentState = ENEMY_STATE.PATROL;
            }
        }
    }
    #endregion

    private void SetNextPatrolPoint()
    {
        ++currentPatrolPoint;
        if (currentPatrolPoint > patrolPoints.Count - 1) currentPatrolPoint = 0;
        hasSetNextPatrol = false;
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

    /*IEnumerator RotateToNextPathingTarget(Vector3 nextPathingTarget, string returnState)
    {
        // Stop enemy from moving
        _agent.ResetPath();

        // Calculate angle between next pathing target and current position
        float angle = Mathf.Atan2((nextPathingTarget - transform.position).y, (nextPathingTarget - transform.position).x) * Mathf.Rad2Deg;
        finalRotation = Quaternion.AngleAxis(angle - 90.0f, Vector3.forward);

        // Lerp enemy's rotation to to next pathing target
        while (Quaternion.Angle(transform.rotation, finalRotation) > 1.0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation, 3.0f * Time.deltaTime);
            yield return null;
        }

        // Once lerp completed, set currentPathingTarget to next pathing target and move enemy there
        currentPathingTarget = nextPathingTarget;
        _agent.SetDestination(currentPathingTarget);

        // Return enemy back to the state it was in before turning
        switch (returnState)
        {
            case "PATROL":
                currentState = ENEMY_STATE.PATROL;
                break;
            case "ALERTED":
                currentState = ENEMY_STATE.ALERTED;
                break;
            case "CHASE":
                currentState = ENEMY_STATE.CHASE;
                break;
            case "SENTRY":
                currentState = ENEMY_STATE.SENTRY;
                break;
            default:
                break;
        }
    }*/

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
                alertTimer = 0.0f;
                currentPathingTarget = collision.gameObject.transform.position;
                currentState = ENEMY_STATE.ALERTED;
            }

            // If hear noise when patrolling, get confused for 1 second
            if (currentState == ENEMY_STATE.PATROL)
            {
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
