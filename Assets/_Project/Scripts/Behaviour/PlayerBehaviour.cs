using System;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[Serializable]
public struct CharacterStatus
{
    private int _hp;
    private int _moveSpeed;
    private int _regenerationTime;

    private int _attackDamage;
    private int _attackRange;
    private int _attackTargetCount;

    private int _skillDamage;
    private int _skillCooltime;
    private int _skillRange;
    private int _skillSpeed;    // 스킬 애니메이션에 대해서 배속으로 제어할 것
    private int _skillTargetCount;

};

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private Character _character;

    private readonly float _moveSpeed = 5f; // temp;
    private Vector3 _currentMovementInput;

    private CharacterStatus _status;

    private Quaternion _initialRotation;


    private void Awake()
    {
        if (_character == null)
        {
            _character = GetComponent<Character>();
        }
    }

    private void Start()
    {
        _initialRotation =  transform.rotation;
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

    public void OnSkill(InputValue value)
    {
        if (value.isPressed)
        {
            _character.Jab();
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
                ? _initialRotation * Quaternion.Euler(0f, 0f, 0f)
                : _initialRotation * Quaternion.Euler(0f, 180f, 0f);
        }

        transform.position += _currentMovementInput * (_moveSpeed * Time.deltaTime);
    }
}
