using UnityEngine;
using TMPro;

public class CoinCountDisplay : MonoBehaviour
{
    public TextMeshProUGUI coinText;
    
    void Update()
    {
        coinText.text = CoinCount.totalCoinsCollected.ToString();
    }
}
