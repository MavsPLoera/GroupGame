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
    public EnemyType enemyType; // ATM used for triggering appropriate animation for each enemy type.
    public bool isInAnimation;

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
        if(_animator && !isInAnimation) // The first condition is TEMP.
        {
            _animator.SetFloat("moveX", _rb.linearVelocity.x);
            _animator.SetFloat("moveY", _rb.linearVelocity.y);

            if(_rb.linearVelocity != Vector2.zero)
            {
                string animation = System.String.Concat(enemyType, "_Walk");
                _animator.Play(animation, 0);
            }
            else
            {
                string animation = System.String.Concat(enemyType, "_Idle");
                _animator.Play(animation, 0);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if(_debug) Debug.Log($"Damaged {gameObject.name} {damage}");
        health -= damage;

        // ** TODO: corr. SFX and particle systems **

        // StartCoroutine(Knockback());
        StartCoroutine(FlickerSprite());
        // StartCoroutine(DamageText());

        if(health <= 0)
        {
            if(_debug) Debug.Log($"{gameObject.name} Dead");
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.transform.parent && collision.gameObject.transform.parent.CompareTag("Player"))
        {
            if(_debug) Debug.Log($"Sword Hit");
            if(_animator && !isInAnimation) // This first condition is TEMP.
            {
                StartCoroutine(Hit());
            }    
            float damage = collision.gameObject.transform.parent.GetComponent<Player_Controller>().swordDamage;
            TakeDamage(damage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(_animator && !isInAnimation) // This first condition is TEMP.
            {
                StartCoroutine(Swing());
            }
            collision.gameObject.GetComponent<Player_Controller>().TakeDamage(damage);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(_animator && !isInAnimation) // This first condition is TEMP.
            {
                StartCoroutine(Swing());
            }
            collision.gameObject.GetComponent<Player_Controller>().TakeDamage(damage);
        }
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

    private IEnumerator FlickerSprite()
    {
        Color32 origColor = _spriteRenderer.color;
        for (int i = 0; i < flickerAmount; i++)
        {
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flickerDuration);
            _spriteRenderer.color = Color.white;
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

    private IEnumerator Swing()
    {
        string animation = System.String.Concat(enemyType, "_Swing");
        isInAnimation = true;
        _rb.linearVelocity = Vector2.zero;
        _animator.Play(animation, 0);
        // Allow animation to complete.
        yield return new WaitForSeconds(.6f);
        isInAnimation = false;
    }

    private IEnumerator Hit()
    {
        string animation = System.String.Concat(enemyType, "_Hit");
        isInAnimation = true;
        _rb.linearVelocity = Vector2.zero;
        _animator.Play(animation, 0);
        // Allow animation to complete.
        yield return new WaitForSeconds(.6f);
        isInAnimation = false;
    }
}
