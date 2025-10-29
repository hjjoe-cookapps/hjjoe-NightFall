using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    private static readonly int _moveSpeed = 1;
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

        _actionModule.OnDisableEvent -= GameManager.Instance.AddCoin;
        _actionModule.OnDisableEvent += GameManager.Instance.AddCoin;
    }

    private void Update()
    {
        Vector3 direction = _player.transform.position - transform.position;
        float magnitude = direction.magnitude;

        direction.Normalize();

        transform.position += direction * _moveSpeed;

        if (magnitude < 0.5f)
        {
            ResourceManager.Instance.Destroy(gameObject);
        }
    }
}
