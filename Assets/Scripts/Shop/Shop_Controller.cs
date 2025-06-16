using System.Collections.Generic;
using UnityEngine;

public class Shop_Controller : MonoBehaviour
{
    public ItemStock[] stock;
    public string welcomeMessgae;
    public string shopKeeperName;
    public string playerAsksAboutItemMessage;
    public string shopKeeperQuestionsJson;
    public string shopKeeperWhatItemMessage;
    public string exitShopMessage;

    public GameObject shopInteractBox;
    private Item selectedItem = null;

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
        //Play welcome message
        List<DialogueLine> temp = new List<DialogueLine>();
        DialogueLine welcome = new DialogueLine();
        welcome.speakerName = "narrator";
        welcome.dialogue = welcomeMessgae;
        temp.Add(welcome);
        Dialogue_Controller.instance.DialogueInteraction(temp);

        for (int i = 0; i < stock.Length; i++)
        {
            if(stock[i].invetory != 0)
                stock[i].item.SetActive(true);
        }
    }

    public void ReStock(int newValue)
    {
        for (int i = 0; i < stock.Length; i++)
        {
            stock[i].invetory = newValue;
        }
    }

    public void TalkToShopKeep()
    {
        shopInteractBox.SetActive(true);
    }

    //[ContextMenu("Test")]
    //public void Test()
    //{
    //    StartCoroutine(UI_Controller.instance.OpenSelectableInventory(selectedItem));

    //    Debug.Log(selectedItem);
    //}

    public void askAboutItem()
    {
        shopInteractBox.SetActive(false);
        List<DialogueLine> temp = new List<DialogueLine>();
        DialogueLine welcome = new DialogueLine();
        welcome.speakerName = shopKeeperName;
        welcome.dialogue = playerAsksAboutItemMessage;


        TalkToShopKeep();
    }

    public void sellItem()
    {
        shopInteractBox.SetActive(false);
        List<DialogueLine> temp = new List<DialogueLine>();
        DialogueLine sell = new DialogueLine();
        sell.speakerName = shopKeeperName;
        sell.dialogue = shopKeeperWhatItemMessage;

        TalkToShopKeep();
    }

    public void exitShop()
    {
        shopInteractBox.SetActive(false);
        List<DialogueLine> temp = new List<DialogueLine>();
        DialogueLine goodbye = new DialogueLine();
        goodbye.speakerName = shopKeeperName;
        goodbye.dialogue = exitShopMessage;
    }

    public void askQuestions()
    {
        shopInteractBox.SetActive(false);
        List<DialogueLine> questions = DialogueParser_Controller.instance.retreiveConversation(shopKeeperQuestionsJson);

        TalkToShopKeep();
    }
}

[System.Serializable]
public class ItemStock
{
    public int invetory;
    public GameObject item;
}
