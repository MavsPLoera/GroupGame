using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dialogue_Controller : MonoBehaviour
{
    public GameObject DialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Button[] choiceButtons;

    public int charactersPerCycle = 5;
    public float speed = 3f;
    private int skipToLine = -1;
    public int characterSoundDelay;
    private int defaultCharactersPerCycle;
    private float defaultSpeed;
    public Coroutine buildingText = null;

    public bool isBuilding = false;
    public bool inConversation = false;
    public bool lineCanBeInterupted = true;
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

    public void setSkipToIndex(int index)
    {
        skipToLine = index;
    }

    public IEnumerator WaitForUserChoiceSelection()
    {
        while (buttonNotSelected)
            yield return null;

        GameObject temp = EventSystem.current.currentSelectedGameObject;

        setSkipToIndex(temp.GetComponent<ButtonChoice_Controller>().SkipToWhatLine);
    }

    public IEnumerator DialogueInteraction(List<DialogueLine> dialogue)
    {
        inConversation = true;
        DialogueBox.SetActive(true);
        DialogueLine line = null;
        Player_Controller.instance.canInput = false;

        for (int i = 0; i < dialogue.Count; i++)
        {
            line = dialogue[i];

            if (line.dialogue != null)
            {
                //Make this better
                if (line.dialogue.dialogueModifier != null)
                {
                    if (line.dialogue.dialogueModifier.charactersPerCycle != 0)
                        charactersPerCycle = line.dialogue.dialogueModifier.charactersPerCycle;

                    if (line.dialogue.dialogueModifier.characterBuildSpeed > 0.0f)
                        speed = line.dialogue.dialogueModifier.characterBuildSpeed;

                    lineCanBeInterupted = !line.dialogue.dialogueModifier.cantBeInterupted;
                    waitForUserInput = line.dialogue.dialogueModifier.waitForUserInput;
                }

                switch (line.dialogue.signal)
                {
                    case Dialogue.StartSignal.C:
                    case Dialogue.StartSignal.NONE:
                        dialogueText.text = line.dialogue.dialogue;
                        dialogueText.maxVisibleCharacters = 0;
                        break;
                    case Dialogue.StartSignal.A:
                        dialogueText.text += line.dialogue.dialogue;
                        break;
                    case Dialogue.StartSignal.WA:
                        yield return new WaitForSeconds(line.dialogue.delay);
                        dialogueText.text += line.dialogue.dialogue;
                        break;
                    case Dialogue.StartSignal.WC:
                        yield return new WaitForSeconds(line.dialogue.delay);
                        dialogueText.text = line.dialogue.dialogue;
                        dialogueText.maxVisibleCharacters = 0;
                        break;
                }
                dialogueText.ForceMeshUpdate();

                if (!isBuilding)
                {
                    if (line.dialogue.buildMethod == Dialogue.BuildMethod.typeWriter)
                    {
                        buildingText = StartCoroutine(BuildTextTypeWriter(line));
                    }
                    else
                    {
                        buildingText = StartCoroutine(BuildTextInstant(line));
                    }
                }

                yield return buildingText;

                if (waitForUserInput)
                {
                    yield return StartCoroutine(WaitForUserInput());
                }
                else if (line.choices != null)
                {
                    yield return StartCoroutine(WaitForUserChoiceSelection());

                    for (int j = 0; j < choiceButtons.Length; j++)
                    {
                        choiceButtons[j].gameObject.SetActive(false);
                    }

                    if (skipToLine >= dialogue.Count)
                    {
                        Debug.LogError("Skipping to line that is outside avalible dialogue lines");
                        i = dialogue.Count;
                    }
                    else
                    {
                        i = skipToLine - 1;
                        nameText.text = "";
                        dialogueText.text = "";
                    }
                }

                if (line.command != null)
                {
                    foreach (string dialogueCommand in line.command)
                    {
                        DialogueCommands_Controller.instance.CallCommand(dialogueCommand);
                    }
                }

                //Reset Dialogue Modifiers
                buttonNotSelected = true;
                lineCanBeInterupted = true;
                waitForUserInput = true;
                charactersPerCycle = defaultCharactersPerCycle;
                speed = defaultSpeed;
            }
        }
        DialogueBox.SetActive(false);
        Player_Controller.instance.canInput = true;
        inConversation = false;
        skipToLine = -1;
    }

    public void ForceComplete()
    {
        dialogueText.maxVisibleCharacters = dialogueText.text.Length;
        buildingText = null;
    }

    public IEnumerator BuildTextTypeWriter(DialogueLine dialougeLine)
    {
        isBuilding = true;

        nameText.text = dialougeLine.name;

        //Prevent String interupts by setting text to dialogue line once then letting player see the text
        while (dialogueText.maxVisibleCharacters < dialogueText.textInfo.characterCount)
        {
            dialogueText.maxVisibleCharacters += charactersPerCycle;

            if (dialogueText.maxVisibleCharacters % characterSoundDelay == 0)
                textAudioSource.PlayOneShot(textAudioClip);

            yield return new WaitForSeconds(.015f / speed);
        }

        if (dialougeLine.choices != null && !(dialougeLine.choices.Count() > choiceButtons.Length))
        {
            for (int i = 0; i < dialougeLine.choices.Count; i++)
            {
                choiceButtons[i].gameObject.SetActive(true);
                ButtonChoice_Controller temp = choiceButtons[i].GetComponent<ButtonChoice_Controller>();
                temp.SetButtonText(dialougeLine.choices[i].choiceText);
                temp.SetSkipToWhatLine(dialougeLine.choices[i].choiceIndex);
            }

            EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
        }
        //else
        //{
        //    Debug.LogError($"To many choices, Dialogue controller only has {choiceButtons.Length} buttons.");
        //}

        buildingText = null;
        isBuilding = false;

        yield return null;
    }

    public IEnumerator BuildTextInstant(DialogueLine dialougeLine)
    {
        isBuilding = true;

        nameText.text = dialougeLine.name;
        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;

        buildingText = null;
        isBuilding = false;
        yield return null;
    }
}
