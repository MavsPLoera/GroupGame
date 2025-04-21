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
        currentArea = areas[areaIndex];
        if(!currentArea.isDiscovered)
        {
            StartCoroutine(LocationDiscovered());
            currentArea.isDiscovered = true;
        }
    }

    public void ExitArea(int areaIndex)
    {
        currentArea = null;
        currentLocationText.text = "";
    }

    private void Update()
    {
        if(currentArea != null)
        {
            currentLocationText.text = currentArea.name;
        }
    }

    private IEnumerator LocationDiscovered()
    {
        locationDiscoveredText.text = $"Discovered {currentArea.name}";
        yield return new WaitForSeconds(3);
        locationDiscoveredText.text = "";
    }
}
