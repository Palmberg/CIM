using System.Xml;
using UnityEngine;
/*
public class TestBuildingLoader : MonoBehaviour
{
    XmlDocument buildingDataXml;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            TextAsset xmlTextAsset = Resources.Load<TextAsset>("building");
            buildingDataXml = new XmlDocument();
            buildingDataXml.LoadXml(xmlTextAsset.text);
            Debug.Log(xmlTextAsset.text);
            Debug.Log(buildingDataXml.InnerText);
            //Debug.Log("Data loaded");
        }
        catch
        {
            //Debug.Log("Data not loaded");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        XmlNodeList buildings = buildingDataXml.SelectNodes("/Building");
        Debug.Log(buildings.Count);
        foreach (Building building in buildings)
        {
            Debug.Log("foreach");
            print(building.name);
            //print(building.position.z);
        }
    }
}
*/