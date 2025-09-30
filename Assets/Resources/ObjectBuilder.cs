using NUnit.Framework;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class ObjectBuilder
{
    public Material cubeMat;

    int houseLayer;
    int dragLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Vector3[] BuildVerticesList(int v, Shape shape)
    {
        Vector3[] newVertices = new Vector3[v];
        for (int i = 0; i < v; i++)
        {
            newVertices[i]=shape.vertices[i].ShapeVector();
        }

        return newVertices;
    }
    public Vector2[] BuildUVList(int u, Shape shape)
    {
        Vector2[] newUV = new Vector2[u];
        for (int i = 0; i < u; i++)
        {
            newUV[i] = shape.uv[i].ShapeUV();
        }

        return newUV;
    }
    public int[] BuildTriangleList(int t, Shape shape)
    {
        int[] newTriangles = new int[t];
        for (int i = 0; i < t; i++)
        {
            newTriangles[i] = shape.triangles[i].trid;
        }

        return newTriangles;
    }
    public void BuildOrigin(GameObject parent, float hd)
    {
        GameObject go = new GameObject("Origin");
        go.transform.parent = parent.transform;
        go.transform.localPosition = new Vector3(0, 0, -hd / 2f);
        go.transform.localRotation = Quaternion.Euler(0,0,0);
    }

    public void BuildPivot(GameObject parent, float hd)
    {
        GameObject go = new GameObject("Pivot");
        go.transform.parent = parent.transform;
        go.transform.localPosition = new Vector3(0, 0, -hd / 2f);
        go.transform.localRotation = Quaternion.Euler(90, 0, 0);
    }

    public Mesh BuildShapes(Mesh mesh, Shape shape)
    {
        Vector3[] newVertices = new Vector3[shape.vertices.Count];
        newVertices = BuildVerticesList(shape.vertices.Count, shape);
        Vector2[] newUV = new Vector2[shape.uv.Count];
        newUV = BuildUVList(shape.uv.Count, shape);
        int[] newTriangles = new int[shape.triangles.Count];
        newTriangles = BuildTriangleList(shape.triangles.Count, shape);
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }
    public GameObject BuildObject(string tag, string name, Position position, Rotation rotation, Scale scale, GameObject parent=null, string mat = null, GameObject go = null)
    {
        go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
        if (tag == "House")
        {
            go.layer=houseLayer;
        }
        if (tag == "Door" || tag == "Window")
        {
            go.layer=dragLayer;
        }
        go.tag = tag;
        if (parent == null) 
        {
            go.transform.position = position.PositionVector();
            go.transform.rotation = rotation.RotationQuaternion();
        }
        else
        {
            go.transform.parent = parent.transform;
            go.transform.localPosition = position.PositionVector();
            go.transform.localRotation = rotation.RotationQuaternion();
        }
        if (scale != null)
        {
            go.transform.localScale = scale.ScaleVector();
        }

        if (mat == null)
        {
            go.GetComponent<Renderer>().material = cubeMat;
        }
        else
        {

            FetchMaterial(go, mat);
        }
            
        return go;
    }

    public void FetchMaterial(GameObject go, string mat)
    {
        //Debug.Log(mat);
        //string filePath = Path.Combine(Application.dataPath + "/Materials/", mat.name + ".mat");
        string filePath = Path.Combine("Materials/", mat);
        //Debug.Log(filePath);

        try
        {
            go.GetComponent<Renderer>().material = Resources.Load(filePath, typeof(Material)) as Material;
            //Debug.Log("Exists");
        }
        catch
        {
        }
        
    }
    
}
