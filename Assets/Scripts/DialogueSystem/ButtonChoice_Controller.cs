using TMPro;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonChoice_Controller : MonoBehaviour
{
    public Button Button;
    public TextMeshProUGUI ButtonText;
    public int SkipToWhatLine;

    public void SetButtonText(string text)
    {
        ButtonText.text = text;
    }

    public void SetSkipToWhatLine(int skipToWhatLine)
    {
        SkipToWhatLine = skipToWhatLine;
    }

}
