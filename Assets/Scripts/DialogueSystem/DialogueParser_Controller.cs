using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

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

    private static Regex interactionRegex = new Regex(@"(.+?(?: as .+?)?):(?:\[(.+?)\])?""(.+?)""(?:(?::choice(.+))|(?::commands\s(.+)))?", RegexOptions.Compiled);
    private static Regex dialogueLinesRegex = new Regex(@"(?:\{(?:[ca]|(?:w[ca]\s\d*\.?\d+))\})?.+?[\.\!\?\:\;]+", RegexOptions.Compiled);
    private static Regex lineSignalRegex = new Regex(@"\{([a-zA-Z]+)(?:\s(\d*\.?\d+))?\}", RegexOptions.Compiled);
    private static Regex dialogueChoices = new Regex(@"\[(.+?),\s(?:(\d+)|(.+?))\]");
    private static Regex dialogueBuildMethod = new Regex(@"(?:(?:ch/c:\s(\d+),\s)?(?:speed:\s(\d*\.?\d+),\s)?(?:cantBeInterupted:\s(true|false),\s)*(?:waitForUserInput:\s(true|false),\s)?)?mode:\s(tw|i)");
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

    [ContextMenu("Parse")]
    public void testParseConversation()
    {
        string[] testConversation = { "Preston as ???:[ch/c: 2, cantBeInterupted: true, waitForUserInput: false, mode: tw]\"Do you know where you are buddy?\":choice [Yes, 2], [No, 5]",
        "Bystander:\"W-w-what?{c} What are you talking about?!?\":commands test command_Test", "Preston:\"Names Preston and these are just the way things roll now...{wa 0.5} You're going into a list now.\":commands testCommand",
        "Bystander:\"NOOOOOOOOOOOOOOOOOOOOOO!!!!!!!!!!\":commands testCommand2" , "Preston:\"Tragic tale isnt it?{a} Quite...\""};

        List<DialogueLine> temp = ParseConversation(testConversation);

        foreach (DialogueLine line in temp)
        {
            Debug.Log(line);
        }

        StartCoroutine(Dialogue_Controller.instance.DialogueInteraction(temp));
    }

    //Pass in a conversation, we want to take this conversation and transform it into lines to send to the dialogue controller.
    public List<DialogueLine> ParseConversation(string[] conversation)
    {
        //Prepare list of dialogue lines to return to the thing that triggered parse conversation
        List<DialogueLine> dialogueConversation = new List<DialogueLine>();

        /*
         * A dialogue line follows a structure of 
         * 
         * name : dialogueBuilding modifications : dialogue : (dialogue choice or commands)
         * 
         * want to use the regex above to be able to break line into these parts and use the other regex to break the line further down and extract specific parts of each line.
         */

        foreach (string line in conversation)
        {
            //Match line to the structure we expect a dialogue line to be in. That way we can "group" each section of the dialogue line for further manipulation.
            Match matches = interactionRegex.Match(line);

            string name = "";

            /*
             * Take the first group of interactionRegex and check the (.+?(?: as .+?)?) value. We want to know if the speakers name wants to be hidden so we check for the command "as"
             * if not we will set name to be the name defined at the begining of the line.
             * if the line is narrator we will set the text to be blank. 
             */

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

            /*
             * Take the third group of interactionRegex and check the ""(.+?)"" value. 
             * When building dialogue lines, characters can have multiple dialogue lines in one string. We use the dialogueLinesRegex to break up these lines into multiple lines.
             */

            MatchCollection dialogueLines = dialogueLinesRegex.Matches(matches.Groups[3].Value);


            /*
             * This comes in handy with combination of the of the names and dialogue lines. If you want the speaker to have multiple lines that have the same modifiers
             * The way this is programmed achieves that. Although the trade of is, that if you want different behavior for a new line, you will have to add another line into the conversation.
             */

            foreach (Match dialogueLine in dialogueLines)
            {
                //Create temp dialogue variable to add to dialogue line.
                Dialogue temp = new Dialogue();
                List<Choice> choices = new List<Choice>();

                //A line can modify how it wants to be built to the dialogue text, so we will use another lineSignalRegex to get either the {a}, {c}, {wa}, {wc} command at the begining of a line and set the build method in the dialogue class.
                Match lineCommand = lineSignalRegex.Match(dialogueLine.Value);

                //Use this switch statement to know when the dialogue begins in the string and if the command has a delay add that value to the dialogue class.
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

                /*
                 * This section is specifically for any modifications to the dialogue controller.
                 * if we want to change the speed of the characters being built, if a line cant be interrupted by user input or to skip waiting for user input. This section checks for any of those modifications.
                 * [REMEMBER] this will apply to all dialogue lines on the same line. so if you have multiple dialogue lines they will all have the same behaviour. This matters more when creating dialogue choices. 
                 */

                Match lineBuildMethod = dialogueBuildMethod.Match(matches.Groups[2].Value);

                if (matches.Groups[2].Value != string.Empty)
                {
                    temp.dialogueModifier = new DialogueModifier();

                    if (lineBuildMethod.Groups[1].Value != string.Empty)
                        temp.dialogueModifier.charactersPerCycle = int.Parse(lineBuildMethod.Groups[1].Value);

                    if (lineBuildMethod.Groups[2].Value != string.Empty)
                        temp.dialogueModifier.characterBuildSpeed = float.Parse(lineBuildMethod.Groups[2].Value);

                    if (lineBuildMethod.Groups[3].Value != string.Empty)
                        temp.dialogueModifier.cantBeInterupted = bool.Parse(lineBuildMethod.Groups[3].Value);

                    if (lineBuildMethod.Groups[4].Value != string.Empty)
                        temp.dialogueModifier.waitForUserInput = bool.Parse(lineBuildMethod.Groups[4].Value);
                }

                /*
                 * By default, a dialogue line will always have a option to change how it is built to the screen as a whole. the default is typewriter. 
                 */

                switch (lineBuildMethod.Groups[5].Value)
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

                /*
                 * This is the optional part of the dialogue line commands and choices
                 * 
                 * You can only have either commands or choices NOT BOTH (might be changed later)
                 * 
                 * commands are called using a key value to querey a dictionary, you can have as many commands as you want
                 * 
                 * choices are limited to two choices right now and have a index value to skip to a line that is specified in the choice
                 * 
                 * Dialogue right now follows a linear path, will maybe add a way to make it to where dialogue can trigger other dialogue conversations.
                 */

                if (matches.Groups[5].Value != string.Empty) //Commands in string
                {
                    dialogueConversation.Add(new DialogueLine(name, temp, matches.Groups[5].Value.Split(" ")));
                }
                else if (matches.Groups[4].Value != string.Empty) //Dialogue choice in string
                {
                    MatchCollection buttonChoices = dialogueChoices.Matches(matches.Groups[4].Value);

                    foreach (Match match in buttonChoices)
                    {
                        if (match.Groups[2].Value != string.Empty)
                        {
                            choices.Add(new Choice(match.Groups[1].Value, int.Parse(match.Groups[2].Value)));
                        }
                        else if (match.Groups[3].Value != string.Empty)
                        {
                            //Since we cant have an array of strings in a dialogue line. use the new line character for branching dialogue to split lines into indicidual dialogue lines.
                            //Think about this more, is it possible with my current dialogue system to have branching dialogue.
                            choices.Add(new Choice(match.Groups[1].Value, ParseConversation(match.Groups[3].Value.Split("\n"))));
                        }
                    }

                    dialogueConversation.Add(new DialogueLine(name, temp, choices));
                }
                else //None present
                {
                    dialogueConversation.Add(new DialogueLine(name, temp));
                }
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
    public List<Choice> choices;
    public string[] command;

    public DialogueLine(string name, Dialogue dialogue)
    {
        this.name = name;
        this.dialogue = dialogue;
        choices = null;
        command = null;
    }

    public DialogueLine(string name, Dialogue dialogue, List<Choice> choices)
    {
        this.name = name;
        this.dialogue = dialogue;
        this.choices = choices;
        command = null;
    }

    public DialogueLine(string name, Dialogue dialogue, string[] command)
    {
        this.name = name;
        this.dialogue = dialogue;
        choices = null;
        this.command = command;
    }

    public override string ToString()
    {
        return "Speaker: " + name + ", " + dialogue + ", DialogueChoice: " + (choices != null ? "yes" : "no") + ", Command: " + (command == null ? "none" : "has commands");
    }
}

[System.Serializable]
public class Dialogue
{
    public string dialogue;
    public float delay;
    public StartSignal signal;
    public BuildMethod buildMethod;
    public DialogueModifier dialogueModifier = null;

    public enum BuildMethod { typeWriter, instant };

    public enum StartSignal { NONE, A, C, WA, WC }

    public override string ToString()
    {
        return "Start Signal: " + signal + ", Delay: " + delay.ToString() + ", " + dialogue;
    }
}

public class Choice
{
    public string choiceText;
    public float delay;
    public string[] commands;

    public int choiceIndex = -1;
    public List<DialogueLine> dialogueResponse = null;
    public ChoiceType type;

    public enum ChoiceType { index, branching };

    public Choice(string choiceText, int choiceIndex)
    {
        this.choiceText = choiceText;
        this.choiceIndex = choiceIndex;
        type = ChoiceType.index;
    }

    public Choice(string choiceText, List<DialogueLine> dialogueResponse)
    {
        this.choiceText = choiceText;
        this.dialogueResponse = dialogueResponse;
        type = ChoiceType.branching;
    }

    public override string ToString()
    {
        return $"Choice Text: {choiceText}, Choice Type:{(type == ChoiceType.index ? "index" : "branching")}";
    }
}

public class DialogueModifier
{
    public int charactersPerCycle = 0;
    public float characterBuildSpeed = -1f;
    public bool cantBeInterupted = false;
    public bool waitForUserInput = true;
}