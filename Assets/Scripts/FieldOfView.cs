using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask layermask;

    public Mesh mesh;
    public GameObject parent;
    public EnemyScript _data;
    public float startingAngle;
    public float fieldOfView;
    public Vector3 origin;
    public float angle;
    float lineOfSight;

    int rayCount;
    float angleIncrease;

    public void Start()
    {
        mesh = new Mesh();
        mesh = GetComponent<MeshFilter>().mesh;
    }
    public void Update()
    { 
        rayCount = 50;
        angle = startingAngle;

        angleIncrease = fieldOfView / rayCount;
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];
     
        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex; 
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), lineOfSight, layermask);
            if (raycastHit2D.collider == null)
            {
                vertex = origin + GetVectorFromAngle(angle) * lineOfSight;
            }else
            {
                vertex = raycastHit2D.point;
            }
            vertices[vertexIndex] = vertex;

            if (i > 0)
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
        mesh.triangles = triangles;
    }

    public Vector3 GetVectorFromAngle(float angle)
    {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    public float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        
        return n;
    }
    public void SetAimDirection(Vector3 aimDirection)
    {
        startingAngle = GetAngleFromVectorFloat(aimDirection) + fieldOfView / 2f;
    }

    public void SetOrigin(Vector3 position)
    {
        this.origin = position;
    }

    public void SetFOV(float fov) {
        this.fieldOfView = fov;
    }

    public void SetLOS(float los) {
        this.lineOfSight = los;
    }

}
