using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Area_Controller : MonoBehaviour
{
    // Area Controller
    // Manages overworld areas.

    // (?) might merge this with Dungeon_Controller.

    [System.Serializable]
    public class Area
    {
        public string name;
        public GameObject areaCollider;
        public List<GameObject> enemies;
        public bool isDiscovered = false;
        public bool isUnlocked = false;
    }

    [Header("Area Controller Misc.")]
    public List<Area> areas;
    public Area currentArea;
    public TextMeshProUGUI currentLocationText;
    public TextMeshProUGUI locationDiscoveredText;
    public float textDisplayDuration;

    public static Area_Controller instance;

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

    public void EnterArea(int areaIndex)
    {
        if(areaIndex < 0 || areaIndex >= areas.Count) return;

        currentArea = areas[areaIndex];
        if(!currentArea.isDiscovered)
        {
            StartCoroutine(LocationDiscovered());
        }
        currentLocationText.text = currentArea.name;
        StartCoroutine(FadeText(currentLocationText, 0, 1, 0.75f));
    }

    public void ExitArea(int areaIndex)
    {
        if(areaIndex < 0 || areaIndex >= areas.Count) return;

        currentArea = null;
        StartCoroutine(FadeText(currentLocationText, 1, 0, 0.75f));
    }

    private IEnumerator LocationDiscovered()
    {
        currentArea.isDiscovered = true;
        locationDiscoveredText.text = $"Discovered {currentArea.name}";
        yield return StartCoroutine(FadeText(locationDiscoveredText, 0, 1, 0.75f));
        yield return new WaitForSeconds(textDisplayDuration);
        yield return StartCoroutine(FadeText(locationDiscoveredText, 1, 0, 0.75f));
        locationDiscoveredText.text = "";
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float currentAlpha, float targetAlpha, float transitionTime)
    {
        Color originalColor = text.color;
        text.alpha = currentAlpha;
        float time = 0;
        while(time < transitionTime)
        {
            time += Time.deltaTime;
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, time / transitionTime);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);
            yield return null;
        }
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, targetAlpha);
    }
}
