using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEditor;

[XmlRoot("StreetCollection")]
public class StreetContainer
{
    [XmlArray("Streets")]
    [XmlArrayItem("Street")]
    public List<Street> Street = new List<Street>(); 

    public static StreetContainer Load(string path)
    {
        //string filePath = Path.Combine(Application.dataPath + "/Resources/", path + ".xml");
        TextAsset _xml=Resources.Load<TextAsset>(path);

        XmlSerializer serializer = new XmlSerializer(typeof(StreetContainer));

        StringReader reader = new StringReader(_xml.text);

        StreetContainer street =serializer.Deserialize(reader) as StreetContainer;
        
        reader.Close();

        return street;
    }

    public static void Save(string path, StreetContainer street) 
    {
        string filePath = Path.Combine(Application.dataPath+"/Resources/", path+ ".xml");
        Debug.Log(street.Street[0].name);
        XmlSerializer serializer=new XmlSerializer(typeof(StreetContainer));
        FileStream stream;
        try
        {
            stream = new FileStream(filePath, FileMode.Truncate);
            //Debug.Log("Truncate");
        }
        catch (FileNotFoundException)
        {
            stream = new FileStream(filePath, FileMode.Create);
            //Debug.Log("Create");
        }
        

        serializer.Serialize(stream, street);
        //Debug.Log("Streamed");
        stream.Close();
        Debug.Log("Saved");
        AssetDatabase.Refresh();
    }
}
