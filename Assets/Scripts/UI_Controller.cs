using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.Audio;
using NUnit.Framework.Constraints;
using System;

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
    public GameObject firstButtonInPauseMenu;
    public TextMeshProUGUI QuestMenuTitleText;
    public TextMeshProUGUI IndexText;
    public TextMeshProUGUI QuestTitleText;
    public TextMeshProUGUI QuestDescriptionText;
    public TextMeshProUGUI QuestStatusText;
    public TextMeshProUGUI NoQuestsText;
    public Button indexRightButton;
    public Button indexLeftButton;
    public int questIndex = 0;
    public bool fullscreenOn = true;
    public TextMeshProUGUI resolutionText;
    public int resolutionIndex = 0;
    public List<Resolution> resolutions = new List<Resolution>();
    public Camera mainCamera;
    public PixelPerfectCamera pxielPerfectCamera;
    //Add things like buttons, text, etc here to change it

    [Header("Options UI Objects.")]
    public GameObject firstButtonInOptionsMenu;
    public GameObject gameSettings;
    public GameObject resolutionButton;
    public Button closeButton;
    public Toggle lastGameObjectInGameSettings;
    public Button lastGameObjectInVideoSettings;
    public Slider lastGameObjectInAudioSettings;
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider sfxVolume;

    [Header("Inventory UI Objects.")]
    public GameObject InventoryPanel;
    public GameObject firstButtonInInventory;
    public InventoryButton[] inventoryButton;
    private int currentSelectedButtonIndex;
    private GameObject lastSelectedObject;
    public GameObject itemOoptionsUI;
    public GameObject itemOptionsFirstButton;
    public GameObject ItemUseOptionButtons;
    public GameObject CheckDecisionUI;
    public GameObject DropButton;
    public GameObject YesButton;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;

    [Header("Selectable Inventory UI Objects.")]
    public GameObject SelectableInventoryPanel;
    public GameObject firstButtonInSelectableInventory;
    public InventoryButton[] selectableInventoryButton;
    public InventoryButton[] selectableArrowInventoryButtons;
    public InventoryButton[] selectableAccessoryInventoryButtons;
    private bool waitingForSelection = true;
    //public TextMeshProUGUI itemNameText;
    //public TextMeshProUGUI itemDescriptionText;

    [Header("Found New Item UI Objects.")]
    public GameObject FoundNewItemPanel;
    public TextMeshProUGUI ItemNameText;
    public TextMeshProUGUI ItemDescriptionText;
    public Image ItemImage;


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
    public AudioMixer audioMixer;
    public AudioClip textSFX;
    public AudioSource UIAudioSource;
    private AudioClip lastPlayedSong; //NO TOUCHIE

    [Header("UI Events")]
    public static Action OpenInventory;
    public static Action<int> equipItem;
    public static Action<int> dropItem;
    public static Func<int, string> inspectItem;
    public static Action UpdateInvetorySlot;
    public static Action UpdateInventory;



    public static UI_Controller instance;

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

        pxielPerfectCamera = mainCamera.GetComponent<PixelPerfectCamera>();
        resolutionText.text = resolutions[resolutionIndex].ToString();
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, fullscreenOn);
    }

    private void Update()
    {
        if (Dungeon_Controller.instance.inDungeon && Dungeon_Controller.instance.currentDungeon != null)
        {
            bool isCleared = Dungeon_Controller.instance.currentDungeon.isCleared;
            dungeonClearedText.text = isCleared ? "(Cleared)" : "(Not Cleared)";
        }
        else
        {
            dungeonClearedText.text = "";
        }

        if (cutsceneUI.activeSelf && !cutsceneSkip && Input.GetMouseButtonDown(0))
        {
            cutsceneSkip = true;
        }
    }

    private void Start()
    {
        CollectCoin();
        CollectHealth();
        UpdatePlayerLives();
        //ActiveQuest();

        changeMasterVolume();
        changeMusicVolume();
        changeSFXVolume();
    }

    public void OnEnable()
    {
        
    }

    public void OnDisable()
    {
        
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
        EventSystem.current.SetSelectedGameObject(firstButtonInPauseMenu);

        Time.timeScale = 0;
        

        Player_Controller.instance.canInput = false;
        Player_Controller.instance.isPaused = true;
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

        Player_Controller.instance.canInput = true;
        Player_Controller.instance.isPaused = false;
    }

    #region inventory
    public void openInventory()
    {
        InventoryPanel.gameObject.SetActive(true);

        updateInventory();

        EventSystem.current.SetSelectedGameObject(firstButtonInInventory);
    }

    public void openItemUseOptions()
    {
        itemOoptionsUI.SetActive(true);
        lastSelectedObject = EventSystem.current.currentSelectedGameObject;
        currentSelectedButtonIndex = int.Parse(lastSelectedObject.name);
        EventSystem.current.SetSelectedGameObject(itemOptionsFirstButton);
    }

    public void closeItemUseOptions()
    {
        currentSelectedButtonIndex = -1;
        EventSystem.current.SetSelectedGameObject(lastSelectedObject);
        itemOoptionsUI.SetActive(false);
    }

    public void ChangeSelectedButtonItemUseText()
    {
        GameObject temp = EventSystem.current.currentSelectedGameObject;
        TextMeshProUGUI buttonText = temp.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = Color.white;
        buttonText.ForceMeshUpdate();
    }

    public void ChangeSelectedButtonItemUseTextBack()
    {
        GameObject temp = EventSystem.current.currentSelectedGameObject;
        TextMeshProUGUI buttonText = temp.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = new Color(0.4823529f, 0.4823529f, 0.4823529f, 1f);
        buttonText.ForceMeshUpdate();
    }

    public void updateInventory()
    {
        //Update inventory
        for (int i = 0; i < inventoryButton.Length; i++)
        {
            Image temp = inventoryButton[i].image;

            if (Player_Controller.instance.itemInSlot[i])
            {
                temp.sprite = Player_Controller.instance.playerInventory[i].itemImage;
                temp.color = new Color(1, 1, 1, 1);

                if (Player_Controller.instance.playerInventory[i].quantity > 1)
                {
                    updateItemQuantity(i);
                }
                else
                {
                    inventoryButton[i].quanitityText.text = "";
                }
            }
            else
            {
                temp.color = new Color(0, 0, 0, 1);
                inventoryButton[i].quanitityText.text = "";
            }

            inventoryButton[i].item = Player_Controller.instance.playerInventory[i];
        }
    }

    public void foundNewItem(Item newItem)
    {
        //ItemNameText.text = newItem.name;
        //ItemDescriptionText.text = newItem.description;
        //ItemImage.sprite = newItem.itemInventoryImage;
        //FoundNewItemPanel.SetActive(true);
    }

    public void deactiveFoundNewItemUI()
    {
        FoundNewItemPanel.SetActive(false);
    }

    public void updateItemQuantity(int index)
    {
        inventoryButton[index].quanitityText.text = Player_Controller.instance.playerInventory[index].quantity.ToString();
    }

    //public void updateArrowItemQuantity(int index)
    //{
    //    arrowInventoryButtons[index].quanitityText.text = Player_Controller.instance.playerInventory[index].quantity.ToString();
    //}

    public void updateInventoryText()
    {
        //int buttonIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        //Item temp = inventoryButton[buttonIndex].item;

        //itemNameText.text = temp.itemName;
        //itemDescriptionText.text = temp.description;
    }

    public void checkPlayerDecision()
    {
        ItemUseOptionButtons.SetActive(false);
        CheckDecisionUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(YesButton);
    }

    public void closeCheckPlayerDecision()
    {
        ItemUseOptionButtons.SetActive(true);
        CheckDecisionUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(DropButton);
    }

    public void EquipItem()
    {
        equipItem?.Invoke(currentSelectedButtonIndex);
        closeItemUseOptions();
    }

    public void DropItem()
    {
        dropItem?.Invoke(currentSelectedButtonIndex);
        //Add are you sure
        closeItemUseOptions();
    }

    public void InspectItem()
    {
        string temp = inspectItem?.Invoke(currentSelectedButtonIndex);

        Debug.Log(temp);

        if (temp != null)
        {
            List<DialogueLine> list = new List<DialogueLine>();

            DialogueLine dialogueLine = new DialogueLine();
            dialogueLine.dialogue = temp;
            dialogueLine.speakerName = "narrator";

            list.Add(dialogueLine);

            StartCoroutine(Dialogue_Controller.instance.DialogueInteraction(list));
        }
        else
        {
            List<DialogueLine> list = new List<DialogueLine>();

            DialogueLine dialogueLine = new DialogueLine();
            dialogueLine.dialogue = "You are uncertain as to what this is.";
            dialogueLine.speakerName = "narrator";

            list.Add(dialogueLine);

            StartCoroutine(Dialogue_Controller.instance.DialogueInteraction(list));
        }
    }
    #endregion

    #region settings - volume
    public void changeMasterVolume()
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume.value) * 20);
    }

    public void changeMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume.value) * 20);
    }

    public void changeSFXVolume()
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume.value) * 20);
    }
    #endregion

    #region settings - game resolution
    public void changeScreenResColor()
    {
        resolutionButton.GetComponent<Image>().color = Color.white;
    }

    public void changeScreenResColorBack()
    {
        resolutionButton.GetComponent<Image>().color = new Color(0.4823529f, 0.4823529f, 0.4823529f, 1f);
    }
    #endregion

    #region pause menu selectable behaviour
    public void ChangeSelectedButtonText()
    {
        GameObject temp = EventSystem.current.currentSelectedGameObject;
        TextMeshProUGUI buttonText = temp.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = Color.white;
        buttonText.fontSize = 45;
        buttonText.ForceMeshUpdate();
    }

    public void ChangeSelectedButtonTextBack()
    {
        GameObject temp = EventSystem.current.currentSelectedGameObject;
        TextMeshProUGUI buttonText = temp.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = new Color(0.4823529f, 0.4823529f, 0.4823529f, 1f);
        buttonText.fontSize = 40;
        buttonText.ForceMeshUpdate();
    }

    public void ChangeSelectedToggle()
    {
        Toggle temp = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
        ColorBlock tempToggle = temp.colors;
        tempToggle.normalColor = Color.white;
        temp.colors = tempToggle;

        TextMeshProUGUI toggleText = temp.GetComponentInChildren<TextMeshProUGUI>();
        toggleText.color = Color.white;
        toggleText.ForceMeshUpdate();
    }

    public void ChangeSelectedToggleBack()
    {
        Toggle temp = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();
        ColorBlock tempToggle = temp.colors;
        tempToggle.normalColor = new Color(0.4823529f, 0.4823529f, 0.4823529f, 1f);
        temp.colors = tempToggle;

        TextMeshProUGUI toggleText = temp.GetComponentInChildren<TextMeshProUGUI>();
        toggleText.color = new Color(0.4823529f, 0.4823529f, 0.4823529f, 1f);
        toggleText.ForceMeshUpdate();
    }

    public void ChangeSelectedSlider()
    {
        Slider temp = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
        TextMeshProUGUI sliderTitleText = temp.GetComponentInChildren<TextMeshProUGUI>();
        sliderTitleText.color = Color.white;
        sliderTitleText.fontSize = 38;
        sliderTitleText.ForceMeshUpdate();

        Image[] tempImage = EventSystem.current.currentSelectedGameObject.GetComponentsInChildren<Image>();

        foreach (Image image in tempImage)
        {
            if (image.CompareTag("Fill"))
            {
                image.color = Color.white;
            }
        }

        ColorBlock tempToggle = temp.colors;
        tempToggle.normalColor = Color.white;
        temp.colors = tempToggle;
    }

    public void ChangeSelectedSliderBack()
    {
        Slider temp = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
        TextMeshProUGUI sliderTitleText = temp.GetComponentInChildren<TextMeshProUGUI>();
        sliderTitleText.color = new Color(0.4823529f, 0.4823529f, 0.4823529f, 1f);
        sliderTitleText.fontSize = 36;
        sliderTitleText.ForceMeshUpdate();

        Image[] tempImage = EventSystem.current.currentSelectedGameObject.GetComponentsInChildren<Image>();

        foreach(Image image in tempImage)
        {
            if(image.CompareTag("Fill"))
            {
                image.color = new Color(0.4823529f, 0.4823529f, 0.4823529f, 1f);
            }
        }

        ColorBlock tempToggle = temp.colors;
        tempToggle.normalColor = new Color(0.4823529f, 0.4823529f, 0.4823529f, 1f);
        temp.colors = tempToggle;
    }
    #endregion

    #region Selectable Inventory

    //Might need to redo items to make the inventory code cleaner.
    public void updateSelectableInventory()
    {
        ////Update inventory
        //for (int i = 0; i < selectableInventoryButton.Length; i++)
        //{
        //    Image temp = selectableInventoryButton[i].image;

        //    if (Player_Controller.instance.playerItems[i].hasItem)
        //    {
        //        temp.sprite = Player_Controller.instance.playerItems[i].itemInventoryImage;
        //        temp.color = new Color(1, 1, 1, 1);

        //        if (Player_Controller.instance.playerItems[i].quantity > 1)
        //        {
        //            updateItemQuantity(i);
        //        }
        //        else
        //        {
        //            selectableInventoryButton[i].quanitityText.text = "";
        //        }


        //    }
        //    else
        //    {
        //        temp.color = new Color(0, 0, 0, 1);
        //        selectableInventoryButton[i].quanitityText.text = "";
        //    }

        //    selectableInventoryButton[i].item = Player_Controller.instance.playerItems[i];
        //}

        ////Update Arrows
        //for (int i = 0; i < selectableArrowInventoryButtons.Length; i++)
        //{
        //    Image temp = selectableArrowInventoryButtons[i].image;

        //    if (Player_Controller.instance.playerArrows[i].hasItem)
        //    {
        //        temp.sprite = Player_Controller.instance.playerArrows[i].itemInventoryImage;
        //        temp.color = new Color(1, 1, 1, 1);

        //        if (Player_Controller.instance.playerArrows[i].quantity > 1)
        //        {
        //            updateArrowItemQuantity(i);
        //        }
        //        else
        //        {
        //            selectableArrowInventoryButtons[i].quanitityText.text = "";
        //        }

        //    }
        //    else
        //    {
        //        temp.color = new Color(0, 0, 0, 1);
        //        selectableArrowInventoryButtons[i].quanitityText.text = "";
        //    }

        //    selectableArrowInventoryButtons[i].item = Player_Controller.instance.playerArrows[i];
        //}

        ////Update Accessories
        //for (int i = 0; i < selectableAccessoryInventoryButtons.Length; i++)
        //{
        //    Image temp = selectableAccessoryInventoryButtons[i].image;

        //    if (Player_Controller.instance.playerAccessories[i].hasItem)
        //    {
        //        temp.sprite = Player_Controller.instance.playerAccessories[i].itemInventoryImage;
        //        temp.color = new Color(1, 1, 1, 1);
        //    }
        //    else
        //    {
        //        temp.color = new Color(0, 0, 0, 1);
        //    }

        //    selectableAccessoryInventoryButtons[i].item = Player_Controller.instance.playerAccessories[i];
        //}
    }

    public IEnumerator OpenSelectableInventory(Item item)
    {
        updateSelectableInventory();
        SelectableInventoryPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstButtonInSelectableInventory);

        yield return StartCoroutine(waitForSelectedItem());

        item = EventSystem.current.currentSelectedGameObject.GetComponent<InventoryButton>().item;
    }

    public void MadeSelection()
    {
        waitingForSelection = false;
    }

    public IEnumerator waitForSelectedItem()
    {
        while(waitingForSelection)
        {
            yield return null;
        }

        waitingForSelection = true;

    }

    public Item returnItem()
    {
        return EventSystem.current.currentSelectedGameObject.GetComponent<InventoryButton>().item;
    }
    #endregion

    public void OpenOptions()
    {
        EventSystem.current.SetSelectedGameObject(firstButtonInOptionsMenu);
        gameSettings.SetActive(true);
    }

    public void CloseOptions()
    {
        EventSystem.current.SetSelectedGameObject(firstButtonInPauseMenu);
    }

    public void UpdateCloseOptionsButton()
    {
        GameObject temp = EventSystem.current.currentSelectedGameObject;
        Navigation tempNav = new Navigation();
        tempNav.mode = Navigation.Mode.Explicit;

        if (temp.CompareTag("GameSettings"))
        {
            tempNav.selectOnUp = lastGameObjectInGameSettings;
            closeButton.GetComponent<Button>().navigation = tempNav;
        }
        else if (temp.CompareTag("VideoSettings"))
        {
            tempNav.selectOnUp = lastGameObjectInVideoSettings;
            closeButton.GetComponent<Button>().navigation = tempNav;
        }
        else if (temp.CompareTag("AudioSettings"))
        {
            tempNav.selectOnUp = lastGameObjectInAudioSettings;
            closeButton.GetComponent<Button>().navigation = tempNav;
        }
    }

    public void fullScreen()
    {
        fullscreenOn = !fullscreenOn;
    }

    public void indexRightScreenResolution()
    {
        if (resolutionIndex + 1 == resolutions.Count)
        {
            resolutionIndex = 0;
        }
        else
        {
            resolutionIndex++;
        }

        resolutionText.text = resolutions[resolutionIndex].ToString(); 
    }

    public void indexLeftScreenResolution()
    {
        if (resolutionIndex - 1 <= -1)
        {
            resolutionIndex = resolutions.Count - 1;
        }
        else
        {
            resolutionIndex--;
        }

        resolutionText.text = resolutions[resolutionIndex].ToString();
    }

    public void setScreenResolution()
    {
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, fullscreenOn);
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

    public void DisplayOutroCutscene()
    {
        cutsceneTitleText.text = "Epilogue";
        StartCoroutine(Cutscene(4, () =>
        {
            StartCoroutine(Player_Controller.instance.gameWin());
            Player_Controller.instance.canInput = false;
        }));
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

    private IEnumerator Cutscene(int idx, System.Action callback = null)
    {
        playerUI.SetActive(false);
        cutsceneUI.SetActive(true);
        cutsceneContinue.SetActive(false);
        cutsceneSkip = false;
        Player_Controller.instance.canInput = false;
        Player_Controller.instance.isTransitioning = true;
        Player_Controller.instance.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        cutsceneDisplayText.text = "";
        string contentText = cutsceneTexts[idx];
        cutsceneImages[idx].SetActive(true);
        cutsceneSubtitleText.text = cutsceneSubtitles[idx];
        // ---
        crossFadeOut.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        crossFadeOut.SetActive(false);
        // ---
        yield return new WaitForSeconds(1);
        for(int i = 0; i < contentText.Length; i++)
        {
            if(cutsceneSkip)
            {
                cutsceneDisplayText.text = contentText;
                break;
            }
            UIAudioSource.PlayOneShot(textSFX);
            cutsceneDisplayText.text += contentText[i];
            yield return new WaitForSeconds(cutsceneTextDuration);
        }
        // yield return new WaitForSeconds(cutsceneDuration);
        yield return new WaitForSeconds(0.4f);
        cutsceneContinue.SetActive(true);
        TextMeshProUGUI continueText = cutsceneContinue.GetComponentInChildren<TextMeshProUGUI>();
        continueText.alpha = 0f;
        StartCoroutine(FadeText(continueText, 0, 1, 1));
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire3"));
        // ---
        crossFadeIn.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        crossFadeIn.SetActive(false);
        // ---
        cutsceneContinue.SetActive(false);
        cutsceneImages[idx].SetActive(false);
        cutsceneUI.SetActive(false);
        playerUI.SetActive(true);
        // ---
        crossFadeOut.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        crossFadeOut.SetActive(false);
        // ---
        Player_Controller.instance.canInput = true;
        Player_Controller.instance.isTransitioning = false;
        Player_Controller.instance.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        callback?.Invoke();
    }
}

[System.Serializable]
public class Resolution
{
    public int width;
    public int height;

    public Resolution(int width, int height)    
    {
        this.width = width;
        this.height = height;
    }

    public override string ToString()
    {
        return width + "x" + height;
    }
}

[System.Serializable]
public class InventoryButton
{
    public Image image;
    public TextMeshProUGUI quanitityText;

    public Item item { get; set; }
}