using System.Drawing;
using UnityEngine;

public class ChangeSizeStreetTest : MonoBehaviour
{
    private Vector3 previousScale;
    private Vector3 size = Vector3.zero;
    public MeshFilter meshFilter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Debug.Log(previousScale);
        meshFilter = GetComponent<MeshFilter>();
        size = GetComponent<MeshFilter>().sharedMesh.bounds.size;
        size = new Vector3(size.x * transform.localScale.x, size.y * transform.localScale.y, size.z * transform.localScale.z);
        previousScale = size;
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                Debug.Log(mesh.vertices[i]);
                vertices[i] = new Vector3(vertices[i].x, vertices[i].y, vertices[i].z );
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetAxis("Mouse ScrollWheel")!=0f) 
        //{
            
        //    transform.localScale += Input.GetAxis("Mouse ScrollWheel")*(new Vector3(0,0,1));
        //    size = GetComponent<MeshFilter>().sharedMesh.bounds.size;
        //    size = new Vector3(size.x * transform.localScale.x, size.y * transform.localScale.y, size.z * transform.localScale.z);
        //    Debug.Log(size+ ", "+previousScale);
        //    transform.localPosition -= (size - previousScale)/2;
        //    Debug.Log(transform.position);
        //    previousScale = size;
        //}
    }
}
