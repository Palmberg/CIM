using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEditor;

[XmlRoot("HouseCollection")]
public class HouseContainer
{
    [XmlArray("Houses")]
    [XmlArrayItem("House")]
    public List<House> House = new List<House>(); 

    public static HouseContainer Load(string path)
    {
        //string filePath = Path.Combine(Application.dataPath + "/Resources/", path + ".xml");
        TextAsset _xml=Resources.Load<TextAsset>(path);

        XmlSerializer serializer = new XmlSerializer(typeof(HouseContainer));

        StringReader reader = new StringReader(_xml.text);

        HouseContainer house=serializer.Deserialize(reader) as HouseContainer;
        
        reader.Close();

        return house;
    }

    public static void Save(string path, HouseContainer house) 
    {
        string filePath = Path.Combine(Application.dataPath+"/Resources/", path+ ".xml");
        //Debug.Log(house.House[0].name);
        XmlSerializer serializer=new XmlSerializer(typeof(HouseContainer));
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
        

        serializer.Serialize(stream, house);
        //Debug.Log("Streamed");
        stream.Close();
        Debug.Log("Saved");
        AssetDatabase.Refresh();
    }
}
