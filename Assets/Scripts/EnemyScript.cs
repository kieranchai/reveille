using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*using static EnemyScript;*/

public class EnemyScript : MonoBehaviour
{
    #region Variables

    // Enemy Components
    private Rigidbody2D _rigidBody;
    private Collider2D _collider;
    public Enemy _data;

    private Vector3 noiseSource;

    // Default Data
    private int id;
    private string enemyName;
    private float walkSpeed;
    private float runSpeed;
    private float lineOfSight;
    private float fieldOfView;

    // Dynamic Data
    public Vector3[] patrolPoints;
    public int targetPoint;
    public float chaseTimer;
    public float sentryTimer;
    public LayerMask blockedLayers;
    [SerializeField] private Transform pfFieldOfView; // fov prefab
    private FieldOfView fov;
    public enum ENEMY_STATE
    {
        PATROL,
        SENTRY,
        ALERTED,
        CHASE
    };
    public ENEMY_STATE currentState = ENEMY_STATE.PATROL;
    #endregion

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        patrolPoints = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            patrolPoints[i] = transform.GetChild(i).GetComponent<Transform>().position;
        }
    }

    private void Start()
    {
        SetEnemyData(_data);
    }

    private void Update()
    {
        switch (currentState)
        {
            case ENEMY_STATE.PATROL:
                PatrolState();
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

        fov = Instantiate(pfFieldOfView, null).GetComponent<FieldOfView>();
        fov.SetFOV(fieldOfView);
        fov.SetLOS(lineOfSight);
    }

    #region Enemy State Methods
    private void PatrolState()
    {
        // If at current patrol point, set next patrol point
        if (transform.position == patrolPoints[targetPoint])
        {
            SetNextPatrolPoint();
        }

        // Go to next patrol point
        transform.up = patrolPoints[targetPoint] - new Vector3(transform.position.x, transform.position.y);
        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[targetPoint], walkSpeed * Time.deltaTime);

        // If see player, go chase state
        if (PlayerInSight())
        {
            currentState = ENEMY_STATE.CHASE;
        }
    }

    public void ChaseState()
    {
        // If see player, go to player
        if (PlayerInSight())
        {
            chaseTimer = 0;
            transform.up = PlayerManager.instance.CurrentPosition() - new Vector3(transform.position.x, transform.position.y);
            transform.position = Vector3.MoveTowards(transform.position, PlayerManager.instance.CurrentPosition(), runSpeed * Time.deltaTime);
        }
        else
        {
            chaseTimer += Time.deltaTime;
        }

        // If player not in sight for 5 seconds, go back to patrolling
        if (chaseTimer >= 5.0f)
        {
            chaseTimer = 0;
            currentState = ENEMY_STATE.PATROL;
        }
    }

    public void AlertState()
    {
        // If see player, go to player
        if (PlayerInSight())
        {
            currentState = ENEMY_STATE.CHASE;
        }

        // Go towards noise source
        transform.up = noiseSource - new Vector3(transform.position.x, transform.position.y);
        transform.position = Vector3.MoveTowards(transform.position, noiseSource, walkSpeed * Time.deltaTime);

        // If at noise source, go back to patrolling
        if (transform.position == noiseSource)
        {
            currentState = ENEMY_STATE.PATROL;
        }
    }

    public void SentryState()
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
        targetPoint++;
        if (targetPoint >= patrolPoints.Length) targetPoint = 0;
    }

    public void FovPos()
    {
        fov.SetAimDirection(transform.up);
        fov.SetOrigin(transform.position);
    }

    public bool PlayerInSight()
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Noise"))
        {
            currentState = ENEMY_STATE.ALERTED;
            noiseSource = collision.gameObject.transform.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Game Over!!!");
        }
    }
}
