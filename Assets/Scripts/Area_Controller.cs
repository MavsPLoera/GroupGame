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
            // StartCoroutine(UI_Controller.instance.LocationDiscovered());
        }
        StartCoroutine(UI_Controller.instance.FadeText(currentArea.name, 0, 1, 0.75f));
    }

    public void ExitArea(int areaIndex)
    {
        if(areaIndex < 0 || areaIndex >= areas.Count) return;
        if(currentArea != null)
        {
            string name = currentArea.name;
            if(instance) StartCoroutine(UI_Controller.instance.FadeText(name, 1, 0, 0.75f));
            currentArea = null;
        }    
    }

}
