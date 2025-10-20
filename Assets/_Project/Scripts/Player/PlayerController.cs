using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    //[Header("Components")]
    [FormerlySerializedAs("character")]
    [SerializeField] private Character _character;

    private readonly float _moveSpeed = 5f;
    private Vector3 _currentMovementInput;

    private void Awake()
    {
        if (_character == null)
        {
            _character = GetComponent<Character>();
        }
    }

    private void Update()
    {
        Move();
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (input.sqrMagnitude > 0.01f) // sqrMagnitude는 magnitude보다 성능이 좋음
        {
            _currentMovementInput = new Vector3(input.x, 0f, input.y);
            _character.SetState(CharacterState.Walk);
        }
        else
        {
            _currentMovementInput = Vector3.zero;
            _character.SetState(CharacterState.Idle);
        }
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            _character.Slash();
        }
    }

    private void Move()
    {
        // 몬스터 타게팅 중이라면 그방향을 봐야함

        if (_currentMovementInput.sqrMagnitude < 0.01f)
        {
            return;
        }

        if (_currentMovementInput.x != 0f)
        {
            transform.rotation = _currentMovementInput.x > 0f
                ? Quaternion.Euler(0f, 0f, 0f)
                : Quaternion.Euler(0f, 180f, 0f);
        }

        transform.position += _currentMovementInput * (_moveSpeed * Time.deltaTime);
    }
}
