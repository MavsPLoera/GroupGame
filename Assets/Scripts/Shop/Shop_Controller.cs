using UnityEngine;

public class Shop_Controller : MonoBehaviour
{
    public GameObject[] itemsToBuy;

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
        for(int i = 0; i < itemsToBuy.Length; i++)
        {
            itemsToBuy[i].SetActive(true);
        }
    }
}
