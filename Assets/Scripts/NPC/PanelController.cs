using UnityEngine;
using UnityEngine.UI; 

public class PanelController : MonoBehaviour
{
    [SerializeField] private GameObject panel; 
    void Start()
    {
        panel.SetActive(false);
    }
    public void ShowPanel()
    {
        panel.SetActive(true);
    }
    
    public void HidePanel()
    {
        panel.SetActive(false);
    }
}