using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class Enemy_Controller : MonoBehaviour
{
    // Enemy Controller
    // Handles generic enemy functions.
    public enum EnemyType
    {
        Skeleton,
        Skeleton_Warrior,
        Skeleton_Ranged
    }


    [Header("Enemy Misc.")]
    public List<GameObject> drops;
    public TextMeshProUGUI text;
    public string[] textList;
    public float flickerAmount;
    public EnemyType enemyType;

    [Header("Enemy Stats.")]
    public float health;
    public float damage;

    [Header("Enemy Cooldowns")]
    public float flickerDuration;

    private Animator _animator;
    private Rigidbody2D _rb;
    private Transform _playerTransform;
    private SpriteRenderer _spriteRenderer;
    private bool _isKnockedback = false;
    private readonly bool _debug = true;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _playerTransform = Player_Controller.instance.transform;
    }

    private void Update()
    {
        if(_rb.linearVelocity != Vector2.zero)
        {
            string animation = string.Concat(enemyType, "_Walk");
            _animator.Play(animation, 0);
        }
        else
        {
            string animation = string.Concat(enemyType, "_Idle");
            _animator.Play(animation, 0);
        }
    }

    public void TakeDamage(float damage)
    {
        if (_debug) Debug.Log($"Damaged {gameObject.name} {damage}");
        health -= damage;

        // ** TODO: corr. SFX and particle systems **

        // StartCoroutine(Knockback());
        StartCoroutine(DamageColor());
        // StartCoroutine(DamageText());

        if (health <= 0)
        {
            if (_debug) Debug.Log($"{gameObject.name} Dead");
            OnDeath();
        }
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
        Color32 origColor = _spriteRenderer.color;
        for (int i = 0; i < flickerAmount; i++)
        {
            _spriteRenderer.color = new Color(255f, 0f, 0f, 255f);
            yield return new WaitForSeconds(flickerDuration);
            _spriteRenderer.color = new Color(0f, 0f, 0f, 255f);
            yield return new WaitForSeconds(flickerDuration);
        }
        _spriteRenderer.color = origColor;
    }

    private IEnumerator DamageText()
    {
        text.text = textList[Random.Range(0, textList.Length)];
        yield return new WaitForSeconds(3);
        text.text = "";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.transform.parent && collision.gameObject.transform.parent.CompareTag("Player"))
        {
            if(_debug) Debug.Log($"Sword Hit");
            float damage = collision.gameObject.transform.parent.GetComponent<Player_Controller>().swordDamage;
            TakeDamage(damage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player_Controller>().TakeDamage(damage);
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
