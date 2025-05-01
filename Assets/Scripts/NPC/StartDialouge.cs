using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartDialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI questAcceptDisplay;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;
    [SerializeField] private bool isQuest; // For non-quest dialogue.
    [SerializeField] private Canvas canvas;
    public string[] questTitles;
    public string[] questDescriptions;
    private bool questDecision = false;
    private int questIdx = 0;

    [Header("Dialogue Content")]
    [SerializeField] private string[] dialogueSlides;
    [SerializeField] private string npcName;
    private int currentSlideIndex = 0;

    private void Start()
    {
        canvas.worldCamera = Camera.main;
    }

    public void BeginDialogue()
    {
        // Show the dialogue panel
        dialoguePanel.SetActive(true);
        questAcceptDisplay.gameObject.SetActive(false);

        npcNameText.text = npcName;
        
        // Reset to the first slide
        currentSlideIndex = 0;
        DisplayCurrentSlide();
    }

    private void DisplayCurrentSlide()
    {
        instructionText.text = dialogueSlides[currentSlideIndex];

        // Check if we're on the last slide to show quest decision buttons
        if (currentSlideIndex == dialogueSlides.Length - 1)
        {
            nextButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(false);

            if (isQuest)
            {
                if (questDecision == false)
                {
                    acceptButton.gameObject.SetActive(true);
                    rejectButton.gameObject.SetActive(true);

                }
                else
                {
                    questAcceptDisplay.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            nextButton.gameObject.SetActive(true);
            acceptButton.gameObject.SetActive(false);
            rejectButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(true);
        }
    }

    public void ShowNextSlide()
    {
        if (currentSlideIndex < dialogueSlides.Length - 1)
        {
            currentSlideIndex++;
            DisplayCurrentSlide();
        }
    }

    public void SkipToEnd()
    {
        currentSlideIndex = dialogueSlides.Length - 1;
        DisplayCurrentSlide();
    }

    public void AcceptQuest()
    {
        Debug.Log("Quest accepted!");
        UI_Controller.instance.PopupText("Quest accepted");
        Player_Controller.instance.quests.Add(new Quest(questTitles[questIdx], questDescriptions[questIdx], false));
        // Bad choice to do it this way, but I'm tired boss. On accept of new quest, complete previous.
        Player_Controller.instance.quests[questIdx].isComplete = true;
        if(questIdx == 0) AreaLock_Controller.instance.unlockSewers(); // Refactor.
        questIdx++;
        UI_Controller.instance.ActiveQuest();
        questDecision = true;
        CloseDialogue();
    }

    public void RejectQuest()
    {
        Debug.Log("Quest rejected!");
        UI_Controller.instance.PopupText("Quest rejected");
        questDecision = false;
        CloseDialogue();
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
