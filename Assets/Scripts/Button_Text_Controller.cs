using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Text_Controller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float opacity;
    private Color _currentColor;

    private void Start()
    {
        _currentColor = gameObject.GetComponent<TextMeshProUGUI>().color;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        gameObject.GetComponent<TextMeshProUGUI>().color = new Color(_currentColor.r, _currentColor.g, _currentColor.b, opacity);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        gameObject.GetComponent<TextMeshProUGUI>().color = new Color(_currentColor.r, _currentColor.g, _currentColor.b, 1f);
    }

    private void OnDisable()
    {
        gameObject.GetComponent<TextMeshProUGUI>().color = _currentColor;
    }
}
