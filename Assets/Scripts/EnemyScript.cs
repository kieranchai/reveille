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
    private bool hasSetFinalRotation;
    private Quaternion finalRotation;
    private Vector3 playerLastSeenPosition;
    public List<Transform> patrolPoints = new List<Transform>();
    private bool hasSetNextPatrol;
    public int currentPatrolPoint;
    public float chaseTimer;
    public float sentryTimer;
    public LayerMask blockedLayers;
    [SerializeField] private Transform pfFieldOfView; // fov prefab
    private FieldOfView fov;
    public enum ENEMY_STATE
    {
        PATROL,
        TURNING,
        ALERTED,
        CHASE,
        SENTRY
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
            case ENEMY_STATE.TURNING:
                RotateToNextPoint();
                break;
            case ENEMY_STATE.ALERTED:
                AlertState();
                break;
            case ENEMY_STATE.CHASE:
                ChaseState();
                break;
            case ENEMY_STATE.SENTRY:
                SentryState();
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

        // If at current patrol point, set and turn towards next patrol point
        if (_agent.remainingDistance <= _agent.stoppingDistance && !hasSetNextPatrol)
        {
            hasSetNextPatrol = true;
            SetNextPatrolPoint();
            currentState = ENEMY_STATE.TURNING;
        }

        // If see player, go chase state
        if (PlayerInSight())
        {
            UpdatePlayerLastSeenPosition();
            currentState = ENEMY_STATE.CHASE;
        }
    }

    // Need to fix and change to coroutine so this doesnt take up a state
    void RotateToNextPoint()
    {
        if (!hasSetFinalRotation)
        {
            hasSetFinalRotation = true;
            float angle = Mathf.Atan2((patrolPoints[currentPatrolPoint].position - transform.position).y, (patrolPoints[currentPatrolPoint].position - transform.position).x) * Mathf.Rad2Deg;
            finalRotation = Quaternion.AngleAxis(angle - 90.0f, Vector3.forward);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation, 3.0f * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, finalRotation) <= 1.0f)
        {
            currentPathingTarget = patrolPoints[currentPatrolPoint].position;
            _agent.SetDestination(currentPathingTarget);
            currentState = ENEMY_STATE.PATROL;
            hasSetFinalRotation = false;
        }

        // If see player, go chase state
        if (PlayerInSight())
        {
            UpdatePlayerLastSeenPosition();
            currentState = ENEMY_STATE.CHASE;
            hasSetFinalRotation = false;
        }
    }

    private void ChaseState()
    {
        _agent.speed = runSpeed;

        // If see player, go to player
        if (PlayerInSight())
        {
            UpdatePlayerLastSeenPosition();

            chaseTimer = 0;

            currentPathingTarget = PlayerManager.instance.CurrentPosition();
            transform.up = currentPathingTarget - new Vector3(transform.position.x, transform.position.y);
            _agent.SetDestination(currentPathingTarget);
        }
        else
        {
            chaseTimer += Time.deltaTime;

            currentPathingTarget = playerLastSeenPosition;
            transform.up = currentPathingTarget - new Vector3(transform.position.x, transform.position.y);
            _agent.SetDestination(currentPathingTarget);
        }

        // If player not in sight for 5 seconds, go back to patrolling
        // Need to add: walk/look around before going back to patrolling
        if (chaseTimer >= 5.0f)
        {
            chaseTimer = 0;
            currentState = ENEMY_STATE.PATROL;
        }
    }

    private void AlertState()
    {
        _agent.speed = walkSpeed;

        // If see player, go to player
        if (PlayerInSight())
        {
            UpdatePlayerLastSeenPosition();
            currentState = ENEMY_STATE.CHASE;
        }

        transform.up = currentPathingTarget - new Vector3(transform.position.x, transform.position.y);
        _agent.SetDestination(currentPathingTarget);

        // If at current target, go back to patrolling
        // Need to add: walk/look around before going back to patrolling
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            currentState = ENEMY_STATE.PATROL;
        }
    }

    private void SentryState()
    {
        if (sentryTimer < 5)
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * 30f, 60f) - 45); //temp 30 is speed 60 is angle
            sentryTimer += Time.deltaTime;
            PlayerInSight();
        }
        else
        {
            currentState = ENEMY_STATE.PATROL;
            chaseTimer = 0;
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
        return inLOS && (sightAngle < fieldOfView);
    }

    private void UpdatePlayerLastSeenPosition()
    {
        playerLastSeenPosition = PlayerManager.instance.CurrentPosition();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Noise"))
        {
            // If currently chasing player and player is in line of sight, don't get distracted
            if (currentState == ENEMY_STATE.CHASE && PlayerInSight())
            {
                return;
            }

            currentPathingTarget = collision.gameObject.transform.position;
            currentState = ENEMY_STATE.ALERTED;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Game Over");
        }
    }
}
