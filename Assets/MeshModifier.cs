using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshModifier : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] newUV;
    int[] newTriangles;
    List<Vector4> originPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        normals = mesh.normals;
        newUV = mesh.uv;
        newTriangles = mesh.triangles;
        for (var i = 0; i < vertices.Length; i++)
        {
            //vertices[i] += normals[i] * Mathf.Sin(Time.time);
            Debug.Log("Vertices: " + vertices[i] + ", Normals: " + normals[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vector = vertices[i];
                if (vector.y == -0.5f && vector.z == 0.5f)
                {
                    //vertices[i] += normals[i];
                    vertices[i] += new Vector3(0,0,1);
                }
            }
            //vertices[0] += normals[0];
            //vertices[1] += normals[1];

            //Debug.Log("Vertices: " + vertices[0] + ", Normals: " + normals[0]);
            mesh.Clear();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            originPoints = new List<Vector4>();
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vector = vertices[i];
                if (vector.y == 0.5f)
                {
                    originPoints.Add(new Vector4(vector.x, vector.y, vector.z, i));
                    vertices[i] = new Vector3(0.5f, -0.5f, 0.5f);
                    //vertices[i] = new Vector3(0.5f, 0, 0.5f);
                    //vertices[i] += normals[i];
                    //vertices[i] += new Vector3(0, 0, 1);
                }

            }
            mesh.Clear();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            for(int i = 0; i < originPoints.Count; i++)
            {
                Vector3 point = new Vector3(originPoints[i].x, originPoints[i].y, originPoints[i].z);
                int v = (int) originPoints[i].w;
                vertices[v] = point;
            }
            mesh.Clear();
        }
        
        mesh.vertices = vertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
    }
}
