using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
public class SaveData
{
    public static HouseContainer houseContainer=new HouseContainer();

    public delegate void SerializeAction();
    public static SerializeAction OnLoaded;
    public static SerializeAction OnBeforeSave;

    public static void Load(string path)
    {
        houseContainer=LoadHouses(path);
        foreach(House house in houseContainer.House)
        {

        }
        OnLoaded();
    }

    public static void Save(string path, HouseContainer house) 
    { 
        //OnBeforeSave();

        SaveHouses(path, house);

        ClearHouses();
    
    }

    public static void AddHouseData(House house)
    {
        houseContainer.House.Add(house);
    }

    public static void ClearHouses()
    {
        houseContainer.House.Clear();
    }

    private static HouseContainer LoadHouses(string path)
    {
        //TextAsset _xml = Resources.Load<TextAsset>(path);

        XmlSerializer serializer = new XmlSerializer(typeof(HouseContainer));

        FileStream stream = new FileStream(path,FileMode.Open);

        HouseContainer house = serializer.Deserialize(stream) as HouseContainer;

        stream.Close();

        return house;
    }

    private static void SaveHouses(string path, HouseContainer house)
    {
        string filePath = Path.Combine(Application.dataPath + "/Resources/", path + ".xml");
        //Debug.Log(house.House[0].name);
        XmlSerializer serializer = new XmlSerializer(typeof(HouseContainer));
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
        //Debug.Log("Closed");
        AssetDatabase.Refresh();

    }
}
