using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerArrow_Controller : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public float damage;

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        changeDirection(Player_Controller.instance.facingTowards.transform.position - Player_Controller.instance.gameObject.transform.position);
        Destroy(gameObject, 5f);
    }

    public void changeDirection(Vector3 direction)
    {
        if (direction.x == 0f && direction.y == 1f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
        else if (direction.x == 0f && direction.y == -1f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        }
        else if (direction.x == 1f && direction.y == 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (direction.x == -1f && direction.y == 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }

        rb.AddForce(new Vector2(direction.x, direction.y) * speed);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            rb.linearVelocity = Vector2.zero;
            Vector2 direction = (collision.gameObject.transform.position - gameObject.transform.position).normalized;
            collision.gameObject.GetComponent<Enemy_Controller>().TakeDamage(damage, direction);
            Destroy(gameObject);
        }
        if (!collision.gameObject.CompareTag("Player"))
        {
            rb.linearVelocity = Vector2.zero;
            Destroy(gameObject);
        }
    }
}
