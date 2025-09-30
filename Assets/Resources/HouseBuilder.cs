using NUnit.Framework;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class HouseBuilder : MonoBehaviour
{
    public Material cubeMat;
    private GameObject madeHouse;
    public const string path = "houses";
    public string pathS = "listOfHouses";
    public const string pathOldFunctioning = "housesSaved";
    public House houseM;
    private HouseContainer hc;
    private HouseContainer sc;

    float houseWidth;
    float houseHeight;
    float houseDepth;
    int houseLayer;
    int dragLayer;
    int attLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Assign LayerMasks for the raycast to know what type of object it is detecting
        houseLayer = LayerMask.NameToLayer("House");
        dragLayer = LayerMask.NameToLayer("Drag");
        attLayer = LayerMask.NameToLayer("AttachableSurface");
        //LoadObject();
    }

    // Update is called once per frame
    void Update()
    {
        //Shortcuts when not using the menu
        if (Input.GetKeyDown(KeyCode.B))
        {
            LoadObject();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            SaveObject();
        }
    }
    //Starts loading Houses
    public void LoadObject()
    {
        //Loads in the objects from the HouseContainer class
        hc = HouseContainer.Load(pathS);
        for (int h = 0; h < hc.House.Count; h++)
        {
            //Resetting the variables used  for origin and collider
            houseWidth = 0;
            houseHeight = 0;
            houseDepth = 0;
            //Instantiates the GameObject of the first house in the list
            GameObject house = BuildObject("House", hc.House[h].name, hc.House[h].position, hc.House[h].rotation, hc.House[h].scale);
            //Instantiates all floors of the house
            Debug.Log(hc.House[h].floors.Count);
            BuildFloors(hc.House[h].floors.Count, hc.House[h].floors, house);
            //Creates the origin and pivot points for rotating the house using TiltControls
            BuildOrigin(house, houseDepth);
            BuildPivot(house, houseDepth);
            //Adds the TiltControls for rotating the house
            house.AddComponent<TiltControls>();
            //Adds the collider for the raycast and modifies it
            house.AddComponent<BoxCollider>();
            house.GetComponent<BoxCollider>().size=new Vector3(houseWidth,houseHeight,houseDepth);
            house.GetComponent<BoxCollider>().center = new Vector3(0, houseHeight / 2f, 0);
        }
    }
    //Reads the vertices needed to create a geometric shape
    private Vector3[] BuildVerticesList(int v, Shape shape)
    {
        //Builds an array for the vertices of an object
        Vector3[] newVertices = new Vector3[v];
        //Each vertices in the data is extracted to this array
        for (int i = 0; i < v; i++)
        {
            newVertices[i]=shape.vertices[i].ShapeVector();
        }
        //The array is returned for geometric shape creation
        return newVertices;
    }
    //Similar to BuildVerticesList but for UV instead of vertices
    private Vector2[] BuildUVList(int u, Shape shape)
    {
        Vector2[] newUV = new Vector2[u];
        for (int i = 0; i < u; i++)
        {
            newUV[i] = shape.uv[i].ShapeUV();
        }

        return newUV;
    }
    //Similar to BuildVerticesList but for triangles instead of vertices
    private int[] BuildTriangleList(int t, Shape shape)
    {
        int[] newTriangles = new int[t];
        for (int i = 0; i < t; i++)
        {
            newTriangles[i] = shape.triangles[i].trid;
        }

        return newTriangles;
    }
    //Builds the empty GameObject for the origin position and rotation of the House
    private void BuildOrigin(GameObject parent, float hd)
    {
        GameObject go = new GameObject("Origin");
        go.transform.parent = parent.transform;
        go.transform.localPosition = new Vector3(0, 0, -hd / 2f);
        go.transform.localRotation = Quaternion.Euler(0,0,0);
    }
    //Builds the empty GameObject for the pivot position and rotation of the House
    private void BuildPivot(GameObject parent, float hd)
    {
        GameObject go = new GameObject("Pivot");
        go.transform.parent = parent.transform;
        go.transform.localPosition = new Vector3(0, 0, -hd / 2f);
        go.transform.localRotation = Quaternion.Euler(90, 0, 0);
    }
    //Creates the floors of a house
    private void BuildFloors(int f, List<Storey> floors, GameObject parent)
    {
        //Iterate over the list of floors connected to current house from the data
        for (int i = 0; i < f; i++)
        {
            //Add the GameObject for the current floor to the scene
            GameObject floor = BuildObject("Floor", floors[i].name, floors[i].position, floors[i].rotation, floors[i].scale, parent, floors[i].material.name);
            //Increase the height of the empty House object
            houseHeight += floor.transform.localScale.y;
            //Check for size of floor and adjust house perimiter accordingly
            if (floor.transform.localScale.x > houseWidth)
            {
                houseWidth = floor.transform.localScale.x;
            }
            if (floor.transform.localScale.z > houseDepth)
            {
                houseDepth = floor.transform.localScale.z;
            }
            //Create a new mesh and name it after the floor OPTIMIZE
            Mesh mesh = new Mesh();
            mesh.name = floors[i].name;
            //Assign the new mesh component to the GameObject
            floor.GetComponent<MeshFilter>().mesh = mesh;
            //Create the geometric shape
            BuildShapes(mesh, floors[i].shape);
            //Add a collider based on the mesh
            floor.AddComponent<MeshCollider>();
            //Create any doors and windows
            BuildDoors(floors[i].doors.Count, floors[i].doors, floor);
            BuildWindows(floors[i].windows.Count, floors[i].windows, floor);
        }

    }
    //Creates the doors of a floor
    private void BuildDoors(int d, List<Door> doors, GameObject parent)
    {
        //Iterate over the list of doors connected to current floor from the data
        for (int i = 0; i < d; i++)
        {
            //Add the GameObject for the current door to the scene
            GameObject door = BuildObject("Door", doors[i].name, doors[i].position, doors[i].rotation, doors[i].scale, parent, doors[i].material.name);
            //Create a new mesh and name it after the floor OPTIMIZE
            Mesh mesh = new Mesh();
            mesh.name = doors[i].name;
            //Assign the new mesh component to the GameObject
            door.GetComponent<MeshFilter>().mesh = mesh;
            //Create the geometric shape
            BuildShapes(mesh, doors[i].shape);
            door.AddComponent<MeshCollider>();
            //Add a collider based on the mesh
        }
    }
    //Similar to BuildDoors but with windows instead
    private void BuildWindows(int w, List<Window>windows,GameObject parent)
    {//OPTIMIZE
        for (int i = 0; i < w; i++)
        {
            GameObject window = BuildObject("Window", windows[i].name, windows[i].position, windows[i].rotation, windows[i].scale, parent, windows[i].material.name);
            Mesh mesh = new Mesh();
            mesh.name = windows[i].name;
            window.GetComponent<MeshFilter>().mesh = mesh;
            BuildShapes(mesh, windows[i].shape);
            window.AddComponent<MeshCollider>();
        }
    }
    //Creates the geometric shape
    private Mesh BuildShapes(Mesh mesh, Shape shape)
    {
        //Use the vertices, vector and triangle arrays to create a mesh OPTIMIZE
        Vector3[] newVertices = new Vector3[shape.vertices.Count];
        newVertices = BuildVerticesList(shape.vertices.Count, shape);
        Vector2[] newUV = new Vector2[shape.uv.Count];
        newUV = BuildUVList(shape.uv.Count, shape);
        int[] newTriangles = new int[shape.triangles.Count];
        newTriangles = BuildTriangleList(shape.triangles.Count, shape);
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
        //Recalculate in order for correct lighting to function
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }
    //Creates the GameObject sent in
    private GameObject BuildObject(string tag, string name, Position position, Rotation rotation, Scale scale, GameObject parent=null, string mat = null, GameObject go = null)
    {
        //Debug.Log("Build Parent: " + parent);
        go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
        //Depending on type of object, each GameObject is assigned a layer
        if (tag == "House")
        {
            go.layer=houseLayer;
        }
        if (tag == "Door" || tag == "Window")
        {
            go.layer=dragLayer;
        }
        if (tag == "Floor")
        {
            go.layer = attLayer;
        }
        //Tags are added for distinguishing GameObjects within layers
        go.tag = tag;
        //Adds the object as a child if it has a parent in the data
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
        //Adds scale if it exists in data
        if (scale != null)
        {
            go.transform.localScale = scale.ScaleVector();
        }
        //Adds material to the GameObject, if none exists, then the default material will be used
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
    //If a material exists in the data base, this function will try to find it
    private void FetchMaterial(GameObject go, string mat)
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
            go.GetComponent<Renderer>().material = cubeMat;
        }
        
    }
    //This whole function completes the save functionality, it needs to be further divided in order to ensure stable funcionality
    public void SaveObject()
    {
        //Create new object to save the House in
        sc = new HouseContainer();

        //madeHouse = GameObject.FindGameObjectWithTag("House");
        //Find object with the tag House
        GameObject[] simHouses = GameObject.FindGameObjectsWithTag("House"); 
        //Iterates over the houses in the array, should be changed to for-loop to ensure faster run times on older hardware
        foreach (GameObject house in simHouses) 
        {
            //Read GameObject data and translate it into object data
            houseM = new House();
            houseM.position = new Position();
            houseM.rotation = new Rotation();
            houseM.name = house.name;
            houseM.position.SavePosition(house.transform.position);
            houseM.rotation.SaveRotation(house.transform.rotation);
            //Iterate over the children of houses
            int c = 0;
            foreach (Transform child in house.transform)
            {
                //Add any floors to the data set
                if (child.tag == "Floor") 
                {
                    //Read GameObject data and translate it into object data
                    houseM.floors.Add(new Storey());
                    houseM.floors[c].name = child.name;
                    houseM.floors[c].position= new Position();
                    houseM.floors[c].rotation= new Rotation();
                    houseM.floors[c].scale = new Scale();
                    houseM.floors[c].position.SavePosition(child.localPosition);
                    houseM.floors[c].rotation.SaveRotation(child.localRotation);
                    houseM.floors[c].scale.SaveScale(child.localScale);
                    houseM.floors[c].shape = new Shape();
                    Mesh mesh = child.GetComponent<MeshFilter>().mesh;
                    for(int i = 0; i < mesh.vertices.Length; i++)
                    {
                        houseM.floors[c].shape.vertices.Add(new Vector());
                        houseM.floors[c].shape.vertices[i].SaveVector(mesh.vertices[i]);
                    }
                    for(int i = 0;i < mesh.triangles.Length; i++)
                    {
                        houseM.floors[c].shape.triangles.Add(new Triangle());
                        houseM.floors[c].shape.triangles[i].trid = mesh.triangles[i];
                    }
                    int d = 0;
                    int w = 0;
                    //Iterate over the children of the floor
                    foreach (Transform gchild in child.transform)
                    {
                        //Add any door to the data set
                        if(gchild.tag == "Door")
                        {
                            //Read GameObject data and translate it into object data
                            //Debug.Log("Save " + gchild.name);
                            houseM.floors[c].doors.Add(new Door());
                            houseM.floors[c].doors[d].name = gchild.name;
                            houseM.floors[c].doors[d].position = new Position();
                            houseM.floors[c].doors[d].rotation = new Rotation();
                            houseM.floors[c].doors[d].scale = new Scale();
                            houseM.floors[c].doors[d].position.SavePosition(gchild.localPosition);
                            houseM.floors[c].doors[d].rotation.SaveRotation(gchild.localRotation);
                            houseM.floors[c].doors[d].scale.SaveScale(gchild.localScale);
                            houseM.floors[c].doors[d].shape = new Shape();
                            Mesh dmesh = gchild.GetComponent<MeshFilter>().mesh;
                            for (int i = 0; i < dmesh.vertices.Length; i++)
                            {
                                houseM.floors[c].doors[d].shape.vertices.Add(new Vector());
                                houseM.floors[c].doors[d].shape.vertices[i].SaveVector(dmesh.vertices[i]);
                            }
                            for (int i = 0; i < dmesh.triangles.Length; i++)
                            {
                                houseM.floors[c].doors[d].shape.triangles.Add(new Triangle());
                                houseM.floors[c].doors[d].shape.triangles[i].trid = dmesh.triangles[i];
                            }
                            Debug.Log("Vectors: "+ houseM.floors[c].doors[d].shape.vertices.Count + " Triangles: " + houseM.floors[c].doors[d].shape.triangles.Count);
                            houseM.floors[c].doors[d].material = new MaterialInfo();
                            houseM.floors[c].doors[d].material.name = gchild.GetComponent<Renderer>().sharedMaterial.name;
                            d++;
                        }
                        //Add any window to the data set
                        if (gchild.tag == "Window")
                        {
                            //Read GameObject data and translate it into object data
                            //Debug.Log("Save "+gchild.name);
                            houseM.floors[c].windows.Add(new Window());
                            houseM.floors[c].windows[w].name = gchild.name;
                            houseM.floors[c].windows[w].position = new Position();
                            houseM.floors[c].windows[w].rotation = new Rotation();
                            houseM.floors[c].windows[w].scale = new Scale();
                            houseM.floors[c].windows[w].position.SavePosition(gchild.localPosition);
                            houseM.floors[c].windows[w].rotation.SaveRotation(gchild.localRotation);
                            houseM.floors[c].windows[w].scale.SaveScale(gchild.localScale);
                            houseM.floors[c].windows[w].shape=new Shape();
                            Mesh wmesh = gchild.GetComponent<MeshFilter>().mesh;
                            for (int i = 0; i < wmesh.vertices.Length; i++) 
                            {
                                houseM.floors[c].windows[w].shape.vertices.Add(new Vector());
                                houseM.floors[c].windows[w].shape.vertices[i].SaveVector(wmesh.vertices[i]);
                            }
                            for (int i = 0;i < wmesh.triangles.Length; i++)
                            {
                                houseM.floors[c].windows[w].shape.triangles.Add(new Triangle());
                                houseM.floors[c].windows[w].shape.triangles[i].trid = wmesh.triangles[i];
                            }
                            houseM.floors[c].windows[w].material = new MaterialInfo();
                            houseM.floors[c].windows[w].material.name = gchild.GetComponent<Renderer>().sharedMaterial.name;
                            w++;
                        }
                    }
                    //Store any material from the floor
                    houseM.floors[c].material = new MaterialInfo();
                    string s = child.GetComponent<Renderer>().sharedMaterial.name;

                    string[] subs = s.Split(' ');
                    //houseM.floors[c].material.name= child.GetComponent<Renderer>().sharedMaterial.name;
                    houseM.floors[c].material.name=subs[0];
                    //Debug.Log("Material: "+houseM.floors[c].material.name);
                    c++;
                }
            }
            //Add the object to the container
            sc.House.Add(houseM);
        }
        //Save the container
        HouseContainer.Save(pathS,sc);
        
    }
}
