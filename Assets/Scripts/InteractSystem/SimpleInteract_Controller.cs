using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class SimpleInteract_Controller : MonoBehaviour
{
    public string[] testConversation;
    public string[] testAlreadyInteractedConversation;
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