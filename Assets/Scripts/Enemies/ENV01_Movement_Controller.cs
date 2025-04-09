using UnityEngine;
using UnityEngine.EventSystems;

public class ENV01_Movement_Controller : MonoBehaviour
{
    [Header("Stats.")]
    public float speedMin;
    public float speedMax;

    [Header("ENV01 Misc.")]
    public Vector2 moveDirection;

    private float _speed;
    private Rigidbody2D _rb;
    private bool _isKnockedback = false;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ColliderTilemap"))
        {
            // Switch directions.
            moveDirection *= -1;
        }
    }
}
