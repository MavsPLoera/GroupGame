using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    public string interactionFile;
    public string alreadyInteractedFile;
    public bool alreadyInteractedWith = false;

    public void Interact()
    {
        if (!alreadyInteractedWith)
        {
            List<DialogueLine> temp = DialogueParser_Controller.instance.retreiveConversation(interactionFile);
            StartCoroutine(Dialogue_Controller.instance.DialogueInteraction(temp));
            alreadyInteractedWith = true;
        }
        else
        {
            List<DialogueLine> temp = DialogueParser_Controller.instance.retreiveConversation(alreadyInteractedFile);
            StartCoroutine(Dialogue_Controller.instance.DialogueInteraction(temp));
        }
    }
}
