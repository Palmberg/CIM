using NUnit.Framework;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class StreetBuilder : MonoBehaviour
{
    public Material cubeMat;
    public const string path = "houses";
    public string pathS = "testStreets";
    public Street street;
    private StreetContainer rc;
    private StreetContainer sc;

    private ObjectBuilder ob;

    float streetWidth;
    float streetHeight;
    float streetDepth;
    int houseLayer;
    int dragLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ob=new ObjectBuilder();
        houseLayer = LayerMask.NameToLayer("House");
        dragLayer = LayerMask.NameToLayer("Drag");
        //LoadObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            LoadObject();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SaveObject();
        }
    }

    public void LoadObject()
    {
        rc = StreetContainer.Load(pathS);
        for (int i = 0; i < rc.Street.Count; i++)
        {
            streetWidth = 0;
            streetHeight = 0;
            streetDepth = 0;
            GameObject street = ob.BuildObject("Street", rc.Street[i].name, rc.Street[i].position, rc.Street[i].rotation, rc.Street[i].scale);
            BuildRoads(rc.Street[i].road.Count, rc.Street[i].road, street);
            BuildPavement(rc.Street[i].pavement.Count, rc.Street[i].pavement, street);
            ob.BuildOrigin(street, streetDepth);
            ob.BuildPivot(street, streetDepth);
            /*street.AddComponent<TiltControls>();
            street.AddComponent<BoxCollider>();
            street.GetComponent<BoxCollider>().size=new Vector3(houseWidth,houseHeight,houseDepth);
            street.GetComponent<BoxCollider>().center = new Vector3(0, houseHeight / 2f, 0);*/
        }
    }

    private void BuildRoads(int r, List<Road> roads, GameObject parent)
    {
        for (int i = 0; i < r; i++)
        {
            GameObject road = ob.BuildObject("Road", roads[i].name, roads[i].position, roads[i].rotation, roads[i].scale, parent, roads[i].material.name);
            //streetHeight += road.transform.localScale.y;
            /*if (road.transform.localScale.x > streetWidth)
            {
                streetWidth = road.transform.localScale.x;
            }
            if (road.transform.localScale.z > streetDepth)
            {
                streetDepth = road.transform.localScale.z;
            }*/
            
            Mesh mesh = new Mesh();
            mesh.name = roads[i].name;
            road.GetComponent<MeshFilter>().mesh = mesh;
            ob.BuildShapes(mesh, roads[i].shape);
            road.AddComponent<MeshCollider>();
        }

    }

    private void BuildPavement(int p, List<Pavement> pavements, GameObject parent)
    {
        for (int i = 0; i < p; i++)
        {
            GameObject door = ob.BuildObject("Pavement", pavements[i].name, pavements[i].position, pavements[i].rotation, pavements[i].scale, parent, pavements[i].material.name);
            Mesh mesh = new Mesh();
            mesh.name = pavements[i].name;
            door.GetComponent<MeshFilter>().mesh = mesh;
            ob.BuildShapes(mesh, pavements[i].shape);
            door.AddComponent<MeshCollider>();
        }
    }

    public void SaveObject()
    {
        sc = new StreetContainer();
        //madeStreet = GameObject.FindGameObjectWithTag("Street");
        GameObject[] simStreets = GameObject.FindGameObjectsWithTag("Street"); 

        foreach (GameObject street in simStreets) 
        {
            Street streetM = new Street();
            streetM.position = new Position();
            streetM.rotation = new Rotation();
            streetM.name = street.name;
            streetM.position.SavePosition(street.transform.position);
            streetM.rotation.SaveRotation(street.transform.rotation);
            int r = 0;
            int p = 0;
            foreach (Transform child in street.transform)
            {
                if (child.transform.tag == "Road")
                {
                    streetM.road.Add(new Road());
                    streetM.road[r].name = child.name;
                    streetM.road[r].position = new Position();
                    streetM.road[r].rotation = new Rotation();
                    streetM.road[r].scale = new Scale();
                    streetM.road[r].position.SavePosition(child.localPosition);
                    streetM.road[r].rotation.SaveRotation(child.localRotation);
                    streetM.road[r].scale.SaveScale(child.localScale);
                    streetM.road[r].shape = new Shape();
                    Mesh mesh = child.GetComponent<MeshFilter>().mesh;
                    for (int i = 0; i < mesh.vertices.Length; i++)
                    {
                        streetM.road[r].shape.vertices.Add(new Vector());
                        streetM.road[r].shape.vertices[i].SaveVector(mesh.vertices[i]);
                    }
                    for (int i = 0; i < mesh.triangles.Length; i++)
                    {
                        streetM.road[r].shape.triangles.Add(new Triangle());
                        streetM.road[r].shape.triangles[i].trid = mesh.triangles[i];
                    }
                    streetM.road[r].material = new MaterialInfo();
                    string s = child.GetComponent<Renderer>().sharedMaterial.name;
                    string[] subs = s.Split(' ');
                    streetM.road[r].material.name = subs[0];
                    r++;
                }
                else if (child.transform.tag == "Pavement")
                {
                    streetM.pavement.Add(new Pavement());
                    streetM.pavement[p].name = child.name;
                    streetM.pavement[p].position = new Position();
                    streetM.pavement[p].rotation = new Rotation();
                    streetM.pavement[p].scale = new Scale();
                    streetM.pavement[p].position.SavePosition(child.localPosition);
                    streetM.pavement[p].rotation.SaveRotation(child.localRotation);
                    streetM.pavement[p].scale.SaveScale(child.localScale);
                    streetM.pavement[p].shape = new Shape();
                    Mesh mesh = child.GetComponent<MeshFilter>().mesh;
                    for (int i = 0; i < mesh.vertices.Length; i++)
                    {
                        streetM.pavement[p].shape.vertices.Add(new Vector());
                        streetM.pavement[p].shape.vertices[i].SaveVector(mesh.vertices[i]);
                    }
                    for (int i = 0; i < mesh.triangles.Length; i++)
                    {
                        streetM.pavement[p].shape.triangles.Add(new Triangle());
                        streetM.pavement[p].shape.triangles[i].trid = mesh.triangles[i];
                    }
                    streetM.pavement[p].material = new MaterialInfo();
                    string s = child.GetComponent<Renderer>().sharedMaterial.name;
                    string[] subs = s.Split(' ');
                    streetM.pavement[p].material.name = subs[0];
                    p++;
                }

                
            }
            sc.Street.Add(streetM);
        }
        StreetContainer.Save(pathS,sc);
        
    }
}
