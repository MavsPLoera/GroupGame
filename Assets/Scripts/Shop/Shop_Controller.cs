using UnityEngine;

public class Shop_Controller : MonoBehaviour
{
    public ItemStock[] stock;
    public string welcomeMessgae;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterShop()
    {
        for(int i = 0; i < stock.Length; i++)
        {
            if(stock[i].invetory != 0)
                stock[i].item.SetActive(true);
        }
    }
}

[System.Serializable]
public class ItemStock
{
    public int invetory;
    public GameObject item;
}
