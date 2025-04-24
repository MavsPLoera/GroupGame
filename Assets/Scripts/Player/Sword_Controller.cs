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
        if (collision.CompareTag("Bullet"))
        {
            //Can do calculation to see if the sword cut the arrow in half
            Destroy(collision.gameObject);
        }
    }

    public void updateHitBox(Vector2 newOffSet, Vector2 newOnSize)
    {
        swordHitBox.offset = newOffSet;
        swordHitBox.size = newOnSize;
    }
}
