using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Tutorial_Controller : MonoBehaviour
{
    // Tutorial Controller
    // Handles tutorial section and
    // triggers assoc. popups.

    public List<GameObject> tutorialColliders;

    public static Tutorial_Controller instance;

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

    public void DisplayTutorial()
    {
        UI_Controller.instance.PopupText("Use WASD to move. Press SPACE to dash.");
        UI_Controller.instance.PopupText("Left Click to attack.");
    }
}
