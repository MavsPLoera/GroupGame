using UnityEngine;

public class Tutorial_Controller : MonoBehaviour
{
    // Tutorial Controller
    // Handles tutorial section and
    // triggers assoc. popups.

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

    void Start()
    {
        UI_Controller.instance.PopupText("Use WASD to move. Press SPACE to dash.");
    }

    void Update()
    {
        
    }
}
