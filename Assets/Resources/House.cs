using System.IO;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SocialPlatforms.Impl;


public class Houses
{
    [XmlElement(ElementName = nameof(House))]
    public List<House> houses = new List<House>();
}


public class House
{
    [XmlAttribute("name")]
    public string name;

    [XmlElement("Position")]
    public Position position;

    [XmlElement("Rotation")]
    public Rotation rotation;

    [XmlElement("Scale")]
    public Scale scale;

    [XmlElement(ElementName=nameof(Storey))]
    public List<Storey> floors=new List<Storey>();

}

public class Storey
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

    [XmlElement(ElementName = nameof(Window))]
    public List<Window> windows = new List<Window>();

    [XmlElement(ElementName = nameof(Door))]
    public List<Door> doors = new List<Door>();
}
public class Window
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
public class Door
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
/*
public class Shape
{
    [XmlElement("Vector")]
    public List<Vector> vertices = new List<Vector>();

    [XmlElement("UV")]
    public List<UV> uv = new List<UV>();

    [XmlElement("Triangle")]
    public List<Triangle> triangles = new List<Triangle>();
}
public class Vector
{
    [XmlElement("X")]
    public float? x;
    [XmlElement("Y")]
    public float? y;
    [XmlElement("Z")]
    public float? z;

    private Vector3 vector;
    public Vector3 ShapeVector()
    {
        return vector = new Vector3(x.GetValueOrDefault(), y.GetValueOrDefault(), z.GetValueOrDefault());
    }
    public void SaveVector(Vector3 vector)
    {
        x=vector.x; y=vector.y; z=vector.z;
    }
}
public class UV
{
    [XmlElement("X")]
    public float? x;
    [XmlElement("Y")]
    public float? y;
    [XmlElement("Z")]
    public float? z;

    private Vector2 uv;
    public Vector2 ShapeUV()
    {
        return uv = new Vector3(x.GetValueOrDefault(), y.GetValueOrDefault());
    }
}
public class Triangle
{
    [XmlAttribute("trid")]
    public int trid;
}

public class Position
{
    [XmlElement("X")]
    public float? x { get; set; }
    [XmlElement("Y")]
    public float? y { get; set; }
    [XmlElement("Z")]
    public float? z { get; set; }

    private Vector3 position;
    public Vector3 PositionVector()
    {
        return position = new Vector3(x.GetValueOrDefault(), y.GetValueOrDefault(), z.GetValueOrDefault());
    }
    public void SavePosition(Vector3 vector)
    {
        x = vector.x; y = vector.y; z = vector.z;
    }

}
public class Rotation
{
    [XmlElement("X")]
    public float? x;
    [XmlElement("Y")]
    public float? y;
    [XmlElement("Z")]
    public float? z;
    [XmlElement("W")]
    public float? w;

    private Quaternion rotation;
    public Quaternion RotationQuaternion() { 
        return rotation=new Quaternion(x.GetValueOrDefault(), y.GetValueOrDefault(), z.GetValueOrDefault(), w.GetValueOrDefault());
    }
    public void SaveRotation(Quaternion vector)
    {
        x = vector.x; y = vector.y; z = vector.z; w= vector.w;
    }
}

public class Scale
{
    [XmlElement("X")]
    public float? x { get; set; }
    [XmlElement("Y")]
    public float? y { get; set; }
    [XmlElement("Z")]
    public float? z { get; set; }

    private Vector3 scale;
    public Vector3 ScaleVector()
    {
        return scale = new Vector3(x.GetValueOrDefault(), y.GetValueOrDefault(), z.GetValueOrDefault());
    }
    public void SaveScale(Vector3 vector)
    {
        x = vector.x; y = vector.y; z = vector.z;
    }
}

public class MaterialInfo
{
    public string name { get; set; }
    //public Material mat { get; set; }
}

public class CreateHouse
{

}*/