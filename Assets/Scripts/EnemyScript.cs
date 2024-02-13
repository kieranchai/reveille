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
/*    public Mesh mesh;*/

    public int id;
    public string enemyName;
    public float walkSpeed;
    public float runSpeed;
    public float lineOfSight;
    public float fieldOfView;

    public Vector3[] patrolPoints;
    public int targetPoint;

   /* public int rayCount = 50;
    public float angle = 0f;
    public float angleIncrease;
    public Vector3 origin = Vector3.zero;*/
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

        patrolPoints = new Vector3[this.transform.childCount-1];
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).name == "FieldOfView") { continue; }
            patrolPoints[i-1] = this.transform.GetChild(i-1).GetComponent<Transform>().position;
        }
    }

    private void Start()
    {
        setEnemyData(_data);
    }
    public void setEnemyData(Enemy data)
    {
        /*mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;*/
        this.lineOfSight = 50f; /*data.lineOfSight;*/
        this.fieldOfView = 90f; /*data.fieldOfView;*/

        this.id = data.id;
        this.enemyName = data.enemyName;
        this.walkSpeed = data.walkSpeed;
        this.runSpeed = data.runSpeed;
      
       /* float angleIncrease = fieldOfView / rayCount;
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];
        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex = origin + GetVectorFromAngle(angle) * lineOfSight;
            vertices[vertexIndex] = vertex;

            if (i>0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles; */
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
    }

    private void patrolState()
    {
        if (transform.position == patrolPoints[targetPoint])
        {
            increaseTargetPoint();
        }


        transform.up = (Vector3)(patrolPoints[targetPoint] - new Vector3(transform.position.x, transform.position.y));
        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[targetPoint], walkSpeed * Time.deltaTime);
    }

    private void increaseTargetPoint()
    {
        targetPoint++;
        if (targetPoint >= patrolPoints.Length) targetPoint = 0;
    }

    /*public Vector3 GetVectorFromAngle(float angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }*/
}
