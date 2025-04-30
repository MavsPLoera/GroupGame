using UnityEngine;

public class Radius_Movement_Controller : MonoBehaviour
{
    [Header("Radius Stats.")]
    public float speedMin;
    public float speedMax;

    private Transform _playerTransform;
    private Enemy_Controller _enemyController;
    private float _speed;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerTransform = Player_Controller.instance.transform;
        _enemyController = GetComponent<Enemy_Controller>();
        // Get random speed between _speedMax and _speedMin.
        _speed = Random.Range(speedMin, speedMax);
    }

    private void Update()
    {
        if(_playerTransform && !_enemyController.isInAnimation)
        {
            // Get vector in direction of player.
            Vector2 moveDirection = (_playerTransform.transform.position - transform.position);

            // Check distance. Stop if in range to player.
            float distance = moveDirection.magnitude;
            if(distance <= .7f)
            {
                _rb.linearVelocity = Vector2.zero;
                // TEMP. (?) to replace with hitbox on animation.
                _enemyController.Attack(null);
            }
            // Check distance. Move toward if in range to player.
            else if(distance <= 5f)
            {
                moveDirection = moveDirection.normalized;
                MoveToward(moveDirection);
            }
        }
    }

    private void MoveToward(Vector2 moveDirection)
    {
        // Move along either X or Y axis. Prefer moving toward the axis
        // enemy is closest aligning to.
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
}
