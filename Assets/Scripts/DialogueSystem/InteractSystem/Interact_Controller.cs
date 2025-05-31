using UnityEngine;
using UnityEngine.Events;

public class Interact_Controller : MonoBehaviour
{
    public UnityEvent interaction;
    public float radiusToShowInteractUI;

    //void Update()
    //{
    //    //Can also do a onTriggerEnter but this works. May need to change if optimization becomes a problem.
    //    Vector2 transformTest = transform.position - Player_Controller.instance.transform.position;

    //    if ((Mathf.Abs(transformTest.x) <= radiusToShowInteractUI) && (Mathf.Abs(transformTest.y) <= radiusToShowInteractUI))
    //    {
    //        if (Player_Controller.instance.canIntectIndicator.activeInHierarchy == false)
    //        {
    //            Player_Controller.instance.canIntectIndicator.SetActive(true);
    //        }
    //    }
    //    else
    //    {
    //        if (Player_Controller.instance.canIntectIndicator.activeInHierarchy)
    //        {
    //            Player_Controller.instance.canIntectIndicator.SetActive(false);
    //        }
    //    }
    //}

    public void Interact()
    {
        interaction.Invoke();
    }
}
