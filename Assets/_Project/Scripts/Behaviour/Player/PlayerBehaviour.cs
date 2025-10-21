using System;
using _Project.Scripts.Utils;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

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

    [SerializeField]
    private Rigidbody _rigidbody;


    private StateMachine<PlayerState> _stateMachine;

    private readonly float _moveSpeed = 5f; // temp;
    private Vector3 _moveInput;

    private CharacterStatus _status;

    private Quaternion _initialRotation;


    private void Awake()
    {
        _character = _character == null ? GetComponent<Character>() : _character;
        _rigidbody = _rigidbody == null ? GetComponent<Rigidbody>() : _rigidbody;

        _stateMachine.RegisterState<PlayerStateIdle>(PlayerState.Idle, this);
        _stateMachine.RegisterState<PlayerStateAttack>(PlayerState.Attack, this);
        _stateMachine.RegisterState<PlayerStateSkill>(PlayerState.Skill, this);
    }

    private void Start()
    {
        _initialRotation = transform.rotation;
        _stateMachine.ChangeState(PlayerState.Idle);
    }

    private void FixedUpdate()
    {
        FixedMove();
    }

    private void Update()
    {
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (input.sqrMagnitude > 0.01f) // sqrMagnitude는 magnitude보다 성능이 좋음
        {
            _moveInput = new Vector3(input.x, 0f, input.y);
            _character.SetState(CharacterState.Walk);
        }
        else
        {
            _moveInput = Vector3.zero;
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

    private void FixedMove()
    {
        // 몬스터 타게팅 중이라면 그방향을 봐야함

        if (_moveInput.sqrMagnitude < 0.01f)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        if (_moveInput.x != 0f)
        {
            transform.rotation = _moveInput.x > 0f
                ? _initialRotation * Quaternion.Euler(0f, 0f, 0f)
                : _initialRotation * Quaternion.Euler(0f, 180f, 0f);
        }

        Vector3 targetVelocity = _moveInput * _moveSpeed;
        Vector3 nextPosition = _rigidbody.position + targetVelocity * Time.fixedDeltaTime;

        if (NavMesh.SamplePosition(nextPosition, out NavMeshHit hit, .2f, NavMesh.AllAreas))
        {
            _rigidbody.linearVelocity = targetVelocity;
        }
        else
        {
            if (NavMesh.FindClosestEdge(_rigidbody.position, out NavMeshHit edgeHit, NavMesh.AllAreas))
            {
                Vector3 edgeNormal = edgeHit.normal;
                Vector3 slideVelocity = targetVelocity - Vector3.Dot(targetVelocity, edgeNormal) * edgeNormal;

                slideVelocity.y = targetVelocity.y;
                _rigidbody.linearVelocity = slideVelocity;
            }
            else
            {
                _rigidbody.linearVelocity = Vector3.zero;
            }
        }
    }
}
