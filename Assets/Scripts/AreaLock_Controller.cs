using System;
using UnityEditor;
using UnityEngine;

public class AreaLock_Controller : MonoBehaviour
{
    public GameObject secondaryNeededArea;
    public GameObject ultimateNeededArea;
    public static AreaLock_Controller instance;

    void Start()
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

    public void cannotEnterText(string tag)
    {
        if (tag.Equals("secondaryNeededLock"))
        {
            //Call UI manager
            Debug.Log("Need secondary ability to enter this area");
        }
        else if (tag.Equals("ultimateNeededLock"))
        {
            //Call UI manager
            Debug.Log("Need ultimate ability to enter this area");
        }
        else
        {
            Debug.Log("Tag bad");
        }
    }


    //Basic area unlocks can also add noises UI text, the whole 9 yards.
    [ContextMenu("unlockSecondaryArea")]
    public void unlockSecondaryNeededAreas()
    {
        secondaryNeededArea.SetActive(false);
    }

    [ContextMenu("unlockUltimateArea")]
    public void unlockUltimateNeededAreas()
    {
        ultimateNeededArea.SetActive(false);
    }
}
