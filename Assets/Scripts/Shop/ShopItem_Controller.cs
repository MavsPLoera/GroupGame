using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ShopItem_Controller : MonoBehaviour
{
    public Item item;
    private void Start()
    {
        TextMeshPro costText = GetComponentInChildren<TextMeshPro>();

        //if (item.cost != 0)
        //    costText.text = item.cost.ToString();

        item.itemInventoryImage = GetComponent<SpriteRenderer>().sprite;
    }

    public void BuyItem()
    {
        float temp = Player_Controller.instance.gold;

        if (temp - item.cost >= 0)
        {
            if (!Player_Controller.instance.itemDiscovered.ContainsKey(item.name))
            {
                Player_Controller.instance.FoundNewItem(item);
                UI_Controller.instance.foundNewItem(item);
            }

            Player_Controller.instance.gold -= item.cost;
            Player_Controller.instance.addItem(item);
            Debug.Log("Bought Item!");
            gameObject.SetActive(false);
        }
        else
        {
            //Call UI to say you dont have enough funds
            Debug.Log("Youre Broke!");
        }
    }

    public void showItemDescription()
    {
        //TODO
        //string[] temp = { $"narrator:\"{item.name}{(item.quantity > 1 ? $" X{item.quantity}" : "")} - cost {(item.cost > 1f ? item.cost.ToString() : $"no")} gold.{{c}} {item.description}\"" };
        List<DialogueLine> itemDescriptionLine = new List<DialogueLine>();

        DialogueLine line1 = new DialogueLine();
        line1.speakerName = "narrator";
        line1.dialogue = $"{item.name}{(item.quantity > 1 ? $" X{item.quantity}" : "")} - cost {(item.cost > 1f ? item.cost.ToString() : $"no")} gold.";
        itemDescriptionLine.Add(line1);

        if (item.description != "")
        {
            DialogueLine line2 = new DialogueLine();
            line2.speakerName = "narrator";
            line2.dialogue = item.description;
            itemDescriptionLine.Add(line2);
        }

        StartCoroutine(Dialogue_Controller.instance.DialogueInteraction(itemDescriptionLine));
    }
}
