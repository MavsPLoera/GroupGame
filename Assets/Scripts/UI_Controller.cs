using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static System.TimeZoneInfo;

public class UI_Controller : MonoBehaviour
{
    [Header("UI GameObjects.")]
    public GameObject playerUI;
    public GameObject gameoverUI;
    public GameObject gamewinUI;
    public GameObject pauseMenuUI;
    public GameObject cutsceneUI;

    //[Header("Player UI Objects.")]
    //Add things like buttons, text, etc here to change it

    //[Header("GameOver UI Objects.")]
    //Add things like buttons, text, etc here to change it

    //[Header("GameWin UI Objects.")]
    //Add things like buttons, text, etc here to change it

    //[Header("PauseMenu UI Objects.")]
    //Add things like buttons, text, etc here to change it

    [Header("CutScene UI Objects")]
    public TextMeshProUGUI cutsceneDisplayText;
    public GameObject cutsceneContinue;
    public GameObject introCutsceneImage;
    public string introCutsceneText;
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
        DisplayIntroCutscene();
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

    public void DisplayIntroCutscene()
    {
        playerUI.SetActive(false);
        cutsceneUI.SetActive(true);
        cutsceneContinue.SetActive(false);
        Player_Controller.instance.canInput = false;
        cutsceneDisplayText.text = "";
        StartCoroutine(Cutscene(cutsceneDisplayText, introCutsceneText));
    }

    private IEnumerator DisplayPopupText(string text, TextMeshProUGUI displayText)
    {
        // TODO: pass and play AudioClip.
        displayText.text = text;
        yield return StartCoroutine(FadeText(displayText, 0, 1, 1f));
        yield return new WaitForSeconds(textDisplayDuration);
        yield return StartCoroutine(FadeText(displayText, 1, 0, 1f));
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

    private IEnumerator Cutscene(TextMeshProUGUI displayText, string contentText)
    {
        yield return new WaitForSeconds(1);
        for(int i = 0; i < contentText.Length; i++)
        {
            if(cutsceneSkip)
            {
                displayText.text = contentText;
                break;
            }
            displayText.text += contentText[i];
            yield return new WaitForSeconds(cutsceneTextDuration);
        }
        // yield return new WaitForSeconds(cutsceneDuration);
        yield return new WaitForSeconds(0.2f);
        cutsceneContinue.SetActive(true);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        cutsceneContinue.SetActive(false);
        cutsceneUI.SetActive(false);
        playerUI.SetActive(true);
        Player_Controller.instance.canInput = true;
    }
}
