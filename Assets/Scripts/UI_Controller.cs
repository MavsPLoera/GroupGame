using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static System.TimeZoneInfo;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    [Header("UI GameObjects.")]
    public GameObject playerUI;
    public GameObject gameoverUI;
    public GameObject gamewinUI;
    public GameObject pauseMenuUI;
    public GameObject cutsceneUI;

    [Header("Player UI Objects.")]
    public TextMeshProUGUI CoinCountText;
    public TextMeshProUGUI HealthPotionsText;
    public TextMeshProUGUI ArrowText;
    public TextMeshProUGUI PlayerLivesText;
    public TextMeshProUGUI currentQuestTitle;
    public TextMeshProUGUI currentQuestStatus;
    public Image ArrowImage;

    //[Header("GameOver UI Objects.")]
    //Add things like buttons, text, etc here to change it

    //[Header("GameWin UI Objects.")]
    //Add things like buttons, text, etc here to change it

    [Header("PauseMenu UI Objects.")]
    public TextMeshProUGUI QuestMenuTitleText;
    public TextMeshProUGUI IndexText;
    public TextMeshProUGUI QuestTitleText;
    public TextMeshProUGUI QuestDescriptionText;
    public TextMeshProUGUI QuestStatusText;
    public TextMeshProUGUI NoQuestsText;
    public Button indexRightButton;
    public Button indexLeftButton;
    public int questIndex = 0;
    //Add things like buttons, text, etc here to change it

    [Header("CutScene UI Objects")]
    public TextMeshProUGUI cutsceneDisplayText;
    public TextMeshProUGUI cutsceneSubtitleText;
    public TextMeshProUGUI cutsceneTitleText;
    public GameObject cutsceneContinue;
    public List<string> cutsceneTexts;
    public List<string> cutsceneSubtitles;
    public List<GameObject> cutsceneImages;
    // public string introCutsceneText;
    public float cutsceneDuration;
    public float cutsceneTextDuration;
    private bool cutsceneSkip = false;

    [Header("UI Controller Misc.")]
    public TextMeshProUGUI currentLocationText;
    public TextMeshProUGUI locationDiscoveredText;
    public TextMeshProUGUI dungeonClearedText;
    public TextMeshProUGUI hintText;
    public float textDisplayDuration;
    public GameObject crossFadeIn;
    public GameObject crossFadeOut;
    private AudioClip lastPlayedSong; //NO TOUCHIE

    public static UI_Controller instance;

    private void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Dungeon_Controller.instance.inDungeon && Dungeon_Controller.instance.currentDungeon != null)
        {
            bool isCleared = Dungeon_Controller.instance.currentDungeon.isCleared;
            dungeonClearedText.text = isCleared ? "(Cleared)" : "(Not Cleared)";
        }
        else
        {
            dungeonClearedText.text = "";
        }

        if(cutsceneUI.activeSelf && !cutsceneSkip && Input.GetMouseButtonDown(0))
        {
            cutsceneSkip = true;
        }
    }

    private void Start()
    {
        CollectCoin();
        CollectHealth();
        UpdatePlayerLives();
        ActiveQuest();
    }

    public void EnterArea(string name)
    {
        currentLocationText.text = name;
        StartCoroutine(FadeText(currentLocationText, 0, 1, 1f));
    }

    public void ExitArea(string name)
    {
        StartCoroutine(FadeText(currentLocationText, 1, 0, 1f));
    }

    public void DiscoverLocation(string name)
    {
        name = $"Discovered {name}";
        StartCoroutine(DisplayPopupText(name, locationDiscoveredText));
    }

    public void PopupText(string text)
    {
        StartCoroutine(DisplayPopupText(text, hintText));
    }

    public void CollectCoin()
    {
        CoinCountText.text = Player_Controller.instance.gold.ToString();
    }   
    
    public void CollectHealth()
    {
        HealthPotionsText.text = Player_Controller.instance.healingPotions.ToString() + " / " + Player_Controller.instance.maxHealthPotions.ToString();
    }

    public void ActiveQuest()
    {
        if(Player_Controller.instance.quests.Count != 0)
        {
            currentQuestTitle.text = Player_Controller.instance.quests[questIndex].questTitle;
            currentQuestStatus.text = Player_Controller.instance.quests[questIndex].isComplete ? "Complete" : "Incomplete";
        }
        else
        {
            currentQuestTitle.text = "No active quest";
            currentQuestStatus.text = "";
        }
            
    }

    public void ShootArrow()
    {
        ArrowText.text = Player_Controller.instance.arrows.ToString() + " / " + Player_Controller.instance.maxArrows.ToString();
    }

    public void UpdatePlayerLives()
    {
        PlayerLivesText.text = "Lives " + Player_Controller.instance.playerLives.ToString();
    }

    public void GameOver()
    {
        playerUI.SetActive(false);

        //Add more here in order to change the text


        gameoverUI.SetActive(true);
    }

    public void GameWin()
    {
        playerUI.SetActive(false);

        //Add more here in order to change the text

        gamewinUI.SetActive(true);
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        playerUI.SetActive(false);
        lastPlayedSong = Music_Controller.instance.pauseMusic();
        Time.timeScale = 0;
        QuestMenuTitleText.gameObject.SetActive(true);
        if(Player_Controller.instance.quests.Count != 0)
        {
            IndexText.gameObject.SetActive(true);
            QuestTitleText.gameObject.SetActive(true);
            QuestDescriptionText.gameObject.SetActive(true);
            QuestStatusText.gameObject.SetActive(true);
            indexRightButton.gameObject.SetActive(true);
            indexLeftButton.gameObject.SetActive(true);
            NoQuestsText.gameObject.SetActive(false);

            IndexText.text = $"{questIndex + 1} / {Player_Controller.instance.quests.Count}";
            QuestTitleText.text = Player_Controller.instance.quests[questIndex].questTitle;
            QuestDescriptionText.text = Player_Controller.instance.quests[questIndex].questDescription;
            QuestStatusText.text = Player_Controller.instance.quests[questIndex].isComplete ? "Complete" : "Incomplete";
        }
        else
        {
            IndexText.gameObject.SetActive(false);
            QuestTitleText.gameObject.SetActive(false);
            QuestDescriptionText.gameObject.SetActive(false);
            QuestStatusText.gameObject.SetActive(false);
            indexRightButton.gameObject.SetActive(false);
            indexLeftButton.gameObject.SetActive(false);
            NoQuestsText.text = "No active quests";
            NoQuestsText.gameObject.SetActive(true);
        }

        Player_Controller.instance.canInput = false;
        Player_Controller.instance.isPaused = true;
    }

    public void indexQuestRight()
    {
        if(!(questIndex + 1 > Player_Controller.instance.quests.Count - 1))
        {
            questIndex++;
            IndexText.text = $"{questIndex + 1} / {Player_Controller.instance.quests.Count}";
            QuestTitleText.text = Player_Controller.instance.quests[questIndex].questTitle;
            QuestDescriptionText.text = Player_Controller.instance.quests[questIndex].questDescription;
            currentQuestStatus.text = QuestStatusText.text = Player_Controller.instance.quests[questIndex].isComplete ? "Complete" : "Incomplete";
        }
    }

    public void indexQuestLeft()
    {
        if(!(questIndex - 1 < 0))
        {
            questIndex--;
            IndexText.text = $"{questIndex + 1} / {Player_Controller.instance.quests.Count}";
            QuestTitleText.text = Player_Controller.instance.quests[questIndex].questTitle;
            QuestDescriptionText.text = Player_Controller.instance.quests[questIndex].questDescription;
            currentQuestStatus.text = QuestStatusText.text = Player_Controller.instance.quests[questIndex].isComplete ? "Complete" : "Incomplete";
        }
    }

    public void UnpauseGame()
    {
        IndexText.gameObject.SetActive(false);
        QuestTitleText.gameObject.SetActive(false);
        QuestDescriptionText.gameObject.SetActive(false);
        QuestStatusText.gameObject.SetActive(false);
        indexRightButton.gameObject.SetActive(false);
        indexLeftButton.gameObject.SetActive(false);
        NoQuestsText.gameObject.SetActive(false);
        pauseMenuUI.SetActive(false);
        playerUI.SetActive(true);
        Music_Controller.instance.resumeMusic(lastPlayedSong);

        Time.timeScale = 1;

        ActiveQuest();
        Player_Controller.instance.canInput = true;
        Player_Controller.instance.isPaused = false;
    }

    public void DisplayIntroCutscene()
    {
        cutsceneTitleText.text = "Prologue";
        StartCoroutine(Cutscene(0));
    }

    public void DisplayCH1Custscene()
    {
        cutsceneTitleText.text = "Chapter 01";
        StartCoroutine(Cutscene(1));
    }

    public void DisplayCH2Custscene()
    {
        cutsceneTitleText.text = "Chapter 02";
        StartCoroutine(Cutscene(2));
    }

    public void DisplayCH3Custscene()
    {
        cutsceneTitleText.text = "Chapter 03";
        StartCoroutine(Cutscene(3));
    }

    private IEnumerator DisplayPopupText(string text, TextMeshProUGUI displayText)
    {
        // TODO: pass and play AudioClip.
        displayText.text = text;
        yield return StartCoroutine(FadeText(displayText, 0, 1, 1));
        yield return new WaitForSeconds(textDisplayDuration);
        yield return StartCoroutine(FadeText(displayText, 1, 0, 1));
        displayText.text = "";
    }

    private IEnumerator FadeText(TextMeshProUGUI textToFade, float currentAlpha, float targetAlpha, float transitionTime)
    {
        Color originalColor = textToFade.color;
        textToFade.alpha = currentAlpha;
        float time = 0;
        while(time < transitionTime)
        {
            time += Time.deltaTime;
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, time / transitionTime);
            textToFade.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);
            yield return null;
        }
        textToFade.color = new Color(originalColor.r, originalColor.g, originalColor.b, targetAlpha);
    }

    private IEnumerator Cutscene(int idx)
    {
        playerUI.SetActive(false);
        cutsceneUI.SetActive(true);
        cutsceneContinue.SetActive(false);
        cutsceneSkip = false;
        Player_Controller.instance.canInput = false;
        Player_Controller.instance.isTransitioning = true;
        cutsceneDisplayText.text = "";
        string contentText = cutsceneTexts[idx];
        cutsceneImages[idx].SetActive(true);
        cutsceneSubtitleText.text = cutsceneSubtitles[idx];
        yield return new WaitForSeconds(1);
        for(int i = 0; i < contentText.Length; i++)
        {
            if(cutsceneSkip)
            {
                cutsceneDisplayText.text = contentText;
                break;
            }
            cutsceneDisplayText.text += contentText[i];
            yield return new WaitForSeconds(cutsceneTextDuration);
        }
        // yield return new WaitForSeconds(cutsceneDuration);
        yield return new WaitForSeconds(0.4f);
        cutsceneContinue.SetActive(true);
        TextMeshProUGUI continueText = cutsceneContinue.GetComponentInChildren<TextMeshProUGUI>();
        continueText.alpha = 0f;
        StartCoroutine(FadeText(continueText, 0, 1, 1));
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        cutsceneContinue.SetActive(false);
        cutsceneImages[idx].SetActive(false);
        cutsceneUI.SetActive(false);
        playerUI.SetActive(true);
        Player_Controller.instance.canInput = true;
        Player_Controller.instance.isTransitioning = false;
    }
}
