using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_Controller : MonoBehaviour
{
    public Item item;


    private void Start()
    {
        TextMeshPro costText = GetComponentInChildren<TextMeshPro>();
        costText.text = item.cost.ToString();

        item.itemInventoryImage = GetComponent<SpriteRenderer>().sprite;
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        float temp = collision.gameObject.GetComponent<Player_Controller>().gold;

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
        string[] temp = { $"narrator:\"{item.name} - cost {item.cost} gold.{{c}} {item.description}\"" };
        List<DialogueLine> dialogueLines = DialogueParser_Controller.instance.ParseConversation(temp);
        StartCoroutine(Dialogue_Controller.instance.DialogueInteraction(dialogueLines));
    }
}
