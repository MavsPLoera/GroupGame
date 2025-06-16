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

        Player_Controller.instance.gold -= item.cost;
        Player_Controller.instance.addItem(item);
        gameObject.SetActive(false);
    }

    public void showItemDescription()
    {
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

        //Ask player if they want the item
        DialogueLine line3 = new DialogueLine();
        line3.speakerName = "narrator";
        line3.dialogue = "Do you wish to buy the item?";

        //Yes Response
        DialogueLine YesResponse = new DialogueLine();
        YesResponse.speakerName = "narrator";

        if(Player_Controller.instance.gold - item.cost >= 0)
        {
            YesResponse.dialogue = "You have bought the item.";
            YesResponse.commands = new DialogueCommands();
            YesResponse.commands.delegateDialogueCommands += BuyItem;
        }
        else
        {
            YesResponse.dialogue = "Seems you dont have enough gold.";
        }

        DialogueChoice YesChoice = new DialogueChoice();
        YesChoice.choiceText = "Yes";
        YesChoice.responseToChoice = new List<DialogueLine> { YesResponse };


        //No response
        DialogueChoice NoChoice = new DialogueChoice();
        NoChoice.choiceText = "No";

        line3.dialogueChoices = new DialogueChoice[] {YesChoice, NoChoice};
        itemDescriptionLine.Add(line3);

        StartCoroutine(Dialogue_Controller.instance.DialogueInteraction(itemDescriptionLine));
    }
}
