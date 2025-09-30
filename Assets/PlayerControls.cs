using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlayerControls : MonoBehaviour
{
    public GameObject userCamera;
    public GameObject pauseMenu;
    public GameObject buildMenu;
    private Button largeWindowItem;
    public GameObject largeWindowButton;
    private Button smallWindowItem;
    public GameObject smallWindowButton;
    private Button doorItem;
    public GameObject doorButton;

    public float walkingSpeed = 10f;
    public float flyingSpeed = 0.012f;
    public float elevationFactor = 3f;

    private bool flying = false;
    private Vector3 previousRotation= Vector3.zero;
    public Vector3 rotationPoint=Vector3.zero;
    public bool orthographic = false;
    private bool overView = false;
    private bool perspective=false;

    private float rotateHorizontal;
    private float rotateVertical;

    private int layer_mask;

    private bool _cursorLocked=false;

    private GameObject selectedObject;
    private GameObject selectedBuilding;
    private Transform selectedObjectOriginPlane;
    private float selectedObjectOriginDistance;
    private Vector3 selectedObjectOriginPosition;
    private Vector3 screenPosition;
    private Vector3 offset;
    public float test = 15;

    private int drag_mask;
    private int att_mask;

    private bool paused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        largeWindowItem = largeWindowButton.GetComponent<Button>();
        largeWindowItem.onClick.AddListener(LargeWindowPlacement);
        smallWindowItem = smallWindowButton.GetComponent<Button>();
        smallWindowItem.onClick.AddListener(SmallWindowPlacement);
        doorItem = doorButton.GetComponent<Button>();
        doorItem.onClick.AddListener(DoorPlacement);
        layer_mask = LayerMask.GetMask("House");
        drag_mask = LayerMask.GetMask("Drag");
        att_mask = LayerMask.GetMask("AttachableSurface");
        //canvas = GameObject.Find("Canvas");
        pauseMenu.SetActive(false);
        buildMenu.SetActive(false);
        flyCam();
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            if (Input.GetKey(KeyCode.W))
            {
                if (flying)
                {
                    transform.position += transform.forward * flyingSpeed;
                }
                else
                {
                    GetComponent<Rigidbody>().AddForce(transform.forward * walkingSpeed);
                }
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (flying)
                {
                    transform.position -= transform.right * flyingSpeed;
                }
                else
                {
                    GetComponent<Rigidbody>().AddForce(-transform.right * walkingSpeed);
                }

            }
            if (Input.GetKey(KeyCode.S))
            {
                if (flying)
                {
                    transform.position -= transform.forward * flyingSpeed;
                }
                else
                {
                    GetComponent<Rigidbody>().AddForce(-transform.forward * walkingSpeed);
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (flying)
                {
                    transform.position += transform.right * flyingSpeed;
                }
                else
                {
                    GetComponent<Rigidbody>().AddForce(transform.right * walkingSpeed);
                }
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                flyCam();
            }
            if (Input.GetKey(KeyCode.Q))
            {
                if (flying)
                {
                    transform.position -= transform.up * flyingSpeed * elevationFactor;
                }

            }
            if (Input.GetKey(KeyCode.E))
            {
                if (flying)
                {
                    transform.position += transform.up * flyingSpeed * elevationFactor;
                }
            }
            rotateVertical = Input.GetAxis("Mouse Y");
            rotateHorizontal = Input.GetAxis("Mouse X");
            if (!orthographic)
            {
                transform.Rotate(Vector3.up * rotateHorizontal);
                userCamera.transform.Rotate(Vector3.left * rotateVertical);
            }
            if(overView)
            {
                transform.Rotate(Vector3.up * rotateHorizontal);
            }
            if (perspective)
            {
                transform.Rotate(Vector3.up * rotateHorizontal);
                userCamera.transform.Rotate(Vector3.left * rotateVertical);
            }
            if (Input.GetKeyDown(KeyCode.LeftControl) && orthographic && selectedObject == null)
            {
                if (flying)
                {
                    flyCam();
                }
                orthographic = false;
                buildMenu.SetActive(false);
                selectedBuilding = null;
                userCamera.GetComponent<Camera>().orthographic = orthographic;
                userCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
                transform.position = transform.position - transform.forward * 5;
                if (!_cursorLocked)
                {
                    Hide_ShowMouseCursor();
                }
            }
            if (flying && Input.GetKeyDown(KeyCode.O))
            {
                if (!orthographic)
                {
                    orthographic = true;
                    overView = true;
                    userCamera.GetComponent<Camera>().orthographic = orthographic;
                    userCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
                    userCamera.GetComponent<Camera>().orthographicSize = 15;
                    previousRotation = userCamera.transform.rotation.eulerAngles;
                    rotationPoint = new Vector3(90,0,0);
                    userCamera.transform.localEulerAngles=rotationPoint;//Quaternion.LookRotation(rotationPoint.y-transform.position.y);
                }
                else
                {
                    overView = false;
                    userCamera.GetComponent<Camera>().orthographic = false;
                    userCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
                    orthographic = false;
                    userCamera.transform.localEulerAngles = previousRotation;
                }
            } 
            if (Input.GetKeyDown(KeyCode.P))
            {
                Debug.Log("P");
                if (!perspective)
                {
                    Debug.Log("!perspective");
                    perspective = true;
                    userCamera.GetComponent<Camera>().orthographic = true;
                    userCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
                    userCamera.GetComponent<Camera>().orthographicSize = 8;
                    
                }
                else
                {
                    Debug.Log("perspective");
                    userCamera.GetComponent<Camera>().orthographic = false;
                    userCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
                    perspective = false;
                }
            }
            var ray = userCamera.transform.forward;
            RaycastHit hit;

            //Debug.Log("PlayerMask: "+layer_mask);
            //Debug.DrawRay(userCamera.transform.position, ray, Color.red);
            if (Physics.Raycast(userCamera.transform.position, ray, out hit, 100, layer_mask))
            {
                if (Input.GetKeyDown(KeyCode.Space) && !orthographic)
                {
                    orthographic = true;
                    buildMenu.SetActive(true);
                    userCamera.GetComponent<Camera>().orthographic = orthographic;
                    userCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
                    userCamera.GetComponent<Camera>().orthographicSize = 4;
                    GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                    GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    if (!flying)
                    {
                        flyCam();
                    }
                    selectedBuilding = hit.transform.gameObject;
                    transform.position = hit.transform.position - (hit.transform.forward * 8);
                    transform.LookAt(hit.transform);
                    transform.position = hit.transform.position + (hit.transform.up * 5) - (hit.transform.forward * 8);
                    userCamera.transform.eulerAngles = new Vector3(0, userCamera.transform.eulerAngles.y, userCamera.transform.eulerAngles.z);
                    if (_cursorLocked)
                    {
                        Hide_ShowMouseCursor();
                    }
                }
            }
            if (Input.GetMouseButtonDown(0) && orthographic)
            {
                if (selectedObject == null && selectedObjectOriginPlane == null)
                {
                    RaycastHit hitSO = RaySelect();
                    if (hitSO.collider != null)
                    {
                        /*if (!(hitSO.collider.CompareTag("Window")||hitSO.collider.CompareTag("Door")))
                        {
                            return;
                        }*/

                        selectedObject = hitSO.collider.gameObject;
                        selectedObjectOriginPlane = selectedObject.transform;
                        selectedObjectOriginPosition = selectedObject.transform.position;
                        Debug.Log("Origin: " + selectedObjectOriginPosition);
                        screenPosition = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
                        offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
                        selectedObjectOriginDistance = Vector3.Distance(hitSO.collider.gameObject.transform.position, userCamera.transform.position);
                        //Debug.Log("Distance: " + selectedObjectOriginDistance);
                        Cursor.visible = false;
                        //Debug.Log("GameObject: " + selectedObject.name + " Layer: " + selectedObject.layer);
                    }

                }
                else
                {
                    Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position) + offset;
                    selectedObject.transform.position = worldPosition;
                    //Debug.Log("Position " + position + " WorldPosition " + worldPosition);

                    selectedObject = null;
                    selectedObjectOriginPlane = null;
                    selectedObjectOriginPosition = new Vector3(0,0,0);
                    Cursor.visible = true;
                }
            }

            if (selectedObject != null && orthographic)//selectedObjectOriginPlane != null &&
            {
                RaycastHit raycastHit = RayPlane();
                //if (raycastHit.transform.tag == "Floor")
                //{
                
                selectedObject.transform.parent = raycastHit.transform;
                Debug.Log(selectedObject.transform.parent);
                //}
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position) + offset;
                selectedObject.transform.position = worldPosition;
                //Debug.Log("Position "+position+" WorldPosition "+worldPosition);
                if (Input.GetMouseButtonDown(1)) 
                {
                    if (selectedObjectOriginPlane == null)
                    {
                        Destroy(selectedObject);
                        selectedObject = null;
                        selectedObjectOriginPlane = null;
                        selectedObjectOriginPosition = new Vector3(0, 0, 0);
                        Cursor.visible = true;
                    }
                    else
                    {
                        Debug.Log("Restore");
                        selectedObject.transform.position = selectedObjectOriginPosition;
                        Debug.Log("Final: " + selectedObjectOriginPosition+" Object: "+selectedObject.transform.position);
                        selectedObject = null;
                        selectedObjectOriginPlane = null;
                        selectedObjectOriginPosition = new Vector3(0, 0, 0);
                        Cursor.visible = true;
                    }
                }
                if (Input.GetMouseButtonDown(2)||Input.GetKeyDown(KeyCode.Backspace))
                {
                    Destroy (selectedObject);
                    selectedObject = null;
                    selectedObjectOriginPlane = null;
                    selectedObjectOriginPosition = new Vector3(0, 0, 0);
                    Cursor.visible = true;
                }

            }


            if (Input.GetKeyDown(KeyCode.H))
            {
                Hide_ShowMouseCursor();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                paused = true;
                pauseMenu.SetActive(true);
                if (_cursorLocked == true)
                {
                    Hide_ShowMouseCursor();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                paused = false;
                pauseMenu.SetActive(false);
                Hide_ShowMouseCursor();
            }
        }
    }

    public RaycastHit RaySelect()
    {
        Vector3 screenMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit select;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out select, 100, drag_mask);
        Debug.DrawRay(worldMousePosNear, worldMousePosFar - worldMousePosNear, Color.red);

        return select;
    }
    public RaycastHit RayPlane()
    {
        Vector3 screenMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit selectPlane;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out selectPlane, 100, att_mask);
        Debug.DrawRay(worldMousePosNear, worldMousePosFar - worldMousePosNear, Color.red);
        //Debug.Log("RayCastHit: " + selectPlane.transform.name);
        return selectPlane;
    }

    public void flyCam()
    {
        if (flying)
        {
            flying = false;
            GetComponent<Rigidbody>().useGravity = true;
        }
        else
        {
            flying = true;
            GetComponent<Rigidbody>().useGravity = false;
        }
    }
    // Toggle mouse cursor lock mode
    public void Hide_ShowMouseCursor()
    {
        if (!_cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            _cursorLocked = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            _cursorLocked = false;
        }
    }
    void LargeWindowPlacement()
    {
        /*RaycastHit hit;
        Physics.Raycast(userCamera.transform.position, userCamera.transform.forward, out hit, 100, layer_mask);
        *///GameObject selectedObject = hit.transform.gameObject;
        GameObject screendepth=selectedBuilding;
        //Debug.Log(selectedBuilding.gameObject.name);
        for(int i = 0; i < selectedBuilding.transform.childCount; i++)
        {
            //Debug.Log("Tag: " + selectedBuilding.transform.GetChild(i).tag);
            if (selectedBuilding.transform.GetChild(i).CompareTag("Floor"))
            {
                for(int j = 0; j < selectedBuilding.transform.GetChild(i).childCount; j++)
                {
                    if (selectedBuilding.transform.GetChild(i).GetChild(j).CompareTag("Window"))
                    {
                        screendepth = selectedBuilding.transform.GetChild(i).GetChild(j).gameObject;
                        break;
                    }
                }
            }
            
        }

        screenPosition = Camera.main.WorldToScreenPoint(screendepth.transform.position);
        /*//screenPosition = Camera.main.WorldToScreenPoint(screendepth.transform.position);
        GameObject newWindow = GameObject.Instantiate(Resources.Load<GameObject>("Window 1"));
        //GameObject newWindow = Resources.Load<GameObject>("Window 1");
        //GameObject placedWindow=GameObject.Instantiate<GameObject>(newWindow);
        //Debug.Log("GameObject: "+ newWindow.name+" Layer: "+newWindow.layer);
        selectedObject = newWindow;*/

        
            GameObject newWindow = GameObject.Instantiate(Resources.Load<GameObject>("LargeWindow"));
            selectedObject = newWindow;
        
        

    }
    void SmallWindowPlacement()
    {
        /*RaycastHit hit;
        Physics.Raycast(userCamera.transform.position, userCamera.transform.forward, out hit, 100, layer_mask);
        *///GameObject selectedObject = hit.transform.gameObject;
        GameObject screendepth = selectedBuilding;
        //Debug.Log(selectedBuilding.gameObject.name);
        for (int i = 0; i < selectedBuilding.transform.childCount; i++)
        {
            //Debug.Log("Tag: " + selectedBuilding.transform.GetChild(i).tag);
            if (selectedBuilding.transform.GetChild(i).CompareTag("Floor"))
            {
                for (int j = 0; j < selectedBuilding.transform.GetChild(i).childCount; j++)
                {
                    if (selectedBuilding.transform.GetChild(i).GetChild(j).CompareTag("Window"))
                    {
                        screendepth = selectedBuilding.transform.GetChild(i).GetChild(j).gameObject;
                        break;
                    }
                }
            }

        }

        screenPosition = Camera.main.WorldToScreenPoint(screendepth.transform.position);
        /*//screenPosition = Camera.main.WorldToScreenPoint(screendepth.transform.position);
        GameObject newWindow = GameObject.Instantiate(Resources.Load<GameObject>("Window 1"));
        //GameObject newWindow = Resources.Load<GameObject>("Window 1");
        //GameObject placedWindow=GameObject.Instantiate<GameObject>(newWindow);
        //Debug.Log("GameObject: "+ newWindow.name+" Layer: "+newWindow.layer);
        selectedObject = newWindow;*/


        
            GameObject newWindow = GameObject.Instantiate(Resources.Load<GameObject>("SmallWindow"));
            selectedObject = newWindow;
        

    }
    void DoorPlacement()
    {
        /*RaycastHit hit;
        Physics.Raycast(userCamera.transform.position, userCamera.transform.forward, out hit, 100, layer_mask);
        *///GameObject selectedObject = hit.transform.gameObject;
        GameObject screendepth = selectedBuilding;
        //Debug.Log(selectedBuilding.gameObject.name);
        for (int i = 0; i < selectedBuilding.transform.childCount; i++)
        {
            //Debug.Log("Tag: " + selectedBuilding.transform.GetChild(i).tag);
            if (selectedBuilding.transform.GetChild(i).CompareTag("Floor"))
            {
                for (int j = 0; j < selectedBuilding.transform.GetChild(i).childCount; j++)
                {
                    if (selectedBuilding.transform.GetChild(i).GetChild(j).CompareTag("Window"))
                    {
                        screendepth = selectedBuilding.transform.GetChild(i).GetChild(j).gameObject;
                        break;
                    }
                }
            }

        }

        screenPosition = Camera.main.WorldToScreenPoint(screendepth.transform.position);
        /*//screenPosition = Camera.main.WorldToScreenPoint(screendepth.transform.position);
        GameObject newWindow = GameObject.Instantiate(Resources.Load<GameObject>("Window 1"));
        //GameObject newWindow = Resources.Load<GameObject>("Window 1");
        //GameObject placedWindow=GameObject.Instantiate<GameObject>(newWindow);
        //Debug.Log("GameObject: "+ newWindow.name+" Layer: "+newWindow.layer);
        selectedObject = newWindow;*/


        
            GameObject newDoor = GameObject.Instantiate(Resources.Load<GameObject>("Door"));
            selectedObject = newDoor;
        

    }

}
