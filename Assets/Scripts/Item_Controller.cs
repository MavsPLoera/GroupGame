using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_Controller : MonoBehaviour
{
    public Item item;


    private void Start()
    {
        TextMeshPro costText = GetComponentInChildren<TextMeshPro>();

        if(item.cost != 0)
            costText.text = item.cost.ToString();

        item.itemInventoryImage = GetComponent<SpriteRenderer>().sprite;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        float temp = Player_Controller.instance.gold;

        if(temp - item.cost >= 0)
        {
            Player_Controller.instance.gold -= item.cost;
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

    public void showItemDescription()
    {
        string[] temp = { $"narrator:\"{item.name}{(item.quantity > 1 ? $" X{item.quantity}" : "")} - cost {(item.cost > 1f ? item.cost.ToString() : $"no")} gold.{{c}} {item.description}\"" };
        List<DialogueLine> dialogueLines = DialogueParser_Controller.instance.ParseConversation(temp);
        StartCoroutine(Dialogue_Controller.instance.DialogueInteraction(dialogueLines));
    }
}
