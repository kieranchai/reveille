using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public Mesh mesh;
    public GameObject parent;
    public float lineOfSight;
    public float fieldOfView;
    public EnemyScript _data;

    public int rayCount = 50;
    public float angle = 0f;
    public float angleIncrease;
    public Vector3 origin = Vector3.zero;
    void Start()
    {
        parent = this.transform.parent.gameObject;
       /* _data = parent.GetComponent<EnemyScript>();*/
        lineOfSight = 5f; /*_data.lineOfSight;*/
        fieldOfView = 9f; /*_data.fieldOfView;*/

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        float angleIncrease = fieldOfView / rayCount;
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



}
