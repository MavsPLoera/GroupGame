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
        if (!instance)
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
        else
        {
            currentLocationText.text = currentArea.name;
            StartCoroutine(FadeText(currentLocationText, 0, 1));
        }
    }

    public void ExitArea(int areaIndex)
    {
        if(areaIndex < 0 || areaIndex >= areas.Count) return;

        currentArea = null;
        // Set currentLocationText to "" once the coroutine is finished.
        StartCoroutine(FadeText(currentLocationText, 1, 0, () =>
        {
            currentLocationText.text = "";
        }));
    }

    private IEnumerator LocationDiscovered()
    {
        locationDiscoveredText.text = $"Discovered {currentArea.name}";
        yield return StartCoroutine(FadeText(locationDiscoveredText, 0, 1));
        yield return new WaitForSeconds(textDisplayDuration);
        yield return StartCoroutine(FadeText(locationDiscoveredText, 1, 0));
        locationDiscoveredText.text = "";
        currentArea.isDiscovered = true;
        currentLocationText.text = currentArea.name;
        StartCoroutine(FadeText(currentLocationText, 0, 1));
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float currentAlpha, float targetAlpha, System.Action callback = null)
    {
        Color originalColor = text.color;
        text.alpha = currentAlpha;
        while(Mathf.Abs(currentAlpha - targetAlpha) >= 0.01f)
        {
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, 4 * Time.deltaTime);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);
            yield return null;
        }
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, targetAlpha);
        callback?.Invoke();
    }
}
