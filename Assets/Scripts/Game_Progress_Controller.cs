using UnityEngine;

public class Game_Progress_Controller : MonoBehaviour
{
    // Game Progress Controller
    // Handles calling appropriate cutscenes and setting player
    // vars. based on game progress. Saves stats. at start of each chapter.

    public static Game_Progress_Controller instance;

    private void Awake()
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

    void Start()
    {
        StartIntro();
    }

    void Update()
    {
        
    }

    public void StartIntro()
    {
        UI_Controller.instance.DisplayIntroCutscene();
    }

    public void StartCH1()
    {
        UI_Controller.instance.DisplayCH1Custscene();
        // Update player respawn location to The Pale Mare.
        Player_Controller.instance.respawnPosition.transform.position = new Vector3(-116.48f, 4.66f, 0);
    }

    public void StartCH2()
    {
        UI_Controller.instance.DisplayCH2Custscene();
        // Move player to The Pale Mare.
        Player_Controller.instance.transform.position = new Vector3(-126.5f, 4.5f, 0);
    }
}
