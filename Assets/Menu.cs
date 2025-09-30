using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject main;
    private Button save;
    private Button load;
    private Button clear;
    private HouseBuilder houseBuilder;
    private StreetBuilder streetBuilder;
    private 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        save=transform.Find("Save").gameObject.GetComponent<Button>();
        save.onClick.AddListener(Save);
        load=transform.Find("Load").gameObject.GetComponent<Button>();
        load.onClick.AddListener(Load);
        clear=transform.Find("Clear").gameObject.GetComponent<Button>();
        clear.onClick.AddListener(Clear);
        houseBuilder=main.GetComponent<HouseBuilder>();
        streetBuilder= main.GetComponent<StreetBuilder>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Save()
    {
        houseBuilder.SaveObject();
        streetBuilder.SaveObject();
    }
    void Load()
    {
        houseBuilder.LoadObject();
        streetBuilder.LoadObject();
    }
    void Clear() 
    {
        GameObject[] simHouses = GameObject.FindGameObjectsWithTag("House");
        GameObject[] simStreets = GameObject.FindGameObjectsWithTag("Street");
        for (int i = 0; i < simHouses.Length; i++)
        {
            Destroy(simHouses[i]);
        }
        for(int i = 0;i < simStreets.Length; i++)
        {
            Destroy(simStreets[i]); 
        }
    }
}
