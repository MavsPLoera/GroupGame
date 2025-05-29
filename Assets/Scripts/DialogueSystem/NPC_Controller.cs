using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    public string[] testConversation = { "Preston as ???:\"Do you know where you are buddy?{wa 1.5} You're in a text file....\":command_Test()",
        "Bystander:\"W-w-what?{c} What are you talking about?!?\":testCommand() command_Test()", "Preston:\"Names Preston and these are just the way things roll now...{wa 0.5} You're going into a list now.\":testCommand()",
        "Bystander:\"NOOOOOOOOOOOOOOOOOOOOOO!!!!!!!!!!\":testCommand2()" , "Preston:\"Tragic tale isnt it?{a} Quite...\"" };
    public string[] testAlreadyInteractedConversation = {"narrator:\"There is nothing left but dust.\"" };

    public bool alreadyInteractedWith = false;

    public List<DialogueLine> conversationParsed;

    public void Interact()
    {
        if (!alreadyInteractedWith)
        {
            conversationParsed = DialogueParser_Controller.instance.ParseConversation(testConversation);
            StartCoroutine(Dialogue_Controller.instance.DialogueInteraction(conversationParsed));
            alreadyInteractedWith = true;
        }
        else
        {
            conversationParsed = DialogueParser_Controller.instance.ParseConversation(testAlreadyInteractedConversation);
            StartCoroutine(Dialogue_Controller.instance.DialogueInteraction(conversationParsed));
            alreadyInteractedWith = true;
        }
    }
}
