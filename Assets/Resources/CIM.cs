using System.IO;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CIM
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement(ElementName = nameof(Globe))]
    public List<Globe> globes = new List<Globe>();
}

public class Globe
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement(ElementName = nameof(StreetSector))]
    public List<StreetSector> streetSectors = new List<StreetSector>();
}

public class StreetSector
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    /*[XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;*/

    [XmlElement(ElementName = nameof(StreetSegment))]
    public List<StreetSegment> streetSegments = new List<StreetSegment>();

    [XmlElement(ElementName = nameof(PlotSeries))]
    public List<PlotSeries> plotSeries = new List<PlotSeries>();
}

public class StreetSegment
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;

    [XmlElement(ElementName = nameof(StreetGround))]
    public List<StreetGround> streetGrounds = new List<StreetGround>();

    [XmlElement(ElementName = nameof(Lane))]
    public List<Lane> lanes = new List<Lane>();
}

public class StreetGround
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;
}

public class Lane
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;

    [XmlElement(ElementName = nameof(LaneElement))]
    public List<LaneElement> lElements = new List<LaneElement>();
}
public class LaneElement
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;
}
public class PlotSeries
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    /*[XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;*/

    [XmlElement(ElementName = nameof(Plot))]
    public List<Plot> plots = new List<Plot>();

}
public class Plot
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;

    [XmlElement(ElementName = nameof(Building))]
    public List<Building> buildings = new List<Building>();

}
public class Building
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    /*[XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;*/

    [XmlElement(ElementName = nameof(Facade))]
    public List<Facade> facades = new List<Facade>();

    [XmlElement(ElementName = nameof(Floor))]
    public List<Floor> floors = new List<Floor>();

}

public class Facade
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;
}

public class Floor
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;

    [XmlElement(ElementName = nameof(FloorPlan))]
    public List<FloorPlan> floorPlan = new List<FloorPlan>();
}
//add floorplan
public class FloorPlan
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement(ElementName = nameof(Room))]
    public List<Room> rooms = new List<Room>();

    /*[XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;*/

    //[XmlElement(ElementName = nameof(Opening))]
    //public List<Opening> openings = new List<Opening>();

    //[XmlElement(ElementName = nameof(Wall))]
    //public List<Wall> walls = new List<Wall>();
}

public class Room
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement(ElementName = nameof(Wall))]
    public List<Wall> walls = new List<Wall>();
}
public class Opening
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;

    [XmlElement(ElementName = nameof(Door))]
    public List<Door> doors = new List<Door>();

    [XmlElement(ElementName = nameof(Window))]
    public List<Window> windows = new List<Window>();
}
public class Wall
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement("Shape")]
    public Shape shape;

    [XmlElement("Material")]
    public MaterialInfo material;

    [XmlElement(ElementName = nameof(Opening))]
    public List<Opening> openings = new List<Opening>();
}
//add opening before window
