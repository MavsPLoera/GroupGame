using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static DialogueControllerOverride;

public class Dialogue_Controller : MonoBehaviour
{
    public GameObject DialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Button[] choiceButtons;

    public int charactersPerCycle = 5;
    public float speed = 3f;
    public int characterSoundDelay;
    private int defaultCharactersPerCycle;
    private float defaultSpeed;
    public Coroutine buildingText = null;

    public BuildMode mode;
    public bool isBuilding = false;
    public bool inConversation = false;
    public bool lineCantBeInterupted = false;
    public bool waitForUserInput = true;
    public bool buttonNotSelected = true;

    public AudioSource textAudioSource;
    public AudioClip textAudioClip;

    public static Dialogue_Controller instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        defaultCharactersPerCycle = charactersPerCycle;
        defaultSpeed = speed;
    }

    public IEnumerator WaitForUserInput()
    {
        //Keep looping until either R or Right GamePad is pressed.
        while ((!Input.GetButtonDown("Fire3") && !Input.GetKeyDown(KeyCode.R)))
            yield return null;
    }

    public void ButtonPressed()
    {
        buttonNotSelected = false;
    }

    public IEnumerator WaitForUserChoiceSelection(List<DialogueLine> currentList, int currentLineIndex)
    {
        while (buttonNotSelected)
            yield return null;

        GameObject temp = EventSystem.current.currentSelectedGameObject;

        //Update list with new responses from choice
        List<DialogueLine> response = temp.GetComponent<ButtonChoice_Controller>().response;

        if (response != null)
        {
            int addingLineIndex = currentLineIndex + 1;

            foreach (DialogueLine line in response)
            {
                currentList.Insert(addingLineIndex++, line);
            }
        }
    }

    //resets controller values back to default
    public void resetValues()
    {
        mode = BuildMode.TYPEWRITER;
        buttonNotSelected = true;
        lineCantBeInterupted = false;
        waitForUserInput = true;
        charactersPerCycle = defaultCharactersPerCycle;
        speed = defaultSpeed;
    }

    public IEnumerator DialogueInteraction(List<DialogueLine> dialogue)
    {
        inConversation = true;
        DialogueBox.SetActive(true);

        //Loop through all the lines passed in from the interaction. If dialogue choices are present we will add the response to the list at the current index.
        for (int i = 0; i < dialogue.Count; i++)
        {
            DialogueLine line = dialogue[i];

            //Check if the dialogue line wants to override the controller defaults, if the line has an override change the values.
            if (line.controllerOverride != null)
            {
                if (line.controllerOverride.charactersPerCycle != -1)
                    charactersPerCycle = line.controllerOverride.charactersPerCycle;

                if (line.controllerOverride.speed != -1f)
                    speed = line.controllerOverride.speed;

                lineCantBeInterupted = line.controllerOverride.cantBeInterrupted;
                waitForUserInput = line.controllerOverride.waitForUserInput;
                mode = line.controllerOverride.mode;
            }

            //Prep dialogue text for the type of start signal the line has provided.
            switch (line.dialogueStartSignal)
            {
                case DialogueLine.StartSignal.C:
                case DialogueLine.StartSignal.NONE:
                    dialogueText.text = line.dialogue;
                    dialogueText.maxVisibleCharacters = 0;
                    break;
                case DialogueLine.StartSignal.A:
                    dialogueText.text += line.dialogue;
                    break;
                case DialogueLine.StartSignal.WA:
                    yield return new WaitForSeconds(line.startSignalDelay);
                    dialogueText.text += line.dialogue;
                    break;
                case DialogueLine.StartSignal.WC:
                    yield return new WaitForSeconds(line.startSignalDelay);
                    dialogueText.text = line.dialogue;
                    dialogueText.maxVisibleCharacters = 0;
                    break;
            }
            dialogueText.ForceMeshUpdate();

            //Depending on the BuildMode set, callthe respective function.
            if (mode == BuildMode.TYPEWRITER)
            {
                buildingText = StartCoroutine(BuildTextTypeWriter(line));
            }
            else
            {
                buildingText = StartCoroutine(BuildTextInstant(line));
            }

            //Wait for line to finish building to screen.
            yield return buildingText;

            /*
             * First case, just a regular dialogue line that waits for user input.
             * 
             * Second case, we have dialogue choice and want to wait for user to respond to choice instead.
             */
            if (waitForUserInput && line.dialogueChoices == null)
            {
                yield return StartCoroutine(WaitForUserInput());
            }
            else if (line.dialogueChoices != null)
            {
                yield return StartCoroutine(WaitForUserChoiceSelection(dialogue, i));

                for (int j = 0; j < choiceButtons.Length; j++)
                {
                    choiceButtons[j].gameObject.SetActive(false);
                }
            }

            //Reset values back to normal if they were modified.
            resetValues();
        }

        //Turn off dialogue box
        DialogueBox.SetActive(false);
        inConversation = false;
    }

    public void ForceComplete()
    {
        dialogueText.maxVisibleCharacters = dialogueText.text.Length;
        buildingText = null;
    }

    public IEnumerator BuildTextTypeWriter(DialogueLine dialougeLine)
    {
        isBuilding = true;

        if(dialougeLine.speakerName == "narrator")
        {
            nameText.text = "";
        } 
        else
        {
            nameText.text = dialougeLine.speakerName;
        }
        

        //[TO DO] Play commands here

        //Prevent String interupts by setting text to dialogue line once then letting player see the text
        while (dialogueText.maxVisibleCharacters < dialogueText.textInfo.characterCount)
        {
            dialogueText.maxVisibleCharacters += charactersPerCycle;

            if (dialogueText.maxVisibleCharacters % characterSoundDelay == 0)
                textAudioSource.PlayOneShot(textAudioSource.clip);

            yield return new WaitForSeconds(.015f / speed);
        }

        if (dialougeLine.dialogueChoices != null && !(dialougeLine.dialogueChoices.Count() > choiceButtons.Length))
        {
            for (int i = 0; i < dialougeLine.dialogueChoices.Count(); i++)
            {
                choiceButtons[i].gameObject.SetActive(true);
                ButtonChoice_Controller temp = choiceButtons[i].GetComponent<ButtonChoice_Controller>();
                temp.SetButtonText(dialougeLine.dialogueChoices[i].choiceText);
                temp.SetResponse(dialougeLine.dialogueChoices[i].responseToChoice);
            }

            EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
        }
        else if (dialougeLine.dialogueChoices != null && (dialougeLine.dialogueChoices.Count() > choiceButtons.Length))
        {
            Debug.LogError($"To many choices, Dialogue controller only has {choiceButtons.Length} buttons.");
        }

        buildingText = null;
        isBuilding = false;

        yield return null;
    }

    public IEnumerator BuildTextInstant(DialogueLine dialougeLine)
    {
        isBuilding = true;

        if (dialougeLine.speakerName == "narrator")
        {
            nameText.text = "";
        }
        else
        {
            nameText.text = dialougeLine.speakerName;
        }

        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;

        //[TO DO] Play commands here

        if (dialougeLine.dialogueChoices != null && !(dialougeLine.dialogueChoices.Count() > choiceButtons.Length))
        {
            for (int i = 0; i < dialougeLine.dialogueChoices.Count(); i++)
            {
                choiceButtons[i].gameObject.SetActive(true);
                ButtonChoice_Controller temp = choiceButtons[i].GetComponent<ButtonChoice_Controller>();
                temp.SetButtonText(dialougeLine.dialogueChoices[i].choiceText);
                temp.SetResponse(dialougeLine.dialogueChoices[i].responseToChoice);
            }

            EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
        }
        else if (dialougeLine.dialogueChoices != null && (dialougeLine.dialogueChoices.Count() > choiceButtons.Length))
        {
            Debug.LogError($"To many choices, Dialogue controller only has {choiceButtons.Length} buttons.");
        }

        buildingText = null;
        isBuilding = false;
        yield return null;
    }
}
