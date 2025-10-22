using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Defines;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public struct MonsterStatus
{
    public string Name;

    public int Hp;
    public int MoveSpeed;

    public int Damage;
    public int Cooltime;
    public int Range;
};

public class MonsterBehaviour : MonoBehaviour
{
    public static readonly int IdleAnimIndex = 0;
    public static readonly int MoveAnimIndex = 2;

    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private Animator _animator;
    //Todo: SerializedField 삭제할것, 테스트 용임
    [SerializeField]
    private MonsterStatus _status;

    private GameObject _mainTarget;
    private GameObject _inRangeTarget;

    private bool _isAttacked;   // 공격 받은 경우 그 대상을 특정 시간동안 추적하게 처리
    private GameObject _chaseTarget;
    private Coroutine _chaseCoroutine;

    private readonly StateMachine<MonsterState> _stateMachine = new();

    #region property

    public NavMeshAgent Agent => _agent;
    public Animator Animator => _animator;
    public MonsterStatus Status => _status;
    public GameObject MainTarget => _mainTarget;
    public GameObject InRangeTarget
    {
        get => _inRangeTarget;
        set => _inRangeTarget = value;
    }

    public GameObject ChaseTarget => _chaseTarget;
    public StateMachine<MonsterState> StateMachine => _stateMachine;

    #endregion

    #region Event

    private void Awake()
    {
        if (_agent == null)
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        _stateMachine.RegisterState<MonsterStateWalk>(MonsterState.Walk, this);
        _stateMachine.RegisterState<MonsterStateChase>(MonsterState.Chase, this);
        _stateMachine.RegisterState<MonsterStateAttack>(MonsterState.Attack, this);
        _stateMachine.RegisterState<MonsterStateDead>(MonsterState.Dead, this);
    }

    private void OnEnable()
    {
        _mainTarget = GameManager.Instance.Castle;
        _inRangeTarget = null;
        _stateMachine.ChangeState(MonsterState.Walk);
    }

    private void Update()
    {
        UpdateRadiusTargets();
        UpdateInRangeTarget();
        UpdateChaseTarget();

        _stateMachine.Execute();
    }

    #endregion

    public void OnAttacked(GameObject target)
    {

        // 공격 당한 경우, 공격 당한 상태가 아닐 경우 상대방을 추적대상으로 선정
        // 그 대상을 일정 기간 추적
        // 시간 경과후 대상 삭제
        // 이미 추적 대상이 있는 경우에는 처리되지 않음
        // 이동 중 다른 유닛, 건물, 플레이어가 공격 범위에 있다면 그 대상을 공격해야함

        if (!_isAttacked)
        {
            _isAttacked = true;
            _chaseTarget = target;
            _chaseCoroutine = StartCoroutine(ChaseCoroutine());
        }
        else if(_chaseTarget == target)
        {
            StopCoroutine(_chaseCoroutine);
            _chaseCoroutine = StartCoroutine(ChaseCoroutine());
        }

    }

    private void UpdateRadiusTargets()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position,
            _status.Range, Defines.FriendLayer);

        var inRadiusTargets = new List<(GameObject target, float dist)>();

        foreach (Collider collider in hitColliders)
        {
            inRadiusTargets.Add((collider.gameObject, (transform.position - collider.transform.position).sqrMagnitude));
        }

        if (inRadiusTargets.Count > 0)
        {
            // list를 order순으로 정렬해서 저장
            _inRangeTarget = inRadiusTargets.OrderBy(md => md.dist).Select(md => md.target).First();
        }
        else
        {
            _inRangeTarget = null;
        }
    }

    private void UpdateInRangeTarget()
    {
        if (_inRangeTarget != null && !_inRangeTarget.activeSelf)
        {
            _inRangeTarget = null;
        }
    }

    private void UpdateChaseTarget()
    {
        if (_isAttacked)
        {
            if (_chaseTarget == null || !_chaseTarget.activeSelf)
            {
                ReleaseChase();
            }
        }
    }

    private IEnumerator ChaseCoroutine()
    {
        yield return CoroutineManager.WaitForSeconds(5f);
        if (_stateMachine.CurStateType == MonsterState.Chase)
        {
            ReleaseChase();
        }
    }

    private void ReleaseChase()
    {
        _isAttacked = false;
        _chaseTarget = null;
        StopCoroutine(_chaseCoroutine);
    }
}
