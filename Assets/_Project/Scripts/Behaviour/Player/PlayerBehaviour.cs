using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Utils;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[Serializable]
public struct CharacterStatus
{
    public int Hp;
    public int MoveSpeed;
    public int RegenerationTime;

    public int AttackDamage;
    public int AttackCooltime;
    public int AttackRange;
    public int AttackTargetCount;

    public int SkillDamage;
    public int SkillCooltime;
    public int SkillRange;
    public int SkillSpeed;    // 스킬 애니메이션에 대해서 배속으로 제어할 것
    public int SkillTargetCount;
    public int SkillTime;   // skill 시전 시간

};

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private Character _character;
    [SerializeField]
    private Rigidbody _rigidbody;


    private CharacterStatus _status;
    private readonly float _moveSpeed = 5f; // temp;

    private bool isSkillActive = false;

    private Vector3 _moveInput;
    private Quaternion _initialRotation;

    private List<MonsterBehaviour> _inRadiusMonsters = new();
    private readonly StateMachine<PlayerState> _stateMachine = new();

    private Coroutine _scanRadiusMonsterCoroutine;

    #region property
    public CharacterStatus CharacterStatus => _status;
    public bool IsSkillActive => isSkillActive;
    public int Cooltime => isSkillActive ? _status.SkillCooltime : _status.AttackCooltime;
    public int Range => isSkillActive ? _status.SkillRange : _status.AttackRange;
    public int TargetCount => isSkillActive ? _status.SkillTargetCount : _status.AttackTargetCount;
    public List<MonsterBehaviour> InRadiusMonsters => _inRadiusMonsters;
    public StateMachine<PlayerState> StateMachine => _stateMachine;

    #endregion

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
        // get data code here
        // _status = ***

        _initialRotation = transform.rotation;
        _stateMachine.ChangeState(PlayerState.Idle);

        _scanRadiusMonsterCoroutine = StartCoroutine(ScanRadiusMonsters());
    }

    private void FixedUpdate()
    {
        FixedMove();
    }

    private void Update()
    {
        _stateMachine.Execute();
    }

    /*
    private void OnDestroy()
    {
        StopCoroutine(_scanRadiusMonsterCoroutine);
    }
    */

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

    public void OnSkill(InputValue value)
    {
        if (value.isPressed)
        {
            isSkillActive = true;
        }

        StartCoroutine(UpdateSkillTime());
    }

    private void FixedMove()
    {
        // 몬스터 타게팅 중이라면 그방향을 봐야함

        if (_moveInput.sqrMagnitude < 0.01f)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        if (_stateMachine.CurStateType != PlayerState.Idle && _moveInput.x != 0f)
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

    private IEnumerator UpdateSkillTime()
    {
        // UI와 시간 동기화 필요
        yield return CoroutineManager.WaitForSeconds(_status.SkillTime);
        isSkillActive = false;
    }

    private void UpdateRadiusMonsters()
    {
        LayerMask monsterLayer = LayerMask.GetMask("Monster");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position,
            Range, monsterLayer);
        var inRadiusMonsters = new List<(MonsterBehaviour monster, float dist)>();

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out MonsterBehaviour monsterBehaviour))
            {
                inRadiusMonsters.Add((monsterBehaviour, (transform.position - collider.transform.position).sqrMagnitude));
            }
        }

        if (inRadiusMonsters.Count > 0)
        {
            // list를 order순으로 정렬해서 저장
            _inRadiusMonsters = inRadiusMonsters.OrderBy(md => md.dist).Select(md => md.monster).ToList();
        }
    }

    private IEnumerator ScanRadiusMonsters()
    {
        while (true)
        {
            UpdateRadiusMonsters();
            yield return CoroutineManager.WaitForSeconds(0.2f);
        }
    }
}
