using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_Controller : MonoBehaviour
{
    public Item item;

    private void Start()
    {
        item.itemImage = GetComponent<SpriteRenderer>().sprite;
    }

    public void PickUpItem()
    {
        float temp = Player_Controller.instance.gold;

        if (!Player_Controller.instance.itemDiscovered.ContainsKey(item.itemName))
        {
            Player_Controller.instance.FoundNewItem(item);
            UI_Controller.instance.foundNewItem(item);
        }

        Player_Controller.instance.addItem(item);
        Debug.Log("Bought Item!");
        gameObject.SetActive(false);
    }
}
