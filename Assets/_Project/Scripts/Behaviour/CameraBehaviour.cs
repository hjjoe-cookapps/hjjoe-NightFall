using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private static readonly Vector3 _positionBias = new Vector3(0, 15, -15);

    [SerializeField]
    private GameObject _player;

    void Start()
    {
        if(_player == null)
        {
            _player =  GameObject.FindWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _player.transform.position +_positionBias;
    }
}
