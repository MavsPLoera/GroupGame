using UnityEngine;

public class Projectile_Controller : MonoBehaviour
{
    [Header("Stats.")]
    public float damage;
    public float speed;
    public float lifespan;

    private Rigidbody2D _rb;
    private readonly bool _debug = true;

    public void SetTarget(Vector3 direction)
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.AddForce(direction * speed);
    }

    void Update()
    {
        lifespan -= Time.deltaTime;
        if(lifespan <= 0)
        {
            if(_debug) Debug.Log("Projectile Despawned");
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_debug) Debug.Log($"Projectile Hit {collision.gameObject.name}");
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player_Controller>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if(collision.gameObject.CompareTag("ColliderTilemap"))
        {
            Destroy(gameObject);
        }
    }
}
