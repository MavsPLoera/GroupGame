using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Area_Controller : MonoBehaviour
{
    // Area Controller
    // Manages overworld areas.

    [System.Serializable]
    public class Area
    {
        public string name;
        public GameObject areaCollider;
        public List<GameObject> enemies;
        public bool isDiscovered = false;
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
            currentArea.isDiscovered = true;
            UI_Controller.instance.DiscoverLocation(currentArea.name);
        }
        UI_Controller.instance.EnterArea(currentArea.name);
    }

    public void ExitArea(int areaIndex)
    {
        if(areaIndex < 0 || areaIndex >= areas.Count || currentArea == null) return;
        string name = currentArea.name;
        if(instance)
        {
            UI_Controller.instance.ExitArea(name);
        }
        currentArea = null;
    }

}
