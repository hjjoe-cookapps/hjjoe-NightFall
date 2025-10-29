using UnityEngine;

public class WoodBehaviour : MonoBehaviour
{
    private static readonly float _moveSpeed = 0.2f;
    [SerializeField]
    private ActionModule _actionModule;

    private GameObject _player;

    private void Awake()
    {
        _actionModule = gameObject.GetOrAddComponent<ActionModule>();
    }

    private void Start()
    {
        _player = GameManager.Instance.Player;

        _actionModule.OnDisableEvent -= GameManager.Instance.AddWood;
        _actionModule.OnDisableEvent += GameManager.Instance.AddWood;
    }

    private void Update()
    {
        Vector3 direction = _player.transform.position - transform.position;
        float magnitude = direction.magnitude;

        direction.Normalize();

        transform.position += direction * _moveSpeed;

        if (magnitude < 0.2f)
        {
            ResourceManager.Instance.Destroy(gameObject);
        }
    }
}
