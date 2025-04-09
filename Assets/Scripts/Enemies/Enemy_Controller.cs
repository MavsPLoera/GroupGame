using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class Enemy_Controller : MonoBehaviour
{
    // Enemy Controller
    // Handles generic enemy functions.

    [Header("M01 Misc.")]
    public List<GameObject> drops;
    public TextMeshProUGUI text;
    public string[] textList;

    [Header("Stats.")]
    public float health;
    public float damage;

    private Rigidbody2D _rb;
    private Transform _playerTransform;
    private bool _isKnockedback = false;
    private readonly bool _debug = true;

    public void TakeDamage(float damage)
    {
        if(_debug) Debug.Log($"Damaged {gameObject.name} {damage}");
        health -= damage;

        // ** TODO: corr. SFX and particle systems **

        // StartCoroutine(Knockback());
        StartCoroutine(DamageColor());
        // StartCoroutine(DamageText());

        if(health <= 0)
        {
            if(_debug) Debug.Log($"{gameObject.name} Dead");
            OnDeath();
        }
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerTransform = Player_Controller.instance.transform;
    }

    private void OnDeath()
    {
        // Drop item.
        /*
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
        */
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.transform.parent.CompareTag("Player"))
        {
            float damage = collision.gameObject.transform.parent.GetComponent<Player_Controller>().swordDamage;
            TakeDamage(damage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            //collision.gameObject.GetComponent<Player_Controller>().TakeDamage(damage);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            //collision.gameObject.GetComponent<Player_Controller>().TakeDamage(damage);
        }
    }
}
