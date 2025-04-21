using UnityEngine;
using UnityEngine.EventSystems;

public class ENV01_Movement_Controller : MonoBehaviour
{
    [Header("ENV01 Stats.")]
    public float speedMin;
    public float speedMax;

    [Header("ENV01 Misc.")]
    public Vector2 moveDirection;

    private Enemy_Controller _enemyController;
    private Transform _playerTransform;
    private float _speed;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _enemyController = GetComponent<Enemy_Controller>();
        _playerTransform = Player_Controller.instance.transform;
        // Get random speed between _speedMax and _speedMin.
        _speed = Random.Range(speedMin, speedMax);
    }

    private void Update()
    {
        if(!_enemyController.isInAnimation)
        {
            // Get vector in direction of player.
            Vector2 playerDirection = (_playerTransform.transform.position - transform.position);

            // Check distance. Stop if in range to player.
            float distance = playerDirection.magnitude;
            if(distance <= .8f)
            {
                _rb.linearVelocity = Vector2.zero;
                // TEMP. (?) to replace with hitbox on animation.
                _enemyController.Attack(null);
            }
            else
            {
                Move(moveDirection);
            }
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
