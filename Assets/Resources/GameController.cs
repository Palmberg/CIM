using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Button saveButton;
    public Button loadButton;
    public const string playerPath = "Prefabs/Player";
    private static string dataPath = string.Empty;

    public static House CreateHouse(string path, Vector3 position, Quaternion rotation)
    {
        //GameObject prefab=Resources.Load<GameObject>(path);

        House house=new House();
        return house;
    }

    void OnEnable()
    {
        saveButton.onClick.AddListener(delegate { SaveData.Save(dataPath, SaveData.houseContainer); });
        loadButton.onClick.AddListener(delegate { SaveData.Load(dataPath); });
    }

    void OnDisable()
    {
        saveButton.onClick.RemoveListener(delegate { SaveData.Save(dataPath, SaveData.houseContainer); });
        loadButton.onClick.RemoveListener(delegate { SaveData.Load(dataPath); });
    }
}
