using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
/*
[XmlRoot("BuildingCollection")]
public class BuildingContainer
{
    [XmlArray("Buildings")]
    [XmlArrayItem("Building")]
    public List<Building> Buildings = new List<Building>(); 

    public static BuildingContainer Load(string path)
    {
        TextAsset _xml=Resources.Load<TextAsset>(path);

        XmlSerializer serializer = new XmlSerializer(typeof(BuildingContainer));

        StringReader reader = new StringReader(_xml.text);

        BuildingContainer buildings = serializer.Deserialize(reader) as BuildingContainer;
        
        reader.Close();

        return buildings;
    }
}
*/