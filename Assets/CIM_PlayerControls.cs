using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
//using static UnityEditor.Progress;

public class CIM_PlayerControls : MonoBehaviour
{
    public GameObject userCamera;
    public GameObject pauseMenu;
    public GameObject buildMenu;
    public GameObject streetMenu;
    private Button largeWindowItem;
    public GameObject largeWindowButton;
    private Button smallWindowItem;
    public GameObject smallWindowButton;
    private Button doorItem;
    public GameObject doorButton;

    private Button busItem;
    public GameObject busButton;
    private Button carItem;
    public GameObject carButton;
    private Button pedestrianItem;
    public GameObject pedestrianButton;

    public float distanceModifier = 1;

    public float walkingSpeed = 10f;
    public float flyingSpeed = 0.012f;
    public float elevationFactor = 3f;

    private bool flying = false;
    private Vector3 previousRotation = Vector3.zero;
    public Vector3 rotationPoint = Vector3.zero;
    public bool orthographic = false;
    private bool overView = false;
    private bool perspective = false;

    private float rotateHorizontal;
    private float rotateVertical;

    private int layer_mask;

    private bool _cursorLocked = false;

    private GameObject selectedObject;
    private GameObject selectedBuilding;
    private GameObject selectedStreetSegment;
    private Transform selectedObjectOriginPlane;
    private float selectedObjectOriginDistance;
    private Vector3 selectedObjectOriginPosition;
    private Vector3 screenPosition;
    public Vector3 offset;
    public float test = 15;

    private int street_mask;
    private int drag_mask;
    private int att_mask;

    private bool buildingMod = false;
    private bool streetMod = false;

    private float buildingDistance;
    private float streetDistance;

    private Vector3 streetBound;

    private bool paused = false;

    private List<GameObject> streetProfiles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        largeWindowItem = largeWindowButton.GetComponent<Button>();
        largeWindowItem.onClick.AddListener(LargeWindowPlacement);
        smallWindowItem = smallWindowButton.GetComponent<Button>();
        smallWindowItem.onClick.AddListener(SmallWindowPlacement);
        doorItem = doorButton.GetComponent<Button>();
        doorItem.onClick.AddListener(DoorPlacement);
        busItem = busButton.GetComponent<Button>();
        busItem.onClick.AddListener(BusProfilePlacement);

        carItem = carButton.GetComponent<Button>();
        carItem.onClick.AddListener(CarProfilePlacement);

        pedestrianItem = pedestrianButton.GetComponent<Button>();
        pedestrianItem.onClick.AddListener(PedestrianProfilePlacement);
        layer_mask = LayerMask.GetMask("Building");
        street_mask = LayerMask.GetMask("Street");
        drag_mask = LayerMask.GetMask("Drag");
        att_mask = LayerMask.GetMask("AttachableSurface","Street");
        //canvas = GameObject.Find("Canvas");
        pauseMenu.SetActive(false);
        buildMenu.SetActive(false);
        streetProfiles = new List<GameObject>();
        //Debug.Log("StreetProfiles initialised: " + streetProfiles.Count);
        flyCam();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            if (Input.GetKey(KeyCode.W))
            {
                if (!orthographic) 
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
                if (!orthographic)
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
            if (Input.GetKey(KeyCode.Z))
            {
                if (flying)
                {

                    userCamera.GetComponent<Camera>().orthographicSize -= flyingSpeed;
                }

            }
            if (Input.GetKey(KeyCode.C))
            {
                if (flying)
                {
                    userCamera.GetComponent<Camera>().orthographicSize += flyingSpeed;
                }
            }
            rotateVertical = Input.GetAxis("Mouse Y");
            rotateHorizontal = Input.GetAxis("Mouse X");
            if (!orthographic)
            {
                transform.Rotate(Vector3.up * rotateHorizontal);
                userCamera.transform.Rotate(Vector3.left * rotateVertical);
            }
            if (overView)
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
                userCamera.GetComponent<Camera>().orthographic = orthographic;
                userCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
                transform.eulerAngles = new Vector3(0, userCamera.transform.eulerAngles.y, 0);
                transform.position = transform.position - transform.forward * 5;
                buildMenu.SetActive(false);
                streetMenu.SetActive(false);
                buildingMod = false;
                streetMod = false;
                selectedBuilding = null;
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
                    rotationPoint = new Vector3(90, 0, 0);
                    userCamera.transform.localEulerAngles = rotationPoint;//Quaternion.LookRotation(rotationPoint.y-transform.position.y);
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
                    buildingMod = true;
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
                    //buildingDistance = (hit.transform.position.z-hit.transform.GetComponent<MeshFilter>().sharedMesh.bounds.extents.z)-transform.position.z;
                    buildingDistance = (hit.transform.position.z - hit.transform.GetComponent<BoxCollider>().bounds.extents.z) - transform.position.z-distanceModifier;
                    userCamera.transform.eulerAngles = new Vector3(0, userCamera.transform.eulerAngles.y, userCamera.transform.eulerAngles.z);
                    if (_cursorLocked)
                    {
                        Hide_ShowMouseCursor();
                    }
                }
            }
            if (Physics.Raycast(userCamera.transform.position, ray, out hit, 100, street_mask))
            {
                if (Input.GetKeyDown(KeyCode.Space) && !orthographic)
                {
                    streetMod = true;
                    orthographic = true;
                    streetMenu.SetActive(true);
                    userCamera.GetComponent<Camera>().orthographic = orthographic;
                    userCamera.GetComponent<Camera>().nearClipPlane = 0.01f;
                    userCamera.GetComponent<Camera>().orthographicSize = 10;
                    GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                    GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    if (!flying)
                    {
                        flyCam();
                    }
                    selectedStreetSegment = hit.transform.gameObject;
                    transform.position = hit.transform.position + (hit.transform.up * 8);
                    transform.LookAt(hit.transform);
                    transform.position = hit.transform.position + (hit.transform.up * 15) + (hit.transform.forward * 8);
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x,hit.transform.eulerAngles.y,transform.eulerAngles.z);


                    streetBound = hit.transform.GetComponent<MeshCollider>().bounds.min;

                    //buildingDistance = (hit.transform.position.y - hit.transform.GetComponent<BoxCollider>().bounds.extents.y) - transform.position.y - distanceModifier;
                    userCamera.transform.eulerAngles = new Vector3(90, userCamera.transform.eulerAngles.y, userCamera.transform.eulerAngles.z);
                    streetDistance = userCamera.transform.position.y - (hit.transform.position.y - hit.transform.GetComponent<MeshCollider>().bounds.extents.y) -14;
                    Debug.Log("StreetDistance: " + streetDistance);
                    //userCamera.transform.LookAt(hit.transform);
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
                        //Debug.Log("Hit: " + hitSO.transform.name);
                        /*if (!(hitSO.collider.CompareTag("Window")||hitSO.collider.CompareTag("Door")))
                        {
                            return;
                        }*/

                        selectedObject = hitSO.collider.gameObject;
                        selectedObjectOriginPlane = selectedObject.transform;
                        selectedObjectOriginPosition = selectedObject.transform.position;
                        //Debug.Log("Origin: " + selectedObjectOriginPosition);
                        screenPosition = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
                        //offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
                        if (streetMod)
                        {
                            offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, streetDistance));
                            Debug.Log("Offset: "+offset);
                        }else if (buildingMod)
                        {
                            offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, buildingDistance));
                        }
                        
                        
                        selectedObjectOriginDistance = Vector3.Distance(hitSO.collider.gameObject.transform.position, userCamera.transform.position);
                        //Debug.Log("Distance: " + selectedObjectOriginDistance);
                        Cursor.visible = false;
                        //Debug.Log("GameObject: " + selectedObject.name + " Layer: " + selectedObject.layer);
                    }

                }
                else
                {
                    Vector3 position=Vector3.zero;
                    if (streetMod)
                    {
                        position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, streetDistance);
                    }
                    else if (buildingMod) 
                    {
                        position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, buildingDistance);
                    }
                    
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position) + offset;
                    if (selectedObject.transform.parent != null&&streetMod)
                    {
                        //selectedObject.transform.parent.GetComponent<MeshCollider>().bounds.extents.x
                        //Vector3 sPProfile = new Vector3(selectedObject.transform.parent.GetComponent<MeshCollider>().bounds.min.x, selectedObject.transform.parent.GetComponent<MeshCollider>().bounds.min.y, selectedObject.transform.parent.GetComponent<MeshCollider>().bounds.min.z);
                        Vector3 sPProfile = new Vector3(streetBound.x, streetBound.y, streetBound.z);
                        worldPosition = new Vector3(sPProfile.x, worldPosition.y, worldPosition.z);
                    }
                    selectedObject.transform.position = worldPosition;
                    //Debug.Log("Position " + position + " WorldPosition " + worldPosition);

                    selectedObject = null;
                    selectedObjectOriginPlane = null;
                    selectedObjectOriginPosition = new Vector3(0, 0, 0);
                    Cursor.visible = true;
                }
            }

            if (selectedObject != null && orthographic)//selectedObjectOriginPlane != null &&
            {
                RaycastHit raycastHit = RayPlane();
                if (raycastHit.transform != null)
                {
                    //if (raycastHit.transform.tag == "Wall")
                    //{
                    //Debug.Log(selectedObject.transform.parent);
                    ////if (raycastHit.transform!=null) {
                    //Debug.Log(raycastHit.transform.name);
                    if (buildingMod)
                    {
                        selectedObject.transform.parent = raycastHit.transform;
                    }
                    else
                    {
                        selectedObject.transform.parent = raycastHit.transform.parent;
                    }

                    try
                    {
                        //streetBound = selectedObject.transform.GetComponent<MeshCollider>().bounds.min;
                        Physics.Raycast(userCamera.transform.position, ray, out hit, 100, street_mask);
                        streetBound = hit.transform.GetComponent<MeshCollider>().bounds.min;
                    }
                    catch
                    {
                        
                    }
                    
                //}
                //    else
                //    {
                //        selectedObject.transform.parent = raycastHit.transform;
                //        Debug.Log(selectedObject.transform.parent);
                //    }
                }
                //Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);
                //Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, buildingDistance);
                Vector3 position = Vector3.zero;
                if (streetMod)
                {
                    position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, streetDistance);
                }
                else if (buildingMod)
                {
                    position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, buildingDistance);
                }
                //Debug.Log(position);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position) + offset;
                if (selectedObject.transform.parent != null&&streetMod)
                {
                    //selectedObject.transform.parent.GetComponent<MeshCollider>().bounds.extents.x
                    //Vector3 sPProfile = new Vector3(selectedObject.transform.parent.GetComponent<MeshCollider>().bounds.min.x, selectedObject.transform.parent.GetComponent<MeshCollider>().bounds.min.y, selectedObject.transform.parent.GetComponent<MeshCollider>().bounds.min.z);
                    Vector3 sPProfile = new Vector3(streetBound.x, streetBound.y, streetBound.z);
                    worldPosition = new Vector3(sPProfile.x, worldPosition.y, worldPosition.z);
                }
                selectedObject.transform.position = worldPosition;
                //Debug.Log("Object: "+selectedObject.transform.name+" Position: "+position+" Worldposition: "+worldPosition);
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
                        Debug.Log("Final: " + selectedObjectOriginPosition + " Object: " + selectedObject.transform.position);
                        selectedObject = null;
                        selectedObjectOriginPlane = null;
                        selectedObjectOriginPosition = new Vector3(0, 0, 0);
                        Cursor.visible = true;
                    }
                }
                if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.Backspace))
                {
                    streetProfiles.Remove(selectedObject);
                    Destroy(selectedObject);
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
        try
        {
            Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out select, 100, drag_mask);
        }
        catch
        {
            Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out select, 100, street_mask);
        }
        
        Debug.DrawRay(worldMousePosNear, worldMousePosFar - worldMousePosNear, Color.red);

        return select;
    }
    public RaycastHit RayPlane()
    {
        try
        {
            Vector3 screenMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Vector3 screenMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
            Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
            Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
            RaycastHit selectPlane;
            
            try
            {
                Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out selectPlane, 100, att_mask);
            }
            catch
            {
                Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out selectPlane, 100, street_mask);
            }

            Debug.DrawRay(worldMousePosNear, worldMousePosFar - worldMousePosNear, Color.red);
            //Debug.Log("Hit: " + selectPlane.transform.name);
            //Debug.Log("RayCastHit: " + selectPlane.transform.name);
            return selectPlane;
        }
        catch (System.Exception e) 
        {
            /*var ray = userCamera.transform.forward;
            RaycastHit hit;

            //Debug.Log("PlayerMask: "+layer_mask);
            //Debug.DrawRay(userCamera.transform.position, ray, Color.red);
            if (Physics.Raycast(userCamera.transform.position, ray, out hit, 100, layer_mask))*/
            /*var ray = userCamera.transform.forward;
            RaycastHit hit;
            Physics.Raycast(userCamera.transform.position, ray, out hit, 100, att_mask);
            Debug.DrawRay(userCamera.transform.position, ray, Color.red);
            Debug.Log("RayCastHit: " + hit.transform.name);*/
            RaycastHit hit=new RaycastHit();
            return hit;

        }
    }

    public void flyCam()
    {
        if (flying)
        {
            flying = false;
            GetComponent<Rigidbody>().useGravity = true;
            //if(streetProfiles.Count > 0)
            //{
            //    for (int i = 0; i < streetProfiles.Count; i++)
            //    {
            //        streetProfiles[i].gameObject.SetActive(false);
            //    }
            //}
            
        }
        else
        {
            flying = true;
            GetComponent<Rigidbody>().useGravity = false;
            //if (streetProfiles.Count > 0)
            //{
            //    for (int i = 0; i < streetProfiles.Count; i++)
            //    {
            //        streetProfiles[i].gameObject.SetActive(true);
            //    }
            //}
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
        Debug.Log("ScreenDepth: " + screendepth.ToString()+ " ScreenPosition: "+screenPosition);
        /*//screenPosition = Camera.main.WorldToScreenPoint(screendepth.transform.position);
        GameObject newWindow = GameObject.Instantiate(Resources.Load<GameObject>("Window 1"));
        //GameObject newWindow = Resources.Load<GameObject>("Window 1");
        //GameObject placedWindow=GameObject.Instantiate<GameObject>(newWindow);
        //Debug.Log("GameObject: "+ newWindow.name+" Layer: "+newWindow.layer);
        selectedObject = newWindow;*/


        GameObject newWindow = GameObject.Instantiate(Resources.Load<GameObject>("LargeWindow"));
        selectedObject = newWindow;
        //ObjectInstancer(selectedObject);



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

    void BusProfilePlacement()
    {
        /*RaycastHit hit;
        Physics.Raycast(userCamera.transform.position, userCamera.transform.forward, out hit, 100, layer_mask);
        *///GameObject selectedObject = hit.transform.gameObject;
        GameObject screendepth = selectedStreetSegment;
        //Debug.Log(selectedBuilding.gameObject.name);
        for (int i = 0; i < selectedStreetSegment.transform.childCount; i++)
        {
            //Debug.Log("Tag: " + selectedBuilding.transform.GetChild(i).tag);
            if (selectedStreetSegment.transform.GetChild(i).CompareTag("Floor"))
            {
                for (int j = 0; j < selectedStreetSegment.transform.GetChild(i).childCount; j++)
                {
                    if (selectedStreetSegment.transform.GetChild(i).GetChild(j).CompareTag("Window"))
                    {
                        screendepth = selectedStreetSegment.transform.GetChild(i).GetChild(j).gameObject;
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


        GameObject newBus = GameObject.Instantiate(Resources.Load<GameObject>("CIM_Bus_Front"));
        selectedObject = newBus;
        offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, streetDistance));
        offset = new Vector3(0, offset.y, 0);
        Debug.Log("Offset: " + offset+", Camera to world: "+ Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, streetDistance)));
        streetProfiles.Add(newBus);
        Debug.Log("StreetProfiles: "+streetProfiles.Count);



    }

    void CarProfilePlacement()
    {
        /*RaycastHit hit;
        Physics.Raycast(userCamera.transform.position, userCamera.transform.forward, out hit, 100, layer_mask);
        *///GameObject selectedObject = hit.transform.gameObject;
        GameObject screendepth = selectedStreetSegment;
        //Debug.Log(selectedBuilding.gameObject.name);
        for (int i = 0; i < selectedStreetSegment.transform.childCount; i++)
        {
            //Debug.Log("Tag: " + selectedBuilding.transform.GetChild(i).tag);
            if (selectedStreetSegment.transform.GetChild(i).CompareTag("Floor"))
            {
                for (int j = 0; j < selectedStreetSegment.transform.GetChild(i).childCount; j++)
                {
                    if (selectedStreetSegment.transform.GetChild(i).GetChild(j).CompareTag("Window"))
                    {
                        screendepth = selectedStreetSegment.transform.GetChild(i).GetChild(j).gameObject;
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


        GameObject newCar = GameObject.Instantiate(Resources.Load<GameObject>("CIM_Car_Front"));
        selectedObject = newCar;
        offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, streetDistance));
        offset = new Vector3(0, offset.y, 0);
        Debug.Log("Offset: " + offset + ", Camera to world: " + Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, streetDistance)));
        streetProfiles.Add(newCar);
        Debug.Log("StreetProfiles: " + streetProfiles.Count);



    }

    void PedestrianProfilePlacement()
    {
        /*RaycastHit hit;
        Physics.Raycast(userCamera.transform.position, userCamera.transform.forward, out hit, 100, layer_mask);
        *///GameObject selectedObject = hit.transform.gameObject;
        GameObject screendepth = selectedStreetSegment;
        //Debug.Log(selectedBuilding.gameObject.name);
        for (int i = 0; i < selectedStreetSegment.transform.childCount; i++)
        {
            //Debug.Log("Tag: " + selectedBuilding.transform.GetChild(i).tag);
            if (selectedStreetSegment.transform.GetChild(i).CompareTag("Floor"))
            {
                for (int j = 0; j < selectedStreetSegment.transform.GetChild(i).childCount; j++)
                {
                    if (selectedStreetSegment.transform.GetChild(i).GetChild(j).CompareTag("Window"))
                    {
                        screendepth = selectedStreetSegment.transform.GetChild(i).GetChild(j).gameObject;
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


        GameObject newPedestrians = GameObject.Instantiate(Resources.Load<GameObject>("CIM_Pedestrians"));
        selectedObject = newPedestrians;
        offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, streetDistance));
        offset = new Vector3(0, offset.y, 0);
        Debug.Log("Offset: " + offset + ", Camera to world: " + Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, streetDistance)));
        streetProfiles.Add(newPedestrians);
        Debug.Log("StreetProfiles: " + streetProfiles.Count);



    }
    private void ObjectInstancer(GameObject selectedObject)
    {
        var ray = userCamera.transform.forward;
        RaycastHit hit;
        Physics.Raycast(userCamera.transform.position, ray, out hit, 100, layer_mask);
        selectedObjectOriginPlane = hit.transform;
        selectedObjectOriginPosition = hit.transform.position;
        Debug.Log("Origin: " + selectedObjectOriginPosition);
        screenPosition = Camera.main.WorldToScreenPoint(selectedObject.transform.position);
        //offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
        if (streetMod)
        {
            offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, streetDistance));
            Debug.Log("Offset: " + offset);
        }
        else if (buildingMod)
        {
            offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, buildingDistance));
        }


        selectedObjectOriginDistance = Vector3.Distance(selectedObject.transform.position, userCamera.transform.position);
        Debug.Log("Distance: " + selectedObjectOriginDistance);
        Cursor.visible = false;
    }
}
