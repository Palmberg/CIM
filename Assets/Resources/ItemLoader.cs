using UnityEngine;

public class ItemLoader : MonoBehaviour
{
    public const string path = "items";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ItemContainer ic=ItemContainer.Load(path);
        for(int i = 0; i < ic.Items.Count; i++)
        {
            print(ic.Items[i].name);
        }
    }


}
