using UnityEngine;

public class HouseLoader : MonoBehaviour
{
    public const string path = "houses";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HouseContainer hc=HouseContainer.Load(path);
        print("Name: " + hc.House[0].name + " Position: " + hc.House[0].position.x);
        //print(hc.House[0].floors[0].shape.vertices[0].ShapeVector());
        //print("Name: "+hc.House[0].name+" Position: (" + hc.House[0].position.x+", "+ hc.House[0].position.y + ", "+hc.House[0].position.z + ")");
        for (int i = 0; i < hc.House[0].floors.Count; i++)
        {
            print(hc.House[0].floors[i].name);

            /*for(int j = 0; j < hc.House[0].floors[i].windows.Count; j++)
            {
                print(hc.House[0].floors[i].windows[j].name);
            }
            for(int k = 0; k < hc.House[0].floors[i].doors.Count; k++)
            {
                print(hc.House[0].floors[i].doors[k].name);
            }
            for (int k = 0; k < hc.House[0].floors[i].shape.vertices.Count; k++)
            {
                print(hc.House[0].floors[i].shape.vertices[k].vertices);
            }*/
            for (int k = 0; k < hc.House[0].floors[i].shape.vertices.Count; k++)
            {
                print(hc.House[0].floors[i].shape.vertices[k].ShapeVector());
            }
        }
    }


}
