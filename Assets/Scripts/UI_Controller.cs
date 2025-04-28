using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_Controller : MonoBehaviour
{

    [Header("UI Controller Misc.")]
    public TextMeshProUGUI currentLocationText;
    public TextMeshProUGUI locationDiscoveredText;
    public TextMeshProUGUI dungeonClearedText;
    public float textDisplayDuration;

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
    }

    public void EnterArea(string name)
    {
        StartCoroutine(FadeText(name, 0, 1, 0.75f));
    }

    public void ExitArea(string name)
    {
        StartCoroutine(FadeText(name, 1, 0, 0.75f));
    }

    public void DiscoverLocation(string name)
    {
        StartCoroutine(DisplayDiscoveredLocation(name));
    }


    public IEnumerator DisplayDiscoveredLocation(string name)
    {
        // TODO: add AudioClip.
        locationDiscoveredText.text = $"Discovered {name}";
        yield return StartCoroutine(FadeText(name, 0, 1, 0.75f));
        yield return new WaitForSeconds(textDisplayDuration);
        yield return StartCoroutine(FadeText(name, 1, 0, 0.75f));
        locationDiscoveredText.text = "";
    }

    public IEnumerator FadeText(string name, float currentAlpha, float targetAlpha, float transitionTime)
    {
        currentLocationText.text = name;
        Color originalColor = currentLocationText.color;
        currentLocationText.alpha = currentAlpha;
        float time = 0;
        while(time < transitionTime)
        {
            time += Time.deltaTime;
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, time / transitionTime);
            currentLocationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);
            yield return null;
        }
        currentLocationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, targetAlpha);
    }
}
