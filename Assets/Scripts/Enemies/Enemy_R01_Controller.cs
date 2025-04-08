using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class Enemy_R01_Controller : MonoBehaviour
{
    // Enemy Ranged 01
    // Moves toward player and fires when in range.

    [Header("R01 Misc.")]
    public List<GameObject> drops;
    public TextMeshProUGUI text;
    public string[] textList;

    [Header("Stats.")]
    public float health;
    public float damage;
    public float speedMin;
    public float speedMax;

    private float _speed;
    private Rigidbody2D _rb;
    private Transform _playerTransform;
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
        if(!_playerTransform && Player_Controller.instance)
        {
            _playerTransform = Player_Controller.instance.transform;
        }

        if(_playerTransform && !_isKnockedback)
        {
            // Get vector in direction of player.
            Vector2 moveDirection = (_playerTransform.transform.position - transform.position);

            // Check distance. Stop if in range to player.
            float distance = moveDirection.magnitude;
            if(distance <= 3.5f)
            {
                // ** TODO: if within a certain range
                // of the player, perform attack animation. **
                _rb.linearVelocity = Vector2.zero;
                if(_debug) Debug.Log($"Aligned");
            }
            else
            {
                // moveDirection = moveDirection.normalized;
                MoveToward(moveDirection);
            }
        }
    }

    private void MoveToward(Vector2 moveDirection)
    {
        // Move along either X or Y axis. Prefer moving toward the axis
        // enemy is closest alinging to.
        if(Mathf.Abs(moveDirection.x) < Mathf.Abs(moveDirection.y))
        {
            if(Mathf.Abs(moveDirection.x) <= 0.05f)
            {
                moveDirection = new Vector2(0, moveDirection.y).normalized;
            }
            else
            {
                moveDirection = new Vector2(moveDirection.x, 0).normalized;
            }
        }
        else
        {
            if(Mathf.Abs(moveDirection.y) <= 0.05f)
            {
                moveDirection = new Vector2(moveDirection.x, 0).normalized;
            }
            else
            {
                moveDirection = new Vector2(0, moveDirection.y).normalized;
            }
        }
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
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player_Controller>().TakeDamage(damage);
        }
    }
}
