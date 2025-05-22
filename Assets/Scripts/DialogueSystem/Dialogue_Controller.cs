using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEditor.Rendering;
using UnityEngine;

public class Dialogue_Controller : MonoBehaviour
{
    public GameObject DialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public int charactersPerCycle = 5;
    public float speed = 3f;
    private int defaultCharactersPerCycle;
    private float defaultSpeed;
    public Coroutine buildingText = null;

    public bool isBuilding = false;
    public bool inConversation = false;
    public bool lineCanBeInterupted = true;

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

    // Update is called once per frame
    void Update()
    {
        //Could make this a event in order to make this more optimized. Alot of the functionality of our game can be transformed into events to allow for more modulation in our code.
        if (inConversation)
        {
            Player_Controller.instance.canInput = false;
        }
        else
        {
            Player_Controller.instance.canInput = true;
        }
    }

    public IEnumerator WaitForUserInput()
    {
        while(!Input.GetKeyDown(KeyCode.R))
            yield return null;
    }

    public IEnumerator DialogueInteraction(List<DialogueLine> dialogue)
    {
        inConversation = true;
        DialogueBox.SetActive(true);
        foreach (DialogueLine line in dialogue)
        {
            if(line.dialogue != null)
            {
                //Modify for clearing and appendeding
                //Also for adding a command to change the build type.
                //Also to change text color for certain words

                //Make this better
                if(line.dialogue.dialogueModifier != null)
                {
                    if (line.dialogue.dialogueModifier.charactersPerCycle != -1)
                        charactersPerCycle = line.dialogue.dialogueModifier.charactersPerCycle;
                    if (line.dialogue.dialogueModifier.characterBuildSpeed > 0.0f)
                        speed = line.dialogue.dialogueModifier.characterBuildSpeed;
                    lineCanBeInterupted = !line.dialogue.dialogueModifier.cantBeInterupted;
                }


                switch (line.dialogue.signal)
                {
                    case Dialogue.StartSignal.C: case Dialogue.StartSignal.NONE:
                        dialogueText.text = line.dialogue.dialogue;
                        nameText.text = line.name;
                        dialogueText.maxVisibleCharacters = 0;
                        break;
                    case Dialogue.StartSignal.A:
                        dialogueText.text += line.dialogue.dialogue;
                        nameText.text = line.name;
                        break;
                    case Dialogue.StartSignal.WA:
                        yield return new WaitForSeconds(line.dialogue.delay);
                        dialogueText.text += line.dialogue.dialogue;
                        nameText.text = line.name;
                        break;
                    case Dialogue.StartSignal.WC:
                        yield return new WaitForSeconds(line.dialogue.delay);
                        dialogueText.text = line.dialogue.dialogue;
                        nameText.text = line.name;
                        dialogueText.maxVisibleCharacters = 0;
                        break;
                }
                dialogueText.ForceMeshUpdate();

                if (!isBuilding)
                {
                    if(line.dialogue.buildMethod == Dialogue.BuildMethod.typeWriter)
                    {
                        buildingText = StartCoroutine(BuildTextTypeWriter(line));
                    }
                    else
                    {
                        buildingText = StartCoroutine(BuildTextInstant(line));
                    }
                }

                yield return buildingText;

                yield return StartCoroutine(WaitForUserInput());

                lineCanBeInterupted = true;
                charactersPerCycle = defaultCharactersPerCycle;
                speed = defaultSpeed;
            }
        }
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

        //dialogueText.text = dialougeLine.dialogue.dialogue;
        //nameText.text = dialougeLine.name; 
        //dialogueText.maxVisibleCharacters = 0;
        //dialogueText.ForceMeshUpdate();

        //Prevent String interupts by setting text to dialogue line once then letting player see the text
        while (dialogueText.maxVisibleCharacters < dialogueText.textInfo.characterCount)
        {
            dialogueText.maxVisibleCharacters += charactersPerCycle;
            textAudioSource.PlayOneShot(textAudioSource.clip);

            yield return new WaitForSeconds(.015f / speed);
        }

        buildingText = null;
        isBuilding = false;

        yield return null;
    }

    public IEnumerator BuildTextInstant(DialogueLine dialougeLine)
    {
        isBuilding = true;

        //dialogueText.text = dialougeLine.dialogue.dialogue;
        //nameText.text = dialougeLine.name;
        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;
        //dialogueText.ForceMeshUpdate();

        buildingText = null;
        isBuilding = false;
        yield return null;
    }
}
