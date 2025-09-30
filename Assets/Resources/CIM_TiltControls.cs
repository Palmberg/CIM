using System.Collections;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class CIM_TiltControls : MonoBehaviour
{
    private GameObject origin;
    private GameObject pivot;
    private GameObject player;
    private float angle;
    private float degree;
    private float playerHeight;
    private float playerHeightCalibration = 5;
    private Quaternion originRotation;
    private Quaternion pivotRotation;
    private Vector3 originPoint;
    public Vector3 pivotPoint;
    private bool rotating = false;
    private bool rotated = false;
    private Vector3 bounds;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //player = GameObject.Find("Player");
        if (transform.tag == "Building")
        {
            bounds = transform.GetComponent<BoxCollider>().bounds.min;
            BoxCollider box=transform.GetComponent<BoxCollider>();
            Vector3 halfSize=box.size*0.5f;

            //Vector3 localMin = box.center - halfSize;
            Vector3 localMin;
            if (transform.rotation.eulerAngles.y == 0)
            {
                localMin = transform.position - halfSize;
            }
            else
            {
                localMin = transform.position + halfSize;
            }
            //Debug.Log("HalfSize: " + halfSize + " LocalMin: " + localMin);
            //pivotPoint = new Vector3(transform.position.x, transform.position.y, bounds.z);
            pivotPoint = new Vector3(transform.position.x, transform.position.y, localMin.z);
        }
        else
        {
            pivotPoint = this.transform.position;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        //playerHeight = player.transform.position.y - playerHeightCalibration;

        //if (playerHeight > 0 && player.GetComponent<PlayerControls>().orthographic == false)
        //{
        //    degree = playerHeight * angle / 1f;
        //    if (degree < angle && degree >= 0)
        //    {
        //        //transform.rotation = Quaternion.RotateTowards(originRotation, pivotRotation, degree);
        //        //transform.Rotate(origin.transform.position, degree,Space.Self);
        //        //RotateAround(transform, pivotPoint, new Vector3(1, 0, 0), degree);
        //        //transform.RotateAround(pivotPoint, new Vector3(90, 0, 0), degree);
        //        /*if (!rotating)
        //        {
        //            StartCoroutine(RotateAroundPivot());
        //        }*/
        //    }
        //    //Debug.Log(degree);
        //}
        //else
        //{
        //    //transform.rotation = Quaternion.RotateTowards(originRotation, pivotRotation, 0);
        //}
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!rotating)
            {
                if (!rotated)
                {
                    if (transform.tag != "Building")
                    {
                        pivotPoint = this.transform.position;
                    }
                    StartCoroutine(RotateAroundPivot(90));
                    rotated = true;
                }
                else
                {
                    StartCoroutine(RotateAroundPivot(-90));
                    rotated = false;
                }
            }
        }
    }

    static void RotateAround(Transform transform, Vector3 pivotPoint, Vector3 axis, float angle)
    {
        Quaternion rot = Quaternion.AngleAxis(angle, axis);
        transform.position = rot * (transform.position - pivotPoint) + pivotPoint;
        transform.rotation = rot * transform.rotation;
    }

    private IEnumerator RotateAroundPivot(int d)
    {
        rotating = true;
        float rotatedDegrees = 0;
        if (d > 0)
        {
            while (rotatedDegrees <= 90f)
            {
                //Vector3 rotation = new Vector3(1, 0, 0);
                Vector3 rotation = this.transform.right;
                float anglePerFrame = d * Time.deltaTime;

                transform.RotateAround(pivotPoint, rotation, anglePerFrame);

                rotatedDegrees += anglePerFrame;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (rotatedDegrees >= -90f)
            {
                //Vector3 rotation = new Vector3(1, 0, 0);

                Vector3 rotation = this.transform.right;
                float anglePerFrame = d * Time.deltaTime;

                transform.RotateAround(pivotPoint, rotation, anglePerFrame);

                rotatedDegrees += anglePerFrame;
                yield return new WaitForEndOfFrame();
            }
        }


        //transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, angle, transform.rotation.eulerAngles.z);
        rotating = false;
    }
}
