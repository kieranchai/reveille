using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
/*using static EnemyScript;*/

public class EnemyScript : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private NoiseController _noiseController;
    public Enemy _data;
    private Transform header;
    private Vector2 heading;
    private Vector2 noiseSource;


    public int id;
    public string enemyName;
    public float walkSpeed;
    public float runSpeed;
    public float lineOfSight;
    public float fieldOfView;

    public Vector2[] patrolPoints;
    public int targetPoint;
    public Vector2 aimdir;
    public Vector3 originalRotaion;

    public float chaseTimer;
    public float sentryTimer;
    public float alertTimer;

    [SerializeField] private LayerMask enemylayermask;

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

    private void Awake()
    {
        header = transform.Find("Heading");
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        patrolPoints = new Vector2[this.transform.childCount-1];
        for (int i = 0; i < this.transform.childCount-1; i++)
        {
            patrolPoints[i] = this.transform.GetChild(i).GetComponent<Transform>().position;
        }
    }

    private void Start()
    {
        setEnemyData(_data);
    }
    public void setEnemyData(Enemy data)
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

    private void Update()
    {
        heading = new Vector2(header.position.x, header.position.y);
        switch (currentState)
        {
            case ENEMY_STATE.PATROL:
                patrolState();
                break;
            case ENEMY_STATE.SENTRY:
                sentryState();
                break;
            case ENEMY_STATE.ALERTED:
                alertState();
                break;
            case ENEMY_STATE.CHASE:
                chaseState();
                break;
        }
        fovPos();
        UpdateAimDir();
    }

    private void patrolState()
    {
        if ((Vector2)transform.position == patrolPoints[targetPoint])
        {
            increaseTargetPoint();
        }

        transform.up = (Vector2)(patrolPoints[targetPoint] - new Vector2(transform.position.x, transform.position.y));
        transform.position = Vector2.MoveTowards(transform.position, patrolPoints[targetPoint], walkSpeed * Time.deltaTime);
        FindTargetPlayer();
    }

    public void chaseState()
    {
        if (chaseTimer < 8)
        {
            transform.up = (Vector2)PlayerManager.instance.GetPosition() - new Vector2(transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)PlayerManager.instance.GetPosition(), runSpeed * Time.deltaTime);
            chaseTimer += Time.deltaTime;
        }else
        {
            currentState = ENEMY_STATE.PATROL;
            sentryTimer = 0;
        }
    }

    public void sentryState()
    {
        if (sentryTimer < 5)
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * 30f, 60f) - 45); //temp 30 is speed 60 is angle
            sentryTimer += Time.deltaTime;
            FindTargetPlayer();
        }
        else
        {
            currentState = ENEMY_STATE.PATROL; 
            chaseTimer = 0;
        }
    }

    public void alertState()
    {
        if (alertTimer < 10)
        {
            transform.up = noiseSource - new Vector2(transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, noiseSource, walkSpeed * Time.deltaTime);
            alertTimer += Time.deltaTime;
        }else
        {
            currentState = ENEMY_STATE.PATROL;
            alertTimer = 0; 
        }

    }


    private void increaseTargetPoint()
    {
        targetPoint++;
        if (targetPoint >= patrolPoints.Length) targetPoint = 0;
    }

    public void fovPos()
    {
        fov.SetAimDirection(aimdir);
        fov.SetOrigin(transform.position);
    }

    public void FindTargetPlayer() {
        if (Vector3.Distance(GetPosition(), PlayerManager.instance.GetPosition()) < lineOfSight) {
            Vector3 dirToPlayer = (PlayerManager.instance.GetPosition() - GetPosition()).normalized;

          if (Vector3.Angle(aimdir, dirToPlayer) < fieldOfView / 2f)
            {
                RaycastHit2D raycastHit2D = Physics2D.Raycast(GetPosition(), dirToPlayer, lineOfSight, enemylayermask);
                if (raycastHit2D.collider != null)
                {
                    if (raycastHit2D.collider.gameObject.tag == "Player")
                    {
                        currentState = ENEMY_STATE.CHASE;
                    }
                }
            }
        }
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void UpdateAimDir()
    {
        aimdir = ((Vector2)(heading - new Vector2(transform.position.x, transform.position.y))).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Noise"))
        {
            currentState = ENEMY_STATE.ALERTED;
            noiseSource = collision.gameObject.transform.position;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Game Over");
        }
    }
}
