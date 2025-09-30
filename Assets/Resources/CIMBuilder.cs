using NUnit.Framework;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;
using UnityEditor.PackageManager.UI;
using static Unity.Burst.Intrinsics.X86;

public class CIMBuilder : MonoBehaviour
{
    public Material cubeMat;
    //private GameObject madeHouse;
    public string path = "CIM_Demo";
    public CIM cim;
    private CIMContainer cc;
    private CIMContainer sc;

    float buildingWidth;
    float buildingHeight;
    float buildingDepth;
    int buildingLayer;
    int dragLayer;
    int attLayer;
    int streetLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Assign LayerMasks for the raycast to know what type of object it is detecting
        buildingLayer = LayerMask.NameToLayer("Building");
        dragLayer = LayerMask.NameToLayer("Drag");
        attLayer = LayerMask.NameToLayer("AttachableSurface");
        streetLayer = LayerMask.NameToLayer("Street");
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
        cc = CIMContainer.Load(path);
        //Plot currentPlot = cc.CIM[0].globes[0].streetSectors[0].plotSeries[0].plots[0];
        for(int c = 0; c < cc.CIM.Count; c++)
        {
            GameObject CIM=BuildObject("CIM",cc.CIM[c].name, cc.CIM[c].position, cc.CIM[c].rotation, cc.CIM[c].scale);
            BuildGlobes(cc.CIM[c].globes.Count, cc.CIM[c].globes, CIM);
        }
    }
    private void BuildGlobes(int g, List<Globe> globes, GameObject parent)
    {
        for (int i = 0; i < g; i++)
        {
            GameObject globe = BuildObject("Globe", globes[i].name, globes[i].position, globes[i].rotation, globes[i].scale, parent);
            BuildStreetSector(globes[i].streetSectors.Count, globes[i].streetSectors, globe);
        }
    }
    private void BuildStreetSector(int ss, List<StreetSector> streetSectors, GameObject parent)
    {
        for (int i = 0; i < ss; i++)
        {
            GameObject streetSector = BuildObject("StreetSector", streetSectors[i].name, streetSectors[i].position, streetSectors[i].rotation, streetSectors[i].scale,parent);
            BuildPlotSeries(streetSectors[i].plotSeries.Count, streetSectors[i].plotSeries, streetSector);
            BuildStreetSegment(streetSectors[i].streetSegments.Count, streetSectors[i].streetSegments, streetSector);
        }
    }
    private void BuildPlotSeries(int ps, List<PlotSeries> plotSeries, GameObject parent)
    {
        for (int i = 0;i < ps; i++)
        {
            GameObject PlotS = BuildObject("PlotSeries", plotSeries[i].name, plotSeries[i].position, plotSeries[i].rotation, plotSeries[i].scale,parent);
            BuildPlot(plotSeries[i].plots.Count, plotSeries[i].plots, PlotS);
        }
    }
    private void BuildPlot(int p, List<Plot> plots, GameObject parent)
    {
        for (int i = 0; i < p; i++)
        {
            GameObject Plot = BuildObject("Plot", plots[i].name, plots[i].position, plots[i].rotation, plots[i].scale, parent);
            BuildBuildings(plots[i].buildings.Count, plots[i].buildings, Plot);
        }
    }
    private void BuildStreetSegment(int ssg, List<StreetSegment> streetSegments, GameObject parent)
    {
        for (int i = 0; i < ssg; i++)
        {
            GameObject StreetS = BuildObject("StreetSegment", streetSegments[i].name, streetSegments[i].position, streetSegments[i].rotation, streetSegments[i].scale, parent, streetSegments[i].material.name);
            Mesh mesh = new Mesh();
            mesh.name = streetSegments[i].name;
            StreetS.GetComponent<MeshFilter>().mesh = mesh;
            //Create the geometric shape
            BuildShapes(mesh, streetSegments[i].shape);
            //Add a collider based on the mesh
            StreetS.AddComponent<MeshCollider>();
            BuildStreetGrounds(streetSegments[i].streetGrounds.Count, streetSegments[i].streetGrounds, StreetS);
            BuildLanes(streetSegments[i].lanes.Count, streetSegments[i].lanes, StreetS);
        }
    }
    private void BuildStreetGrounds(int sg, List<StreetGround> streetGrounds, GameObject parent)
    {
        for (int i = 0; i < sg; i++)
        {
            GameObject streetG = BuildObject("Street", streetGrounds[i].name, streetGrounds[i].position, streetGrounds[i].rotation, streetGrounds[i].scale, parent, streetGrounds[i].material.name);
            Mesh mesh = new Mesh();
            mesh.name = streetGrounds[i].name;
            streetG.GetComponent<MeshFilter>().mesh = mesh;
            //Create the geometric shape
            BuildShapes(mesh, streetGrounds[i].shape);
            //Add a collider based on the mesh
            streetG.AddComponent<MeshCollider>();
        }
    }
    private void BuildLanes(int l, List<Lane> lanes, GameObject parent)
    {
        for (int i = 0; i < l; i++)
        {
            GameObject lane = BuildObject("Lane", lanes[i].name, lanes[i].position, lanes[i].rotation, lanes[i].scale, parent);
            Mesh mesh = new Mesh();
            mesh.name = lanes[i].name;
            //lane.GetComponent<MeshFilter>().mesh = mesh;
            //Create the geometric shape
            //BuildShapes(mesh, lanes[i].shape);
            //Add a collider based on the mesh
            //lane.AddComponent<MeshCollider>();
            lane.AddComponent<CIM_TiltControls>();
            lane.AddComponent<BoxCollider>();
            if (lane.name == "CIM_Bus_Front(Clone)")
            {
                lane.GetComponent<BoxCollider>().center = new Vector3(0, 4.45f, 0);
                lane.GetComponent<BoxCollider>().size = new Vector3(7, 10, 1);
            }
            else 
            {
                lane.GetComponent<BoxCollider>().center = new Vector3(0, 0, 0);
                lane.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
            }
            BuildLaneElements(lanes[i].lElements.Count, lanes[i].lElements, lane);
        }
    }
    private void BuildLaneElements(int le, List<LaneElement> elements, GameObject parent)
    {
        for (int i = 0; i < le; i++)
        {
            GameObject lane = BuildObject("LaneElement", elements[i].name, elements[i].position, elements[i].rotation, elements[i].scale, parent, elements[i].material.name);
            Mesh mesh = new Mesh();
            mesh.name = elements[i].name;
            lane.GetComponent<MeshFilter>().mesh = mesh;
            //Create the geometric shape
            BuildShapes(mesh, elements[i].shape);
            //Add a collider based on the mesh
            lane.AddComponent<MeshCollider>();
        }
    }
    private void BuildBuildings(int b, List<Building> buildings, GameObject parent)
    {
        for (int i = 0; i < b; i++)
        {
            //Resetting the variables used  for origin and collider
            buildingWidth = 0;
            buildingHeight = 0;
            buildingDepth = 0;
            //Instantiates the GameObject of the first house in the list
            GameObject building = BuildObject("Building", buildings[i].name, buildings[i].position, buildings[i].rotation, buildings[i].scale, parent);
            //Instantiates all floors of the house
            BuildFloors(buildings[i].floors.Count, buildings[i].floors, building);
            //Creates the origin and pivot points for rotating the house using TiltControls
            //BuildOrigin(building, buildingDepth);
            //BuildPivot(building, buildingDepth);
            //Adds the TiltControls for rotating the house
            building.AddComponent<CIM_TiltControls>();
            //Adds the collider for the raycast and modifies it
            building.AddComponent<BoxCollider>();
            building.GetComponent<BoxCollider>().size = new Vector3(buildingWidth, buildingHeight, buildingDepth);
            building.GetComponent<BoxCollider>().center = new Vector3(0, buildingHeight / 2f, 0);
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
            newVertices[i] = shape.vertices[i].ShapeVector();
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
        go.transform.localRotation = Quaternion.Euler(0, 0, 0);
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
    private void BuildFloors(int f, List<Floor> floors, GameObject parent)
    {
        //Iterate over the list of floors connected to current house from the data
        for (int i = 0; i < f; i++)
        {
            //Add the GameObject for the current floor to the scene
            GameObject floor = BuildObject("Floor", floors[i].name, floors[i].position, floors[i].rotation, floors[i].scale, parent, floors[i].material.name);
            //Create a new mesh and name it after the floor
            Mesh mesh = new Mesh();
            mesh.name = floors[i].name;
            //Assign the new mesh component to the GameObject
            floor.GetComponent<MeshFilter>().mesh = mesh;
            //Create the geometric shape
            BuildShapes(mesh, floors[i].shape);
            //Add a collider based on the mesh
            floor.AddComponent<MeshCollider>();
            Bounds size = floor.GetComponent<MeshFilter>().sharedMesh.bounds;
            //Check for size of floor and adjust house perimiter accordingly
            if (size.size.x > buildingWidth)
            {
                buildingWidth = size.size.x;
            }
            if (size.size.z > buildingDepth)
            {
                buildingDepth = size.size.z;
            }
            //Create floorPlans
            BuildFloorPlan(floors[i].floorPlan.Count, floors[i].floorPlan, floor);
        }

    }
    private void BuildFloorPlan(int fp, List<FloorPlan> floorPlans, GameObject parent)
    {
        for (int i = 0; i < fp; i++)
        {
            //Add the GameObject for the current floor to the scene
            GameObject floorPlan = BuildObject("FloorPlan", floorPlans[i].name, floorPlans[i].position, floorPlans[i].rotation, floorPlans[i].scale, parent);
            //Increase the height of the empty House object
            buildingHeight += floorPlan.transform.localScale.y;
            BuildRooms(floorPlans[i].rooms.Count, floorPlans[i].rooms, floorPlan);
        }
    }

    private void BuildRooms(int r, List<Room> rooms, GameObject parent)
    {
        for (int i = 0; i < r; i++)
        {
            //Add the GameObject for the current floor to the scene
            GameObject room = BuildObject("Room", rooms[i].name, rooms[i].position, rooms[i].rotation, rooms[i].scale, parent);
            BuildWalls(rooms[i].walls.Count, rooms[i].walls, room);
        }
    }

    private void BuildOpenings(int o, List<Opening> openings, GameObject parent)
    {
        //Iterate over the list of doors connected to current floor from the data
        for (int i = 0; i < o; i++)
        {
            //Add the GameObject for the current door to the scene
            GameObject opening = BuildObject("Opening", openings[i].name, openings[i].position, openings[i].rotation, openings[i].scale, parent, openings[i].material.name);
            //Debug.Log("Scale: "+openings[i].scale.x+" ,"+ openings[i].scale.y+" ,"+ openings[i].scale.z);
            //Create a new mesh and name it after the floor OPTIMIZE
            Mesh mesh = new Mesh();
            mesh.name = openings[i].name;
            //Assign the new mesh component to the GameObject
            opening.GetComponent<MeshFilter>().mesh = mesh;
            //Create the geometric shape
            BuildShapes(mesh, openings[i].shape);
            opening.AddComponent<MeshCollider>();
            //Add a collider based on the mesh
            //Create any doors and windows
            BuildDoors(openings[i].doors.Count, openings[i].doors, opening);
            BuildWindows(openings[i].windows.Count, openings[i].windows, opening);
        }
    }

    //Creates the walls of a floor
    private void BuildWalls(int d, List<Wall> walls, GameObject parent)
    {
        //Iterate over the list of doors connected to current floor from the data
        for (int i = 0; i < d; i++)
        {
            //Add the GameObject for the current door to the scene
            GameObject wall = BuildObject("Wall", walls[i].name, walls[i].position, walls[i].rotation, walls[i].scale, parent, walls[i].material.name);
            //Create a new mesh and name it after the floor OPTIMIZE
            Mesh mesh = new Mesh();
            mesh.name = walls[i].name;
            //Assign the new mesh component to the GameObject
            wall.GetComponent<MeshFilter>().mesh = mesh;
            //Create the geometric shape
            BuildShapes(mesh, walls[i].shape);
            wall.AddComponent<MeshCollider>();
            //Add a collider based on the mesh
            //Create any doors and windows
            /*BuildDoors(walls[i].doors.Count, walls[i].doors, wall);
            BuildWindows(walls[i].windows.Count, walls[i].windows, wall);*/
            BuildOpenings(walls[i].openings.Count, walls[i].openings, wall);
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
            //Debug.Log(doors[i].material.name);
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
    private void BuildWindows(int w, List<Window> windows, GameObject parent)
    {//OPTIMIZE
        for (int i = 0; i < w; i++)
        {
            GameObject window = BuildObject("Window", windows[i].name, windows[i].position, windows[i].rotation, windows[i].scale, parent, windows[i].material.name);
            Mesh mesh = new Mesh();
            mesh.name = windows[i].name;
            window.GetComponent<MeshFilter>().mesh = mesh;
            BuildShapes(mesh, windows[i].shape);
            //window.AddComponent<MeshCollider>();
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
    private GameObject BuildObject(string tag, string name, Position position, Rotation rotation, Scale scale, GameObject parent = null, string mat = null, GameObject go = null)
    {
        //Debug.Log("Build Parent: " + parent);
        go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
        //Depending on type of object, each GameObject is assigned a layer
        if (tag == "Building")
        {
            go.layer = buildingLayer;
        }
        if (tag == "Opening"||tag=="Lane")
        {
            go.layer = dragLayer;
        }
        if (tag == "Wall")
        {
            go.layer = attLayer;
        }
        if (tag == "Street")
        {
            go.layer = streetLayer;
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
        string filePath = Path.Combine("Materials/", mat);
        
        try
        {
            go.GetComponent<Renderer>().material = Resources.Load(filePath, typeof(Material)) as Material;
            //Debug.Log("Filepath :" + filePath);
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
        sc = new CIMContainer();
        GameObject[] CIM_Objects = GameObject.FindGameObjectsWithTag("CIM");
        int c = 0;//KOM IHÅG ATT LÄTTA TILL ITERATION
        foreach (GameObject cim in CIM_Objects) {
            sc.CIM.Add(new CIM());
            sc.CIM[c] = SaveCIM(cim);
            int g = 0;
            foreach(Transform globe in cim.transform)
            {
                sc.CIM[c].globes.Add(new Globe());
                sc.CIM[c].globes[g] = SaveGlobe(globe.gameObject);
                int s = 0;
                foreach(Transform streetsector in globe.transform)
                {
                    sc.CIM[c].globes[g].streetSectors.Add(new StreetSector());
                    sc.CIM[c].globes[g].streetSectors[s] = SaveStretSector(streetsector.gameObject);
                    int ps = 0;
                    int ss = 0;
                    foreach(Transform child in streetsector.transform)
                    {
                        if (child.tag == "PlotSeries")
                        {
                            sc.CIM[c].globes[g].streetSectors[s].plotSeries.Add(new PlotSeries());
                            sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps] = SavePlotSeries(child.gameObject);
                            int p = 0;
                            foreach(Transform plot in child.transform)
                            {
                                sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots.Add(new Plot());
                                sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p]= SavePlot(plot.gameObject);
                                int b = 0;
            //Iterates over the houses in the array, should be changed to for-loop to ensure faster run times on older hardware
                                foreach (Transform building in plot.transform)
                                {
                                    //Read GameObject data and translate it into object data
                                    sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings.Add(new Building());
                                    sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b] = SaveBuilding(building.gameObject);
                                    //Iterate over the children of houses
                                    int f = 0;
                                    foreach (Transform floor in building.transform)
                                    {
                                        //Read GameObject data and translate it into object data
                                        sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors.Add(new Floor());
                                        sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f]=SaveFloor(floor.gameObject);
                                        int fp = 0;
                                            //Iterate over the children of the floor
                                        foreach (Transform floorplan in floor.transform)
                                        {
                                            //Read GameObject data and translate it into object data
                                            sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].floorPlan.Add(new FloorPlan());
                                            sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].floorPlan[fp] = SaveFloorPlan(floorplan.gameObject);
                                            int r = 0;
                                            foreach (Transform fpchild in floorplan.transform)
                                            {
                                                if(fpchild.tag == "Room")
                                                {
                                                    //Read GameObject data and translate it into object data
                                                    sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].floorPlan[fp].rooms.Add(new Room());
                                                    sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].floorPlan[fp].rooms[r]=SaveRooms(fpchild.gameObject);
                                                    int w = 0;

                                                    foreach(Transform rchild in fpchild)
                                                    {
                                                        if (rchild.tag == "Wall")
                                                        {
                                                            //Read GameObject data and translate it into object data
                                                            sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].floorPlan[fp].rooms[r].walls.Add(new Wall());
                                                            sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].floorPlan[fp].rooms[r].walls[w] = SaveWalls(rchild.gameObject);
                                                            int o = 0;

                                                            foreach(Transform wchild in rchild)
                                                            {
                                                                if (wchild.tag == "Opening")
                                                                {
                                                                    //Read GameObject data and translate it into object data
                                                                    sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].floorPlan[fp].rooms[r].walls[w].openings.Add(new Opening());
                                                                    sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].floorPlan[fp].rooms[r].walls[w].openings[o] = SaveOpenings(wchild.gameObject);
                                                                    int d = 0;
                                                                    int ww = 0;

                                                                    foreach (Transform ochild in wchild.transform)
                                                                    {
                                                                        //Add any door to the data set
                                                                        if (ochild.tag == "Door")
                                                                        {
                                                                            //Read GameObject data and translate it into object data
                                                                            sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].floorPlan[fp].rooms[r].walls[w].openings[o].doors.Add(new Door());
                                                                            sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].floorPlan[fp].rooms[r].walls[w].openings[o].doors[d] = SaveDoors(ochild.gameObject);
                                                                            d++;
                                                                        }
                                                                        //Add any window to the data set
                                                                        if (ochild.tag == "Window")
                                                                        {
                                                                            //Read GameObject data and translate it into object data
                                                                            sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].floorPlan[fp].rooms[r].walls[w].openings[o].windows.Add(new Window());
                                                                            sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].floorPlan[fp].rooms[r].walls[w].openings[o].windows[ww] = SaveWindows(ochild.gameObject);
                                                                            ww++;
                                                                        }
                                                                    }
                                                                    o++;
                                                                }
                                                            }
                                                            w++;
                                                        }
                                                    }
                                                    r++;
                                                }
                                            }
                                            fp++;
                                        }
                                        f++;
                                    }
                                    b++;
                                }
                                p++;
                            }
                            ps++; 
                        }                        
                        if (child.tag == "StreetSegment"){
                            sc.CIM[c].globes[g].streetSectors[s].streetSegments.Add(new StreetSegment());
                            sc.CIM[c].globes[g].streetSectors[s].streetSegments[ss]=SaveStreetSegment(child.gameObject);
                            //ss++;
                            int sg = 0;
                            int l = 0;
                            foreach (Transform schild in child.transform)
                            {
                                //Debug.Log(schild.tag+" L: "+l);
                                if (schild.tag == "Street")
                                {
                                    //Debug.Log("Name: " + schild.name+" SG: "+sg+" Streetsegment: "+ sc.CIM[c].globes[g].streetSectors[s].streetSegments[ss].name);
                                    sc.CIM[c].globes[g].streetSectors[s].streetSegments[ss].streetGrounds.Add(new StreetGround());
                                    sc.CIM[c].globes[g].streetSectors[s].streetSegments[ss].streetGrounds[sg] = SaveStreetGround(schild.gameObject);
                                    sg++;
                                }
                                if (schild.tag == "Lane")
                                {
                                    sc.CIM[c].globes[g].streetSectors[s].streetSegments[ss].lanes.Add(new Lane());
                                    sc.CIM[c].globes[g].streetSectors[s].streetSegments[ss].lanes[l] = SaveLane(schild.gameObject);
                                    int le = 0;
                                    foreach (Transform lchild in schild.transform)
                                    {
                                        sc.CIM[c].globes[g].streetSectors[s].streetSegments[ss].lanes[l].lElements.Add(new LaneElement());
                                        sc.CIM[c].globes[g].streetSectors[s].streetSegments[ss].lanes[l].lElements[le] = SaveLaneElement(lchild.gameObject);
                                        le++;
                                    }
                                    l++;
                                }
                            }
                            ss++;
                        }
                    }
                    s++;
                }
                g++;
            }
            c++;
        }
        CIMContainer.Save(path, sc);
    }
    private LaneElement SaveLaneElement(GameObject go)
    {
        LaneElement laneElement = new LaneElement();
        laneElement.name = go.name;
        laneElement.position = new Position();
        laneElement.rotation = new Rotation();
        laneElement.scale = new Scale();
        laneElement.position.SavePosition(go.transform.localPosition);
        laneElement.rotation.SaveRotation(go.transform.localRotation);
        laneElement.scale.SaveScale(go.transform.localScale);
        laneElement.shape = SaveShape(laneElement.shape, go.GetComponent<MeshFilter>().mesh);
        laneElement.material = SaveMaterial(laneElement.material, go);
        return laneElement;
    }
    private Lane SaveLane(GameObject go)
    {
        Lane lane= new Lane();
        lane.name = go.name;
        lane.position = new Position();
        lane.rotation = new Rotation();
        lane.scale = new Scale();
        lane.position.SavePosition(go.transform.localPosition);
        lane.rotation.SaveRotation(go.transform.localRotation);
        lane.scale.SaveScale(go.transform.localScale);
        //lane.shape = SaveShape(lane.shape, go.GetComponent<MeshFilter>().mesh);
        //lane.material = SaveMaterial(lane.material, go);
        return lane;
    }
    private StreetGround SaveStreetGround(GameObject go)
    {
        StreetGround street = new StreetGround();
        street.name = go.name;
        street.position = new Position();
        street.rotation = new Rotation();
        street.scale = new Scale();
        street.position.SavePosition(go.transform.localPosition);
        street.rotation.SaveRotation(go.transform.localRotation);
        street.scale.SaveScale(go.transform.localScale);
        street.shape = SaveShape(street.shape, go.GetComponent<MeshFilter>().mesh);
        street.material = SaveMaterial(street.material, go);
        return street;
    }
    private StreetSegment SaveStreetSegment(GameObject go)
    {
        StreetSegment street = new StreetSegment();
        street.name = go.name;
        street.position = new Position();
        street.rotation = new Rotation();
        street.scale = new Scale();
        street.position.SavePosition(go.transform.localPosition);
        street.rotation.SaveRotation(go.transform.localRotation);
        street.scale.SaveScale(go.transform.localScale);
        street.shape = SaveShape(street.shape, go.GetComponent<MeshFilter>().mesh);
        street.material = SaveMaterial(street.material, go);
        return street;
    }
    private Shape SaveShape(Shape shape, Mesh mesh)
    {
        shape = new Shape();
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            shape.vertices.Add(new Vector());
            shape.vertices[i].SaveVector(mesh.vertices[i]);
        }
        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            shape.triangles.Add(new Triangle());
            shape.triangles[i].trid = mesh.triangles[i];
        }
        return shape;
    }
    private MaterialInfo SaveMaterial(MaterialInfo material, GameObject go)
    {
        material = new MaterialInfo();
        material.name = go.GetComponent<Renderer>().sharedMaterial.name;
        return material;
    }
    private Window SaveWindows(GameObject go)
    {
        Window window=new Window();
        window.name = go.name;
        window.position = new Position();
        window.rotation = new Rotation();
        window.scale = new Scale();
        window.position.SavePosition(go.transform.localPosition);
        window.rotation.SaveRotation(go.transform.localRotation);
        window.scale.SaveScale(go.transform.localScale);
        window.shape = SaveShape(window.shape, go.GetComponent<MeshFilter>().mesh);
        window.material = SaveMaterial(window.material, go);
        return window;
    }
    private Door SaveDoors(GameObject go)
    {
        Door door = new Door();
        door.name = go.name;
        door.position = new Position();
        door.rotation = new Rotation();
        door.scale = new Scale();
        door.position.SavePosition(go.transform.localPosition);
        door.rotation.SaveRotation(go.transform.localRotation);
        door.scale.SaveScale(go.transform.localScale);
        door.shape = SaveShape(door.shape, go.GetComponent<MeshFilter>().mesh);
        door.material = SaveMaterial(door.material, go);
        return door;
    }
    private Opening SaveOpenings(GameObject go)
    {
        Opening opening = new Opening();
        opening.name = go.name;
        opening.position = new Position();
        opening.rotation = new Rotation();
        opening.scale = new Scale();
        opening.position.SavePosition(go.transform.localPosition);
        opening.rotation.SaveRotation(go.transform.localRotation);
        opening.scale.SaveScale(go.transform.localScale);
        opening.shape = SaveShape(opening.shape, go.GetComponent<MeshFilter>().mesh);
        opening.material = SaveMaterial(opening.material, go);
        return opening;
    }
    private Wall SaveWalls(GameObject go)
    {
        Wall wall = new Wall();
        wall.name = go.name;
        wall.position = new Position();
        wall.rotation = new Rotation();
        wall.scale = new Scale();
        wall.position.SavePosition(go.transform.localPosition);
        wall.rotation.SaveRotation(go.transform.localRotation);
        wall.scale.SaveScale(go.transform.localScale);
        wall.shape = SaveShape(wall.shape, go.GetComponent<MeshFilter>().mesh);
        wall.material = SaveMaterial(wall.material, go);
        return wall;
    }
    private Room SaveRooms(GameObject go)
    {
        Room room = new Room();
        room.name = go.name;
        room.position = new Position();
        room.rotation = new Rotation();
        room.scale = new Scale();
        room.position.SavePosition(go.transform.localPosition);
        room.rotation.SaveRotation(go.transform.localRotation);
        room.scale.SaveScale(go.transform.localScale);
        return room;
    }
    private FloorPlan SaveFloorPlan(GameObject go)
    {
        FloorPlan plan = new FloorPlan();
        plan.name = go.name;
        plan.position = new Position();
        plan.rotation = new Rotation();
        plan.scale = new Scale();
        plan.position.SavePosition(go.transform.localPosition);
        plan.rotation.SaveRotation(go.transform.localRotation);
        plan.scale.SaveScale(go.transform.localScale);
        return plan;
    }
    private Floor SaveFloor(GameObject go)
    {
        Floor floor = new Floor();
        floor.name = go.name;
        floor.position = new Position();
        floor.rotation = new Rotation();
        floor.scale = new Scale();
        floor.position.SavePosition(go.transform.localPosition);
        floor.rotation.SaveRotation(go.transform.localRotation);
        floor.scale.SaveScale(go.transform.localScale);
        floor.shape = SaveShape(floor.shape, go.GetComponent<MeshFilter>().mesh);
        floor.material = SaveMaterial(floor.material, go);
        return floor;
    }
    private Building SaveBuilding(GameObject go)
    {
        Building building = new Building();
        building.name = go.name;
        building.position = new Position();
        building.rotation = new Rotation();
        building.scale = new Scale();
        building.position.SavePosition(go.transform.localPosition);
        building.rotation.SaveRotation(go.transform.localRotation);
        building.scale.SaveScale(go.transform.localScale);
        return building;
    }
    private Plot SavePlot(GameObject go)
    {
        Plot plot = new Plot();
        plot.name = go.name;
        plot.position = new Position();
        plot.rotation = new Rotation();
        plot.scale = new Scale();
        plot.position.SavePosition(go.transform.localPosition);
        plot.rotation.SaveRotation(go.transform.localRotation);
        plot.scale.SaveScale(go.transform.localScale);
        plot.shape = SaveShape(plot.shape, go.GetComponent<MeshFilter>().mesh);
        plot.material = SaveMaterial(plot.material, go);
        return plot;
    }
    private PlotSeries SavePlotSeries(GameObject go)
    {
        PlotSeries plotS = new PlotSeries();
        plotS.name = go.name;
        plotS.position = new Position();
        plotS.rotation = new Rotation();
        plotS.scale = new Scale();
        plotS.position.SavePosition(go.transform.localPosition);
        plotS.rotation.SaveRotation(go.transform.localRotation);
        plotS.scale.SaveScale(go.transform.localScale);
        return plotS;
    }
    private StreetSector SaveStretSector(GameObject go)
    {
        StreetSector s = new StreetSector();
        s.name = go.name;
        s.position = new Position();
        s.rotation = new Rotation();
        s.scale = new Scale();
        s.position.SavePosition(go.transform.localPosition);
        s.rotation.SaveRotation(go.transform.localRotation);
        s.scale.SaveScale(go.transform.localScale);
        return s;
    }
    private Globe SaveGlobe(GameObject go)
    {
        Globe globe = new Globe();
        globe.name = go.name;
        globe.position = new Position();
        globe.rotation = new Rotation();
        globe.scale = new Scale();
        globe.position.SavePosition(go.transform.localPosition);
        globe.rotation.SaveRotation(go.transform.localRotation);
        globe.scale.SaveScale(go.transform.localScale);
        return globe;
    }
    private CIM SaveCIM(GameObject go)
    {
        CIM cim = new CIM();
        cim.name = go.name;
        cim.position = new Position();
        cim.rotation = new Rotation();
        cim.scale = new Scale();
        cim.position.SavePosition(go.transform.localPosition);
        cim.rotation.SaveRotation(go.transform.localRotation);
        cim.scale.SaveScale(go.transform.localScale);
        return cim;
    }
}
//Store any material from the floor
/*sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].material = new MaterialInfo();
string str = child.GetComponent<Renderer>().sharedMaterial.name;

string[] subs = str.Split(' ');
//houseM.floors[c].material.name= child.GetComponent<Renderer>().sharedMaterial.name;
sc.CIM[c].globes[g].streetSectors[s].plotSeries[ps].plots[p].buildings[b].floors[f].material.name = subs[0];*/
//Debug.Log("Material: "+houseM.floors[c].material.name);