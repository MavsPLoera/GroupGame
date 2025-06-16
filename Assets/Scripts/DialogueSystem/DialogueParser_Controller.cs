using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

public class DialogueParser_Controller : MonoBehaviour
{
    public static DialogueParser_Controller instance;

    void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ContextMenu("Create JSON")]
    public void createFile()
    {
        string path = Application.dataPath + "/DialogueInteractions" + "/testFile.json";
        File.WriteAllText(path, "test");
        Debug.Log(path);

    }

    [ContextMenu("Read JSON")]
    public void readFile()
    {
        retreiveConversation("testFile.json");
    }


    public List<DialogueLine> retreiveConversation(string conversationIdentifier)
    {
        //List we are going to return
        List<DialogueLine> temp = null;

        //Get path from given file name, all dialogue interactions should be in the DialogueInteractions file
        string path = Application.dataPath + "/DialogueInteractions/" + conversationIdentifier;

        if (!File.Exists(path))
        {
            Debug.LogError($"Cannot find file {conversationIdentifier}");
        }
        else
        {
            temp = JsonConvert.DeserializeObject<List<DialogueLine>>(File.ReadAllText(path));

            foreach (DialogueLine line in temp)
            {
                Debug.Log(line);

                if (line.dialogueChoices != null)
                {
                    for (int i = 0; i < line.dialogueChoices.Length; i++)
                    {
                        Debug.Log(line.dialogueChoices[i]);
                    }
                }
            }
        }

        return temp;
    }
}

public class DialogueLine
{
    //Can make this a acharacter class to allow for different sounds to be swapped in and out for building the text.
    public string speakerName;
    public string dialogue;

    /*
     * Start signal is meant to control HOW a dialogue line is appended to the text currently on the screen. 
     * Do you want to clear the text currently there? append to the text? or do either one with a wait? This allows you to do so.
     */

    public StartSignal dialogueStartSignal = 0;
    public float startSignalDelay = 0.0f;

    /*
     * Optional fields for dialogue line
     * 
     * Controller Override controlls the dialogue builder with specified values and control.
     * Commands are called at the beggining of a dialogue line being built
     * Dialouge Choice allows a user to respond to the character.
     */

    public DialogueControllerOverride controllerOverride = null;
    public DialogueCommands commands = null;
    public DialogueChoice[] dialogueChoices = null;

    public enum StartSignal { NONE, A, C, WA, WC }

    public override string ToString()
    {
        return "Speaker name: " + speakerName + ", Dialogue: " + dialogue + ", StartSignal: " + dialogueStartSignal + ", SignalDelay: " + startSignalDelay
            + ", ControllerOverride: " + controllerOverride + ", Commands: " + commands;
    }
}


public class DialogueControllerOverride
{
    public int charactersPerCycle = -1;
    public float speed = -1f;
    public bool cantBeInterrupted = false;
    public bool waitForUserInput = true;
    public BuildMode mode = 0;

    public enum BuildMode { TYPEWRITER, INSTANT }

    public override string ToString()
    {
        return "Char/cylce: " + charactersPerCycle + ", Speed: " + speed + ", CantBeInterrupted: " + cantBeInterrupted + ", WaitForUserInput: " + waitForUserInput + ", Mode: " + mode;
    }
}

public class DialogueCommands
{
    //Used to call general commands that are in the DialogueCommands manager
    public string[] commandsToCall;

    public delegate void DialogueCommand();
    public DialogueCommand delegateDialogueCommands;
}

public class DialogueChoice
{
    public string choiceText;

    //Response is a list of dialogue lines, response can also be nothing as well.
    public List<DialogueLine> responseToChoice;

    //Can optionally reference a file to be able to get the dialogue lines as well to make formatting easier.

    public override string ToString()
    {
        string choices = "Response: ";

        if (responseToChoice != null)
        {
            for (int i = 0; i < responseToChoice.Count; i++)
            {
                choices += responseToChoice[i].ToString();
                choices += ", ";
            }
        }

        return $"Choice Text: " + choiceText + (choices == "Response: " ? " " : ", " + choices);
    }
}