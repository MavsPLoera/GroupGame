using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

public class Enemy_M01_Controller : MonoBehaviour
{
    [Header("M01 Misc.")]
    public Enemy_Stats_Controller stats;
    public List<GameObject> drops;
    public TextMeshProUGUI text;
    public string[] textList;

    private float _health, _damage, _speed;
    private Rigidbody2D _rb;
    private Transform _playerTransform;
    private bool _isKnockedback = false, _isHorizontal = true;
    private readonly bool _debug = false;

    public void TakeDamage(float damage)
    {
        if(_debug) Debug.Log($"Damaged {gameObject.name} {damage}");

        // ** TODO: corr. SFX and particle systems **

        StartCoroutine(Knockback());
        StartCoroutine(DamageColor());
        StartCoroutine(DamageText());

        if(_health <= 0)
        {
            if(_debug) Debug.Log($"{gameObject.name} Dead");
            OnDeath();
        }
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Get assoc. stats.
        float _speedMin = stats.speedMin;
        float _speedMax = stats.speedMax;
        _health = stats.health;
        _damage = stats.damage;

        // Get random speed between _speedMax and _speedMin.
        _speed = Random.Range(_speedMin, _speedMax);
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

            // Check distance. Stop if close to player.
            float distance = moveDirection.magnitude;
            if(distance <= 1)
            {
                // ** TODO: if within a certain range
                // of the player, perform attack animation. **
                _rb.linearVelocity = Vector2.zero;
            }
            else
            {
                moveDirection = moveDirection.normalized;
                MoveToward(moveDirection);
            }
        }
    }

    private void MoveToward(Vector2 moveDirection)
    {
        // Move along a single axis (X/Y) until aligned, then switch.
        // Doing this to mirror 4-directional player movement.
        if(_isHorizontal)
        {
            if(Mathf.Abs(moveDirection.x) < 0.1f)
            {
                // Switch axis if X close to 0.
                _isHorizontal = false;
            }
            else
            {
                moveDirection = new Vector2(moveDirection.x, 0).normalized;
            }
        }
        else
        {
            if(Mathf.Abs(moveDirection.y) < 0.1f)
            {
                // Switch axis if Y close to 0.
                _isHorizontal = true;
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
}