using UnityEngine;

public class MeshCreator : MonoBehaviour
{
    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;
    public GameObject copy;
    GameObject objToSpawn;
    int objectsSpawned = 0;
    public Material cubeMat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*newVertices = copy.GetComponent<MeshFilter>().mesh.vertices;
        Debug.Log(newVertices.Length);
        //newUV=copy.GetComponent<MeshFilter>().mesh.uv;
        newTriangles=copy.GetComponent<MeshFilter>().mesh.triangles;
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
        //mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        for(int i = 0; i < newVertices.Length; i++)
        {
            Debug.Log("Point: "+i);
            Debug.Log("Vertices X: " + newVertices[i].x+" Y: "+ newVertices[i].y + " Z: " + newVertices[i].z);
            Debug.Log("New UV X: " + newUV[i].x + " Y: " + newUV[i].y);
            Debug.Log("Triangles: " + newTriangles[i]);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadObject();
        }
    }

    private void LoadObject()
    {
        objToSpawn = new GameObject("Coded");
        objToSpawn.transform.position += Vector3.forward * objectsSpawned;
        objToSpawn.transform.localScale += Vector3.up * objectsSpawned;
        objectsSpawned++;

        objToSpawn.AddComponent<MeshFilter>();
        objToSpawn.AddComponent<MeshRenderer>();
        objToSpawn.GetComponent<Renderer>().material = cubeMat;
        
        newVertices = copy.GetComponent<MeshFilter>().mesh.vertices;
        for (int i = 0; i < newVertices.Length; i++)
        {
            Debug.Log(newVertices[i]);
        }
        newTriangles = copy.GetComponent<MeshFilter>().mesh.triangles;
        for (int i = 0; i < newTriangles.Length; i++)
        {
            Debug.Log(newTriangles[i]);
        }
        Mesh mesh = new Mesh();
        objToSpawn.GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
        mesh.RecalculateNormals();
        objToSpawn.AddComponent<MeshCollider>();
    }
}
