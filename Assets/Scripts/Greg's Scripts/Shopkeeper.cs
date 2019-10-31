using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeeper : MonoBehaviour
{

    public GameObject[] shopKeeperItems;
    public List<GameObject> itemSpots;

    // Start is called before the first frame update
    void Start()
    {
        shopKeeperItems = Resources.LoadAll<GameObject>("ShopkeeperItems");
        RandomizeItmes();
    }

    void RandomizeItmes ()
    {
        int[] items = new int[3];
        items[0] = Random.Range(0, 5);
        items[1] = items[2] = items[0];
        while (items[1] == items[0])
            items[1] = Random.Range(0, 5);
        while (items[2] == items[0] || items[2] == items[1])
            items[2] = Random.Range(0, 5);

        for (int i = 0; i < itemSpots.Count; i++)
        {
            GameObject GO = Instantiate(shopKeeperItems[items[i]], itemSpots[i].transform.position, Quaternion.Euler(0,90,0));
            GO.transform.parent = itemSpots[i].transform;
        }

    }
}
