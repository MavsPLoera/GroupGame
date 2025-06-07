using TMPro;
using UnityEngine;

public class Item_Controller : MonoBehaviour
{
    public float itemCost;
    public int quantity;
    public string description;
    public string itemName;

    private void Start()
    {
        TextMeshPro[] texts = GetComponentsInChildren<TextMeshPro>();
        texts[1].text = itemCost.ToString();
        texts[0].text = $"{itemName} {(quantity > 0 ? $"X{quantity}" : "")}";
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        float temp = collision.gameObject.GetComponent<Player_Controller>().gold;

        if(temp - itemCost >= 0)
        {
            //call buy item pass in gameobject with tag for the player that bought item
            Item item = new Item();
            item.quantity = quantity;
            item.description = description;
            item.name = itemName;
            item.itemInventoryImage = GetComponent<SpriteRenderer>().sprite;

            Player_Controller.instance.addItem(item);
            Debug.Log("Bought Item!");
            Destroy(gameObject);
        }
        else
        {
            //Call UI to say you dont have enough funds
            Debug.Log("Youre Broke!");
        }
    }
}
