using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEditor;

[XmlRoot("CIM_Collection")]
public class CIMContainer
{
    [XmlArray("CIMs")]
    [XmlArrayItem("CIM")]
    public List<CIM> CIM = new List<CIM>();

    public static CIMContainer Load(string path)
    {
        //string filePath = Path.Combine(Application.dataPath + "/Resources/", path + ".xml");
        TextAsset _xml = Resources.Load<TextAsset>(path);

        XmlSerializer serializer = new XmlSerializer(typeof(CIMContainer));

        StringReader reader = new StringReader(_xml.text);

        CIMContainer CIM = serializer.Deserialize(reader) as CIMContainer;

        reader.Close();

        return CIM;
    }

    public static void Save(string path, CIMContainer CIM)
    {
        string filePath = Path.Combine(Application.dataPath + "/Resources/", path + ".xml");
        //Debug.Log(house.House[0].name);
        XmlSerializer serializer = new XmlSerializer(typeof(CIMContainer));
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


        serializer.Serialize(stream, CIM);
        //Debug.Log("Streamed");
        stream.Close();
        Debug.Log("Saved");
        AssetDatabase.Refresh();
    }
}