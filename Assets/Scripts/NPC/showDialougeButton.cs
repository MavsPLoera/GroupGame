using UnityEngine;
using UnityEngine.UI; 


public class ShowDialogueButton : MonoBehaviour
{
    public GameObject npc;
    public GameObject player;
    public float requiredDistance = 3f; 
    private float distanceFromNpc;
    public Button talkToNpcButton;
    private readonly bool _debug = false;

    void Awake()
    {
        if (talkToNpcButton == null)
        {
            talkToNpcButton = GetComponent<Button>();
            
            if (talkToNpcButton == null)
            {
                Debug.LogError("Talk button not found. Please assign a Button in the inspector.");
            }
        }
        UpdateDialogueButtonState();
    }
    
    void Update()
    {
        UpdateDialogueButtonState();
    }
    
    private void UpdateDialogueButtonState()
    {
        if (player == null || npc == null || talkToNpcButton == null)
            return;
            
        // Calculate distance between player and NPC
        distanceFromNpc = Vector3.Distance(player.transform.position, npc.transform.position);
        
        // Enable or disable the button based on distance
        if (distanceFromNpc <= requiredDistance)
        {
            talkToNpcButton.gameObject.SetActive(true);
            if(_debug) Debug.Log("Button should appear");

        }
        else
        {
            talkToNpcButton.gameObject.SetActive(false);
        }
    }
    
    public void OnTalkButtonClicked()
    {
        StartDialogue dialogueManager = npc.GetComponent<StartDialogue>();
        if (dialogueManager != null)
        {
            dialogueManager.BeginDialogue();
        }

    }
}