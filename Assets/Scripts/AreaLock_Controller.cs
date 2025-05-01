using System;
using UnityEditor;
using UnityEngine;

public class AreaLock_Controller : MonoBehaviour
{
    public GameObject secondaryNeededArea;
    public GameObject ultimateNeededArea;
    public GameObject sewersLock;
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
            UI_Controller.instance.PopupText("You're not ready to go here");
            Debug.Log("Need secondary ability to enter this area");
        }
        else if (tag.Equals("ultimateNeededLock"))
        {
            UI_Controller.instance.PopupText("You're not ready to go here");
            Debug.Log("Need ultimate ability to enter this area");
        }
        else if (tag.Equals("sewersLock"))
        {
            UI_Controller.instance.PopupText("You do not have the key to enter this area");
            Debug.Log("Need quest to enter this area");
        }
        else
        {
            Debug.Log("Tag bad");
        }
    }


    //Basic area unlocks can also add noises UI text, the whole 9 yards.
    [ContextMenu("unlockSewers")]
    public void unlockSewers()
    {
        sewersLock.SetActive(false);
    }

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
