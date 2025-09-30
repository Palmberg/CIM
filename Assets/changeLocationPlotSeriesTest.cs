using UnityEditor.Animations;
using UnityEngine;

public class changeLocationPlotSeriesTest : MonoBehaviour
{
    public GameObject street;
    public float offset=5f;
    private Vector3 size = Vector3.zero;
    private Vector3 position = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(Transform child in transform.parent)
        {
            if(child.tag == "StreetSegment")
            {
                street = child.gameObject;
            }
            size = street.GetComponent<MeshFilter>().sharedMesh.bounds.size;
            position=street.transform.localPosition;
            //Debug.Log(new Vector3(street.GetComponent<MeshFilter>().sharedMesh.bounds.size.x, street.GetComponent<MeshFilter>().sharedMesh.bounds.size.y, street.GetComponent<MeshFilter>().sharedMesh.bounds.size.z) *street.transform.localScale);
            size = new Vector3(size.x * street.transform.localScale.x, size.y * street.transform.localScale.y, size.z * street.transform.localScale.z);
            //Debug.Log("Size: "+size+" Position: "+position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.transform.localPosition=new Vector3(gameObject.transform.localScale   size.z+position.z;
        //size = street.GetComponent<MeshFilter>().sharedMesh.bounds.size;

        //size = new Vector3(size.x * street.transform.localScale.x, size.y * street.transform.localScale.y, size.z * street.transform.localScale.z);
        //position = street.transform.localPosition;

        //gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, (size.z / 2) + position.z + offset);//((size.z+position.z)/2)-(position.z/2));
        //Debug.Log("Size: " + size + " Position: " + position+" PlotSeriesPosition: "+gameObject.transform.localPosition+" Test: "+street.GetComponent<MeshFilter>().sharedMesh.bounds.max);
    }
}
