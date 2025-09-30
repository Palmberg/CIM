using System.IO;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
/*
public class Building
{
    [XmlAttribute("name")]
    public string name;

    /*[XmlElement("Position")]
    //public List<Position> positions = new List<Position>();
    //public Position[] positions;
    public Vector3 position;/**//*
    [XmlElement("Position")]
    public Position position;
    //[XmlAttribute("x")]
    //public string x;
}

public class Position
{
    [XmlElement("X")]
    public float X;
    [XmlElement("Y")]
    public float y;
    [XmlElement("Z")]
    public float z;
}
*/

/*public class Building
{
    public string name { get; set; }
    public Vector3 position { get; set; }

    public Building(XmlNode curBuildNode)
    {
        name=curBuildNode.Attributes["name"].Value;

        XmlNode positionNode = curBuildNode.SelectSingleNode("Position");

        float x = float.Parse(positionNode["X"].InnerText);
        float y = float.Parse(positionNode["Y"].InnerText);
        float z = float.Parse(positionNode["Z"].InnerText);

        position = new Vector3(x, y, z);

    }

}*/