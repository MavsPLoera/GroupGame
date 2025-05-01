using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using System.Linq;

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
        public bool isCleared = false;
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

    private void Update()
    {
        if(currentArea != null && !currentArea.isCleared)
        {
            // Continually check enemies. If all dead, set isCleared.
            List<GameObject> currentEnemies = currentArea.enemies?.Where(enemy => enemy.activeSelf).ToList();

            if(currentEnemies != null && currentEnemies.Count == 0)
            {
                currentArea.isCleared = true;
            }
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

        /*
        if(!currentArea.isCleared)
        {
            currentArea?.enemies.ForEach(enemy => enemy.SetActive(true));
        }
        */
        if(currentArea != null && currentArea.enemies != null)
        {
            foreach(GameObject enemy in currentArea.enemies)
            {
                if(enemy.GetComponent<Enemy_Controller>().wasKilled == false)
                {
                    enemy.SetActive(true);
                }
            }
        }
    }

    public void ExitArea(int areaIndex)
    {
        if(areaIndex < 0 || areaIndex >= areas.Count || currentArea == null) return;
        string name = currentArea.name;
        if(instance)
        {
            UI_Controller.instance.ExitArea(name);
        }

        currentArea?.enemies.ForEach(enemy => enemy.SetActive(false));
        currentArea = null;
    }

    public void ResetArea(int areaIndex = -1)
    {
        if(areaIndex == -1)
        {
            // Reset current area state on death.
            currentArea?.enemies.ForEach(enemy => enemy.SetActive(false));
            currentArea.isCleared = false;
            currentArea = null;
        }    
        else
        {
            // Reset passed in area index.
            currentArea = areas[areaIndex];
            currentArea?.enemies.ForEach(enemy => enemy.SetActive(false));
            currentArea.isCleared = false;
            currentArea = null;
        }
    }

    public void FullReset()
    {
        foreach(var area in areas)
        {
            area?.enemies.ForEach(enemy => enemy.GetComponent<Enemy_Controller>().wasKilled = false);
        }
    }
}
