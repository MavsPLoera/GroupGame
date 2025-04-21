using System.Collections;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("Camera Controller Misc.")]
    public bool inDungeon = false;

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
           Vector3 targetPosition = new(_playerTransform.position.x, _playerTransform.position.y, -100f);
           transform.position = Vector3.Lerp(transform.position, targetPosition, 5f * Time.deltaTime);
        }
    }

    public IEnumerator UpdatePosition(Vector3 targetPosition, System.Action callback = null)
    {
        Dungeon_Controller.instance.isTransitioning = true;
        Player_Controller.instance.canInput = false;
        Player_Controller.instance.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        float mult = Vector2.Distance(transform.position, targetPosition) / 3;
        /*
        while(Vector3.Distance(transform.position, targetPosition) > 0.5f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, 2f * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        */
        while(transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, mult * Time.deltaTime);
            yield return null;
        }
        callback?.Invoke();
        Dungeon_Controller.instance.isTransitioning = false;
        Player_Controller.instance.canInput = true;
        Player_Controller.instance.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
