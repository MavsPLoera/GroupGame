using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

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

    private enum TextType
    {
        Generic,
        Damage
    }

    [Header("Enemy Misc.")]
    public List<GameObject> drops;
    public TextMeshProUGUI text;
    public string[] genericTextList;
    public string[] damageTextList;
    public float flickerAmount;
    public float knockbackAmount;
    public EnemyType enemyType; // ATM used for triggering appropriate animations for each enemy type.
    public bool isInAnimation;

    [Header("Enemy Stats.")]
    public float health;
    public float damage;

    [Header("Enemy Cooldowns")]
    public float flickerDuration;
    public float attackCooldownTime;

    private Animator _animator;
    private Rigidbody2D _rb;
    private Transform _playerTransform;
    private Vector2 _originalPosition;
    private Color32 _originalColor;
    private float _originalHealth;
    private SpriteRenderer _spriteRenderer;
    private bool _attackCooldown = false;
    private readonly bool _debug = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _playerTransform = Player_Controller.instance.transform;
        _originalPosition = transform.position;
        _originalColor = _spriteRenderer.color;
        _originalHealth = health;
        GetComponentInChildren<Canvas>().worldCamera = Camera_Controller.instance.GetComponent<Camera>();
    }

    private void Update()
    {
        if(!isInAnimation)
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

    public void TakeDamage(float damage, Vector2 direction)
    {
        if(_debug) Debug.Log($"Damaged {gameObject.name} {damage}");
        health -= damage;

        StartCoroutine(Knockback(direction));
        StartCoroutine(FlickerSprite());

        /*
        int textChance = Random.Range(0, 100);
        if(textChance <= 25)
        {
            StartCoroutine(DisplayText(TextType.Damage));
        }
        */

        if(health <= 0)
        {
            if(_debug) Debug.Log($"{gameObject.name} Dead");
            OnDeath();
        }
    }

    public void Attack(Collision2D collision)
    {
        // Allow attack despite cooldown if in collision.
        if(!isInAnimation && (!_attackCooldown || collision != null))
        {
            // TEMP
            StartCoroutine(Swing());
            // collision?.gameObject.GetComponent<Player_Controller>().TakeDamage(damage);
            Player_Controller.instance.TakeDamage(damage);
            StartCoroutine(AttackCooldown());
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
        // Destroy(gameObject);
        StartCoroutine(Death());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.transform.parent && collision.gameObject.transform.parent.CompareTag("Player"))
        {
            if(_debug) Debug.Log($"Sword Hit");
            if(!isInAnimation) 
            {
            }    
            float damage = collision.gameObject.transform.parent.GetComponent<Player_Controller>().swordDamage;
            // Testing...
            // Vector2 direction = (transform.position - Player_Controller.instance.transform.position).normalized;
            Vector2 direction = (Player_Controller.instance.facingTowards.transform.position - Player_Controller.instance.transform.position).normalized;
            TakeDamage(damage, direction);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Attack(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Attack(collision);
        }
    }

    private void OnEnable()
    {
        int textChance = Random.Range(0, 100);
        if(textChance <= 10)
        {
            StartCoroutine(DisplayText(TextType.Generic));
        }
    }

    private void OnDisable()
    {
        // Reset enemy stats and position.
        transform.position = _originalPosition;
        _spriteRenderer.color = _originalColor;
        health = _originalHealth;
        isInAnimation = false;
        _attackCooldown = false;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        StopAllCoroutines();
    }

    private IEnumerator Knockback(Vector2 direction)
    {
        if(_debug) Debug.Log($"{gameObject.name} Knockbacked");
        string animation = System.String.Concat(enemyType, "_Death");
        isInAnimation = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(direction * knockbackAmount);
        _animator.Play(animation, 0);
        yield return new WaitForSeconds(.6f);
        isInAnimation = false;
    }

    private IEnumerator FlickerSprite()
    {
        for(int i = 0; i < flickerAmount; i++)
        {
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(flickerDuration);
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(flickerDuration);
        }
        _spriteRenderer.color = _originalColor;
    }

    private IEnumerator DisplayText(TextType textType)
    {
        switch(textType)
        {
            case TextType.Generic:
                text.text = genericTextList[Random.Range(0, genericTextList.Length)];
                break;
            case TextType.Damage:
                text.text = damageTextList[Random.Range(0, damageTextList.Length)];
                break;
            default:
                break;
        }
        yield return new WaitForSeconds(3);
        text.text = "";
    }

    public IEnumerator Swing()
    {
        string animation = System.String.Concat(enemyType, "_Swing");
        isInAnimation = true;
        // Freeze X, Y, and Z.
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        if(enemyType != EnemyType.Skeleton_Ranged)
        {
            _animator.Play(animation, 0);
        }
        // Allow animation to complete.
        yield return new WaitForSeconds(.6f);
        // Freeze Z.
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        isInAnimation = false;
    }

    public IEnumerator Shoot()
    {
        string animation = System.String.Concat(enemyType, "_Shoot");
        isInAnimation = true;
        // Freeze X, Y, and Z.
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        if (enemyType == EnemyType.Skeleton_Ranged)
        {
            _animator.Play(animation, 0);
        }
        // Allow animation to complete.
        yield return new WaitForSeconds(.6f);
        // Freeze Z.
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        isInAnimation = false;
    }

    private IEnumerator Death()
    {
        string animation = System.String.Concat(enemyType, "_Death");
        isInAnimation = true;
        // Freeze X, Y, and Z.
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _animator.Play(animation, 0);
        // Allow animation to complete.
        yield return new WaitForSeconds(.6f);
        isInAnimation = false;
        gameObject.SetActive(false);
    }

    private IEnumerator AttackCooldown()
    {
        _attackCooldown = true;
        isInAnimation = true;
        _rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(attackCooldownTime);
        _attackCooldown = false;
        isInAnimation = false;
    }
}
