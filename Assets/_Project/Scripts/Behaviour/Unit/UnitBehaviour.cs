using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Defines;
using Spine;
using Spine.Unity;
using UnityEngine;

[Serializable]
public struct UnityStatus
{
    public string Name;

    public int HP;
    public float MoveSpeed;

    public AttackType AttackType;
    public bool IsAttackSplash;

    public int AttackDamage;
    public float WaitTime;  // 공격 대기시간
    public float Range;
}

public class UnitBehaviour : MonoBehaviour
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
    private UnityStatus _status;
    [SerializeField] // TODO : Erase
    private GameObject _barracks; // unit 생성 배럭 지정
    private GameObject _anyTarget; // 몬스터 아무거나
    private GameObject _inRangeTarget; //

    private bool _isWaveStart;
    private Coroutine _scanMonsterCoroutine;

    private bool _isAttackAble;
    private Coroutine _checkAttackTimeCoroutine;


    private readonly StateMachine<UnitState> _stateMachine = new();

    #endregion

    #region property

    public Rigidbody2D Rigidbody => _rigidbody;
    public SkeletonAnimation SkeletonAnimation => _skeletonAnimation;
    public HPModule HPModule => _hpModule;

    public UnityStatus Status => _status;

    public GameObject Barracks
    {
        get => _barracks;
        set => _barracks = value;
    }
    public GameObject AnyTarget => _anyTarget;
    public  GameObject InRangeTarget => _inRangeTarget;
    public bool IsWaveStart => _isWaveStart;
    public bool IsAttackAble
    {
        get => _isAttackAble;
        set => _isAttackAble = value;
    }
    public StateMachine<UnitState> StateMachine => _stateMachine;

    #endregion

    #region event

    private void Awake()
    {
        _rigidbody = _rigidbody == null ?  GetComponent<Rigidbody2D>() : _rigidbody;
        _skeletonAnimation = _skeletonAnimation == null ? GetComponentInChildren<SkeletonAnimation>() : _skeletonAnimation;
        _hpModule = _hpModule == null ? GetComponent<HPModule>() : _hpModule;

        _stateMachine.RegisterState<UnitStateIdle>(UnitState.Idle, this);
        _stateMachine.RegisterState<UnitStateChase>(UnitState.Chase, this);
        _stateMachine.RegisterState<UnitStateAttack>(UnitState.Attack, this);
        _stateMachine.RegisterState<UnitStateDead>(UnitState.Dead, this);
    }

    private void OnEnable()
    {
        _hpModule.Init(_status.HP);
    }

    private void Start()
    {
        _skeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
        _skeletonAnimation.AnimationState.Complete += OnAnimationComplete;

        _stateMachine.ChangeState(UnitState.Idle);
    }

    private void Update()
    {
        _stateMachine.Execute();
        UpdateAnytargetMonster();
    }

    #endregion

    public void StartWave()
    {
        _isWaveStart = true;
        _scanMonsterCoroutine = StartCoroutine(ScanMonsterCoroutine());
        _checkAttackTimeCoroutine = StartCoroutine(CheckAttackTimeCoroutine());
        _stateMachine.ChangeState(UnitState.Idle);
    }

    public void EndWave()
    {
        _isWaveStart = false;
        StopCoroutine(_scanMonsterCoroutine);
        _scanMonsterCoroutine = null;
        StopCoroutine(_checkAttackTimeCoroutine);
        _checkAttackTimeCoroutine = null;
        _stateMachine.ChangeState(UnitState.Return);
    }

    public void Move()
    {
        Vector2 velocity = Vector2.zero;
        if (_anyTarget != null && _anyTarget.activeInHierarchy)
        {
            velocity = _anyTarget.transform.position - transform.position;
        }

        _rigidbody.linearVelocity = velocity.normalized * _status.MoveSpeed;
    }

    public void Rotation()
    {
        float scaleX = _skeletonAnimation.Skeleton.ScaleX;

        switch (_stateMachine.CurStateType)
        {
            case UnitState.Idle:
                break;
            case UnitState.Chase:
                if(_anyTarget != null && _anyTarget.activeSelf)
                {
                    scaleX = transform.position.x < _anyTarget.transform.position.x ? 1 : -1;
                }
                break;
            case UnitState.Attack:
                if (_inRangeTarget != null)
                {
                    scaleX = transform.position.x < _inRangeTarget.transform.position.x ? 1 : -1;
                }
                else
                {
                    return;
                }
                break;
            case UnitState.Dead:
                return;
        }

        if (scaleX != _skeletonAnimation.Skeleton.ScaleX)
        {
            _skeletonAnimation.Skeleton.ScaleX = scaleX;
        }
    }

    private IEnumerator ScanMonsterCoroutine()
    {
        while (true)
        {
            UpdateRadiusMonster();
            yield return CoroutineManager.WaitForSeconds(0.2f);
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

    //TODO : Optimize
    private void UpdateAnytargetMonster()
    {
        if (!_isWaveStart)
        {
            return;
        }

        if (_anyTarget != null && _anyTarget.activeSelf)
        {
            return;
        }

        var monsters = GameManager.Instance.Monsters;
        var targets = new List<(GameObject target, float dist)>();
        foreach (var monster in monsters)
        {
            targets.Add((monster, (transform.position - monster.transform.position).sqrMagnitude));
        }

        if (targets.Count > 0)
        {
            // list를 order순으로 정렬해서 저장
            _anyTarget = targets.OrderBy(md => md.dist).Select(md => md.target).First();
        }
        else
        {
            _anyTarget = null;
        }
    }

    private void UpdateRadiusMonster()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position,
            _status.Range, Defines.MonsterLayer);

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
}

