using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DialogueParser_Controller : MonoBehaviour
{
    /*
     * 
     * COMMENT CODE TO EXPLAIN
     * MIGHT WANT TO CONSIDER ADDING COMMANDS TO CHANGE SPEED OF TEXT BEING BUILT AND NUMBER OF CHARACTERS TO SHOW.
     * OR EVEN CHANGING THE COLOR OF THE TEXT
     * 
     * what is here is fine for a baseline though
     * 
     */

    private static Regex interactionRegex = new Regex(@"(.+?(?: as .+?)?):(?:\[(.+?)\])?""(.+?)""(?::(.+))?", RegexOptions.Compiled);
    private static Regex dialogueLinesRegex = new Regex(@"(?:\{(?:[ca]|(?:w[ca]\s\d*\.?\d+))\})?.+?[\.\!\?\:\;]+", RegexOptions.Compiled);
    private static Regex lineSignalRegex = new Regex(@"\{([a-zA-Z]+)(?:\s(\d*\.?\d+))?\}", RegexOptions.Compiled);
    private static Regex commandsRegex = new Regex(@"(\w+\(\w*(?:\,\s\w+)*\))", RegexOptions.Compiled);
    private static Regex dialogueBuildMethod = new Regex(@"(?:ch/c:\s(\d+),\sspeed:\s(\d*\.?\d+),\scantBeInterupted:\s(true|false),\s)?mode:\s(tw|i)");
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

    void Update()
    {
        
    }

    [ContextMenu("Parse")]
    public void testParseConversation()
    {
        string[] testConversation = { "Preston as ???:\"Do you know where you are buddy?{wa 1.5} You're in a text file....\":command_Test()",
        "Bystander:\"W-w-what?{c} What are you talking about?!?\":testCommand() command_Test()", "Preston:\"Names Preston and these are just the way things roll now...{wa 0.5} You're going into a list now.\":testCommand()",
        "Bystander:\"NOOOOOOOOOOOOOOOOOOOOOO!!!!!!!!!!\":testCommand2()" , "Preston:\"Tragic tale isnt it?{a} Quite...\"" };

        ParseConversation(testConversation);
    }

    public List<DialogueLine> ParseConversation(string[] conversation)
    {
        List<DialogueLine> dialogueConversation = new List<DialogueLine>();

        foreach(string line in conversation)
        {
            Match matches = interactionRegex.Match(line);
            string name = "";

            int index = matches.Groups[1].Value.IndexOf(" as ");
            if (index == -1)
            {
                if (matches.Groups[1].Value != "narrator")
                {
                    name = matches.Groups[1].Value;
                }
                else
                {
                    name = "";
                }
                    
            }
            else
            {
                name = matches.Groups[1].Value.Substring(index + 4);
            }

            MatchCollection dialogueLines = dialogueLinesRegex.Matches(matches.Groups[3].Value);

            foreach (Match dialogueLine in dialogueLines)
            {
                Dialogue temp = new Dialogue();
                Match lineCommand = lineSignalRegex.Match(dialogueLine.Value);

                switch (lineCommand.Groups[1].Value)
                {
                    case "c":
                        temp.signal = Dialogue.StartSignal.C;
                        temp.delay = 0.0f;
                        temp.dialogue = dialogueLine.Value.Substring(3);
                        break;
                    case "a":
                        temp.signal = Dialogue.StartSignal.A;
                        temp.delay = 0.0f;
                        temp.dialogue = dialogueLine.Value.Substring(3);
                        break;
                    case "wc":
                        temp.signal = Dialogue.StartSignal.WC;
                        temp.delay = float.Parse(lineCommand.Groups[2].Value);
                        index = dialogueLine.Value.IndexOf("}");
                        temp.dialogue = dialogueLine.Value.Substring(index + 1);
                        break;
                    case "wa":
                        temp.signal = Dialogue.StartSignal.WA;
                        temp.delay = float.Parse(lineCommand.Groups[2].Value);
                        index = dialogueLine.Value.IndexOf("}");
                        temp.dialogue = dialogueLine.Value.Substring(index + 1);
                        break;
                    default:
                        temp.signal = Dialogue.StartSignal.NONE;
                        temp.delay = 0.0f;
                        temp.dialogue = dialogueLine.Value;
                        break;
                }


                Match lineBuildMethod = dialogueBuildMethod.Match(matches.Groups[2].Value);
                
                if (lineBuildMethod.Groups[1].Value != string.Empty)
                {
                    temp.dialogueModifier = new DialogueModifier();
                    temp.dialogueModifier.charactersPerCycle = int.Parse(lineBuildMethod.Groups[1].Value);
                    temp.dialogueModifier.characterBuildSpeed = float.Parse(lineBuildMethod.Groups[2].Value);
                    temp.dialogueModifier.cantBeInterupted = bool.Parse(lineBuildMethod.Groups[3].Value);
                }

                //Debug.Log(lineBuildMethod.Groups[4].Value);
                switch (lineBuildMethod.Groups[4].Value)
                {
                    case "tw":
                        temp.buildMethod = Dialogue.BuildMethod.typeWriter;
                        break;
                    case "i":
                        temp.buildMethod = Dialogue.BuildMethod.instant;
                        break;
                    default:
                        temp.buildMethod = Dialogue.BuildMethod.typeWriter;
                        break;
                }

                dialogueConversation.Add(new DialogueLine(name, temp));
            }
        }

        return dialogueConversation;
    }
}

[System.Serializable]
public class DialogueLine
{
    public string name;
    public Dialogue dialogue;
    public string command;

    public DialogueLine(string name, Dialogue dialogue, string command = "")
    {
        this.name = name;
        this.dialogue = dialogue;   
        this.command = command;
    }

    public override string ToString()
    {
        return "Speaker: " + name + ", " + dialogue + ", Command: " + (command == "" ? "none" : command);
    }
}

[System.Serializable]
public class Dialogue
{
    public string dialogue;
    public float delay;
    public StartSignal signal;
    public BuildMethod buildMethod;
    public DialogueModifier dialogueModifier;

    public enum BuildMethod {typeWriter, instant};

    public enum StartSignal { NONE, A, C, WA, WC }

    public override string ToString()
    {
        return "Start Signal: " + signal + ", Delay: "  + delay.ToString() + ", " + dialogue;
    }
}

[System.Serializable]
public class DialogueModifier
{
    public int charactersPerCycle = -1;
    public float characterBuildSpeed = -1;
    public bool cantBeInterupted = false;
}