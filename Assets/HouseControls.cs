using UnityEngine;

public class HouseControls : MonoBehaviour
{
    public GameObject block;
    public GameObject origin;
    public GameObject pivot;
    public GameObject player;
    private float angle;
    private float degree;
    private float playerHeight;
    private float playerHeightCalibration=5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        block = transform.parent.gameObject;
        origin = block.transform.Find("Origin").gameObject;
        pivot = block.transform.Find("Pivot").gameObject;
        player = GameObject.Find("Player");
        angle = Quaternion.Angle(transform.rotation, pivot.transform.rotation);
        //Debug.Log(angle);

    }

    // Update is called once per frame
    void Update()
    {
        playerHeight = player.transform.position.y - playerHeightCalibration;

        if (playerHeight>0&&player.GetComponent<PlayerControls>().orthographic==false)
        {
            degree = playerHeight * angle / 1f;
            if (degree < angle&&degree>=0)
            {
                transform.rotation = Quaternion.RotateTowards(origin.transform.rotation, pivot.transform.rotation, degree);
            }
            //Debug.Log(degree);
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(origin.transform.rotation, pivot.transform.rotation, 0);
        }
        //if (Input.GetKey(KeyCode.Q))
        //{
        //    if (degree > 0)
        //    {
        //        degree -= angle / 100f;
        //    }
        //    transform.rotation = Quaternion.RotateTowards(origin.transform.rotation, pivot.transform.rotation, degree);
        //    Debug.Log(degree);
        //}
    }
}
