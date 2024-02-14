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


    public int id;
    public string enemyName;
    public float walkSpeed;
    public float runSpeed;
    public float lineOfSight;
    public float fieldOfView;

    public Vector2[] patrolPoints;
    public int targetPoint;
    public Vector2 aimdir;

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
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        patrolPoints = new Vector2[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++)
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
        switch (currentState)
        {
            case ENEMY_STATE.PATROL:
                patrolState();
                break;
            case ENEMY_STATE.SENTRY:
            case ENEMY_STATE.ALERTED:
            case ENEMY_STATE.CHASE:
                break;
        }
        fovPos();
    }

    private void patrolState()
    {
        if ((Vector2)transform.position == patrolPoints[targetPoint])
        {
            increaseTargetPoint();
        }

        transform.up = (Vector2)(patrolPoints[targetPoint] - new Vector2(transform.position.x, transform.position.y));
        transform.position = Vector2.MoveTowards(transform.position, patrolPoints[targetPoint], walkSpeed * Time.deltaTime);
        aimdir = ((Vector2)(patrolPoints[targetPoint] - new Vector2(transform.position.x, transform.position.y))).normalized;
        fov.SetAimDirection(aimdir);
        fov.SetOrigin(transform.position);
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
        
    }

    public Vector3 GetPosition()
    {
        return this.transform.position;
    }
}
