using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class Enemy_ENV01_Controller : MonoBehaviour
{
    // Enemy Environmental 01
    // Moves back and forth on collision.
    
    [Header("ENV01 Misc.")]
    public List<GameObject> drops;
    public TextMeshProUGUI text;
    public string[] textList;
    public Vector2 moveDirection;

    [Header("Stats.")]
    public float health;
    public float damage;
    public float speedMin;
    public float speedMax;

    private float _speed;
    private Rigidbody2D _rb;
    private bool _isKnockedback = false;
    private readonly bool _debug = true;

    public void TakeDamage(float damage)
    {
        if(_debug) Debug.Log($"Damaged {gameObject.name} {damage}");

        // ** TODO: corr. SFX and particle systems **

        StartCoroutine(Knockback());
        StartCoroutine(DamageColor());
        StartCoroutine(DamageText());

        if(health <= 0)
        {
            if(_debug) Debug.Log($"{gameObject.name} Dead");
            OnDeath();
        }
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Get random speed between _speedMax and _speedMin.
        _speed = Random.Range(speedMin, speedMax);
    }

    private void Update()
    {
        if(!_isKnockedback)
        {
            Move(moveDirection);
        }
    }

    private void Move(Vector2 moveDirection)
    {
       _rb.linearVelocity = moveDirection * _speed;
    }

    private void OnDeath()
    {
        // Drop item.
        int dropChance = Random.Range(0, 100);
        if(dropChance <= 10)
        {
            // Drop health poition.
            Instantiate(drops[1], transform.position, transform.rotation);
        }
        else
        {
            // Drop gold.
            Instantiate(drops[0], transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

    private IEnumerator Knockback()
    {
        // Pauses movement on knockback.
        _isKnockedback = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(2);
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _isKnockedback = false;
    }

    private IEnumerator DamageColor()
    {
        Color32 origColor = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 100, 100, 255);
        yield return new WaitForSeconds(2);
        gameObject.GetComponent<SpriteRenderer>().color = origColor;
    }

    private IEnumerator DamageText()
    {
        text.text = textList[Random.Range(0, textList.Length)];
        yield return new WaitForSeconds(3);
        text.text = "";
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player_Controller>().TakeDamage(damage);
        }
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("ColliderTilemap"))
        {
            // Switch directions.
            moveDirection *= -1;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player_Controller>().TakeDamage(damage);
        }
    }
}
