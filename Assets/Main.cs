using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject house;
    public GameObject origin;
    public GameObject pivot;
    private float angle;
    private float degree;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        angle = Quaternion.Angle(house.transform.rotation, pivot.transform.rotation);
        //Debug.Log(angle);
    }

    // Update is called once per frame
    void Update()
    {
        
        //if (Input.GetKey(KeyCode.E))
        //{
        //    if (degree < angle)
        //    {
        //        degree += angle / 100f;
        //    }
        //    house.transform.rotation = Quaternion.RotateTowards(origin.transform.rotation, pivot.transform.rotation, degree);
        //    Debug.Log(degree);
        //}
        //if (Input.GetKey(KeyCode.Q))
        //{
        //    if (degree > 0)
        //    {
        //        degree -= angle / 100f;
        //    }
        //    house.transform.rotation = Quaternion.RotateTowards(origin.transform.rotation, pivot.transform.rotation, degree);
        //    Debug.Log(degree);
        //}
    }
}
