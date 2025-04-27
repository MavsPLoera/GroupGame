using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_Controller : MonoBehaviour
{

    [Header("UI Controller Misc.")]
    public TextMeshProUGUI currentLocationText;
    public TextMeshProUGUI locationDiscoveredText;

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

    void Start()
    {
        
    }

    void Update()
    {
        
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

/*
private IEnumerator LocationDiscovered()
{
    currentArea.isDiscovered = true;
    locationDiscoveredText.text = $"Discovered {currentArea.name}";
    yield return StartCoroutine(FadeText(locationDiscoveredText, 0, 1, 0.75f));
    yield return new WaitForSeconds(textDisplayDuration);
    yield return StartCoroutine(FadeText(locationDiscoveredText, 1, 0, 0.75f));
    locationDiscoveredText.text = "";
}
*/
