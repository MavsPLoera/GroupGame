using UnityEngine;

public class Sword_Controller : MonoBehaviour
{
    public BoxCollider2D swordHitBox;

    void Start()
    {
        swordHitBox = GetComponent<BoxCollider2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            //Tell enemy to hurt themselves
        }
    }

    public void updateHitBox(Vector2 newOffSet, Vector2 newOnSize)
    {
        swordHitBox.offset = newOffSet;
        swordHitBox.size = newOnSize;
    }
}
