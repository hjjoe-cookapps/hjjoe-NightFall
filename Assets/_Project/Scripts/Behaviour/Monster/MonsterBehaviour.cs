using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Defines;
using Spine;
using Spine.Unity;
using UnityEngine;
using Event = Spine.Event;
using MonsterState = _Project.Scripts.Defines.MonsterState;

[Serializable]
public struct MonsterStatus
{
    public string Name;

    public int HP;
    public float MoveSpeed;

    public int Damage;
    public float WaitTime;
    public float Range;
};

public class MonsterBehaviour : MonoBehaviour
{
    #region variable
    public event Action<GameObject> OnDeadEvent;

    [SerializeField]
    private Rigidbody2D _rigidbody;
    [SerializeField]
    private SkeletonAnimation _skeletonAnimation;
    [SerializeField]
    private HPModule _hpModule;
    [SerializeField]
    private Transform _centerTransform;

    //Todo: SerializedField 삭제할것, 테스트 용임
    [SerializeField]
    private MonsterStatus _status;

    private GameObject _mainTarget;
    private GameObject _inRangeTarget;

    private bool _isAttackAble;
    private Coroutine _checkAttackTimeCoroutine;

    private bool _isAttacked;   // 공격 받은 경우 그 대상을 특정 시간동안 추적하게 처리
    private GameObject _chaseTarget;
    private Coroutine _chaseCoroutine;

    private readonly StateMachine<MonsterState> _stateMachine = new();

    #endregion

    #region property
    public Rigidbody2D Rigidbody => _rigidbody;
    public SkeletonAnimation SkeletonAnimation => _skeletonAnimation;
    public Transform CenterTransform => _centerTransform;
    public HPModule  HpModule => _hpModule;
    public MonsterStatus Status => _status;
    public GameObject MainTarget => _mainTarget;
    public GameObject InRangeTarget
    {
        get => _inRangeTarget;
        set => _inRangeTarget = value;
    }

    public bool IsAttackAble
    {
        get => _isAttackAble;
        set => _isAttackAble = value;
    }

    public GameObject ChaseTarget => _chaseTarget;
    public StateMachine<MonsterState> StateMachine => _stateMachine;

    #endregion

    #region event
    private void Awake()
    {
        _rigidbody = _rigidbody == null ? GetComponent<Rigidbody2D>() : _rigidbody;
        _skeletonAnimation = _skeletonAnimation == null ? GetComponentInChildren<SkeletonAnimation>() : _skeletonAnimation;
        _centerTransform = _centerTransform == null ? transform.Find("Body")?.GetComponent<Transform>() : _centerTransform;
        _hpModule = _hpModule == null ? GetComponent<HPModule>() : _hpModule;

        _stateMachine.RegisterState<MonsterStateMove>(MonsterState.Move, this);
        _stateMachine.RegisterState<MonsterStateChase>(MonsterState.Chase, this);
        _stateMachine.RegisterState<MonsterStateAttack>(MonsterState.Attack, this);
        _stateMachine.RegisterState<MonsterStateDead>(MonsterState.Dead, this);
    }

    private void OnEnable()
    {
        _hpModule.Init(_status.HP);

        _mainTarget = GameManager.Instance.Castle;
        _inRangeTarget = null;

        _checkAttackTimeCoroutine = StartCoroutine(CheckAttackTimeCoroutine());
        _stateMachine.ChangeState(MonsterState.Move);

    }

    private void Start()
    {
        _skeletonAnimation.AnimationState.Event -= Attack;
        _skeletonAnimation.AnimationState.Event += Attack;

        _skeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
        _skeletonAnimation.AnimationState.Complete += OnAnimationComplete;

        _hpModule.OnDamageEventOpponent -= OnDamaged;
        _hpModule.OnDamageEventOpponent += OnDamaged;

        _hpModule.OnDeadEvent -= OnDead;
        _hpModule.OnDeadEvent += OnDead;

    }

    private void Update()
    {
        UpdateRadiusTargets();
        UpdateInRangeTarget();
        UpdateChaseTarget();

        _stateMachine.Execute();
    }

    private void OnDisable()
    {
        OnDeadEvent?.Invoke(gameObject);
        StopAllCoroutines();
    }

    #endregion

    public void Rotation()
    {
        float scaleX = _skeletonAnimation.Skeleton.ScaleX;

        switch (_stateMachine.CurStateType)
        {
            case MonsterState.Move:
                if(_mainTarget != null && _mainTarget.activeSelf)
                {
                    scaleX = transform.position.x < _mainTarget.transform.position.x ? 1 : -1;
                }

                break;
            case MonsterState.Chase:
                if(_chaseTarget != null && _chaseTarget.activeSelf)
                {
                    scaleX = transform.position.x < _chaseTarget.transform.position.x ? 1 : -1;
                }
                break;
            case MonsterState.Attack:
                if (_inRangeTarget != null && _inRangeTarget.activeSelf)
                {
                    scaleX = transform.position.x < _inRangeTarget.transform.position.x ? 1 : -1;
                }
                break;
            case MonsterState.Dead:
                return;
        }

        if (scaleX != _skeletonAnimation.Skeleton.ScaleX)
        {
            _skeletonAnimation.Skeleton.ScaleX = scaleX;
        }
    }

    public void Move()
    {
        Vector2 velocity = Vector2.zero;
        switch (_stateMachine.CurStateType)
        {
            case MonsterState.Move:
                if (_mainTarget != null && _mainTarget.activeSelf)
                {
                    velocity = _mainTarget.transform.position - transform.position;
                }
                break;
            case MonsterState.Chase:
                if (_chaseTarget != null && _chaseTarget.activeSelf)
                {
                    velocity = _chaseTarget.transform.position - transform.position;
                }
                break;
            case MonsterState.Attack:
            case MonsterState.Dead:
                return;
        }

        _rigidbody.linearVelocity = velocity.normalized * _status.MoveSpeed;
    }

    public void OnDead()
    {
        _stateMachine.ChangeState(MonsterState.Dead);
    }

    public void OnDamaged(GameObject target)
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

    private void Attack(TrackEntry trackEntry, Event e)
    {
        if (e.Data.Name != "Attack_Hit")
        {

            return;
        }

        // 애니메이터에 의해 공격 애니메이션 중 발생하는 함수
        if (_inRangeTarget == null || !_inRangeTarget.activeSelf)
        {
            return;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position,
            _status.Range, Defines.FriendLayer);

        foreach (Collider2D col in hitColliders)
        {
            if (col.gameObject == _inRangeTarget)
            {
                _inRangeTarget.GetRoot().GetComponent<HPModule>()?.TakeDamage(_status.Damage, gameObject);
            }
        }
    }

    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        switch (trackEntry.Animation.Name)
        {
            case "Death":
            {
                ResourceManager.Instance.Destroy(gameObject);
                break;
            }
        }
    }

    private void UpdateRadiusTargets()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position,
            _status.Range, Defines.FriendLayer);

        var inRadiusTargets = new List<(GameObject target, float dist)>();

        foreach (Collider2D collider in hitColliders)
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

    private IEnumerator CheckAttackTimeCoroutine()
    {
        while (true)
        {
            if (!_isAttackAble)
            {
                yield return CoroutineManager.WaitForSeconds(_status.WaitTime);
                _isAttackAble = true;
            }

            yield return null;
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
