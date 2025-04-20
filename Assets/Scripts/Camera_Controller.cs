using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("Camera Controller Misc.")]
    public bool inDungeon = false;
    public Vector3 targetPosition;

    public static Camera_Controller instance;

    private Transform _playerTransform;

    private void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _playerTransform = Player_Controller.instance.transform;
        inDungeon = false;
        transform.position = _playerTransform.position;
    }

    private void LateUpdate()
    {
        if(!inDungeon)
        {
            Vector3 targetPosition = new Vector3(_playerTransform.position.x, _playerTransform.position.y, -100f);
            transform.position = Vector3.Lerp(transform.position, targetPosition, 5f * Time.deltaTime);
        }
    }

    public void UpdatePosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }


}
