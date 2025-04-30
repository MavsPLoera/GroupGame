using UnityEngine;
using TMPro;

public class CoinCount : MonoBehaviour
{
    public static int totalCoinsCollected = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            totalCoinsCollected++;
            Debug.Log("Coins Collected: " + totalCoinsCollected);
            Destroy(gameObject);
        }
    }

    
}
