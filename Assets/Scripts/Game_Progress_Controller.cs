using Unity.VisualScripting;
using UnityEngine;

public class Game_Progress_Controller : MonoBehaviour
{
    // Game Progress Controller
    // Handles calling appropriate cutscenes and setting player
    // vars. based on game progress. Saves stats. at start of each chapter.

    [Header("Saved Player Stats")]
    public int savedHealingPotions;
    public int savedArrows;
    public float savedGold;
    public Vector3 savedRespawn;
    public int chapterIdx = 0;

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
        savedRespawn = Player_Controller.instance.respawnPosition.transform.position;
        StartIntro();
        Player_Controller.instance.quests
            .Add(new Quest("Make Contact", "Make haste to The Pale Mare in Saltmourne. Your contact is waiting there for you.", "???"));
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
        Player_Controller.instance.respawnPosition.transform.position = savedRespawn = new Vector3(-116.48f, 4.66f, 0);
        SavePlayerProgress();
        chapterIdx = 1;
    }

    public void StartCH2()
    {
        UI_Controller.instance.DisplayCH2Custscene();
        // Move player to The Pale Mare.
        Dungeon_Controller.instance.inDungeon = false;
        Camera_Controller.instance.inDungeon = false;
        Player_Controller.instance.transform.position = new Vector3(-126.5f, 4.5f, 0);
        SavePlayerProgress();
        // Unlock new area.
        AreaLock_Controller.instance.unlockSecondaryNeededAreas();
        chapterIdx = 2;
    }

    public void StartCH3()
    {
        UI_Controller.instance.DisplayCH3Custscene();
        // Move player to The Pale Mare.
        Dungeon_Controller.instance.inDungeon = false;
        Camera_Controller.instance.inDungeon = false;
        Player_Controller.instance.transform.position = new Vector3(-126.5f, 4.5f, 0);
        SavePlayerProgress();
        // Unlock new area.
        AreaLock_Controller.instance.unlockUltimateNeededAreas();
        chapterIdx = 3;
    }

    public void ResetToLastCheckpoint()
    {
        // Called from GameOver Panel to reset player's progress
        // to last saved state.
        Player_Controller.instance.healingPotions = savedHealingPotions;
        Player_Controller.instance.arrows = savedArrows;
        Player_Controller.instance.gold = savedGold;
        Player_Controller.instance.respawnPosition.transform.position  = Player_Controller.instance.transform.position = savedRespawn;
        Player_Controller.instance.playerHealth = 100;
        Player_Controller.instance.playerLives = 3;
        Player_Controller.instance.isPaused = false;
        Player_Controller.instance.isTransitioning = false;
        Dungeon_Controller.instance.inDungeon = false;
        Camera_Controller.instance.inDungeon = false;
        Player_Controller.instance.gameObject.SetActive(true);
        UI_Controller.instance.gameoverUI.SetActive(false);
        switch (chapterIdx)
        {
            case 0:
                StartIntro();
                break;
            case 1:
                StartCH1();
                break;
            case 2:
                StartCH2();
                break;
            default:
                break;
        }
    }

    private void SavePlayerProgress()
    {
        savedHealingPotions = Player_Controller.instance.healingPotions;
        savedArrows = Player_Controller.instance.arrows;
        savedGold = Player_Controller.instance.gold;
    }
}
