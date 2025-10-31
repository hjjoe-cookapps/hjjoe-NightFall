using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Defines;
using CookApps.Inspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Event = Spine.Event;

public abstract class UnitBehaviour : MonoBehaviour , IAttackAction<MonsterBehaviour>
{
    #region variable

    public event Action<UnitBehaviour> OnDeadEvent;

    [Required]
    private UnitType _unitType;

    [SerializeField]
    private Rigidbody2D _rigidbody;
    [SerializeField]
    private SkeletonAnimation _skeletonAnimation;
    [SerializeField]
    private HPModule _hpModule;
    [Required]
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    protected UnitStatus _status;
    private Transform _barracks; // unit 생성 배럭 지정

    private Vector3 _colliderOffset;

    private MonsterBehaviour _anyTarget; // 몬스터 아무거나
    private MonsterBehaviour _inRangeTarget;

    private bool _isWaveStart;
    private Coroutine _scanMonsterCoroutine;

    private bool _isAttackAble;
    private Coroutine _checkAttackTimeCoroutine;


    private readonly StateMachine<UnitState> _stateMachine = new();

    #endregion

    #region property

    public UnitType UnitType => _unitType;
    public Rigidbody2D Rigidbody => _rigidbody;
    public SkeletonAnimation SkeletonAnimation => _skeletonAnimation;
    public HPModule HPModule => _hpModule;

    public UnitStatus Status => _status;

    public Transform Barracks
    {
        get => _barracks;
        set => _barracks = value;
    }
    public MonsterBehaviour AnyTarget => _anyTarget;
    public  MonsterBehaviour InRangeTarget => _inRangeTarget;
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
        _stateMachine.RegisterState<UnitStateReturn>(UnitState.Return, this);
        _stateMachine.RegisterState<UnitStateDead>(UnitState.Dead, this);
    }

    private void Start()
    {
        Enum.TryParse(gameObject.name, out _unitType);
        _status = SpecDataManager.Instance.GetUnitStatus(_unitType);

        _colliderOffset = GetComponent<Collider2D>().offset;

        _hpModule.OnDeadEvent -= OnDead;
        _hpModule.OnDeadEvent += OnDead;

        _hpModule.OnDamageEvent -= UpdateHpUI;
        _hpModule.OnDamageEvent += UpdateHpUI;

        _skeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
        _skeletonAnimation.AnimationState.Complete += OnAnimationComplete;

        _skeletonAnimation.AnimationState.Event -= Attack;
        _skeletonAnimation.AnimationState.Event += Attack;

        _stateMachine.ChangeState(UnitState.Idle);
    }

    private void Update()
    {
        _stateMachine.Execute();
        UpdateAnytargetMonster();
    }

    private void OnDisable()
    {
        OnDeadEvent?.Invoke(this);
        OnDeadEvent = null;
    }

    #endregion

    public void StartWave()
    {
        _hpModule.Init(_status.HP);
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
        if (_anyTarget != null && _anyTarget.gameObject.activeInHierarchy)
        {
            velocity = _anyTarget.transform.position - transform.position;
        }

        _rigidbody.linearVelocity = velocity.normalized * _status.MoveSpeed;
    }

    private void OnDead()
    {
        _stateMachine.ChangeState(UnitState.Dead);
    }

    private void UpdateHpUI()
    {
        _slider.value = _hpModule.HP / _hpModule.MaxHP;
        if (_hpModule.HP == _hpModule.MaxHP)
        {
            _slider.gameObject.SetActive(false);
        }
        else
        {
            _slider.gameObject.SetActive(true);
        }
    }

    public void Rotation()
    {
        float scaleX = _skeletonAnimation.Skeleton.ScaleX;

        switch (_stateMachine.CurStateType)
        {
            case UnitState.Idle:
                break;
            case UnitState.Chase:
                if(_anyTarget != null && _anyTarget.gameObject.activeSelf)
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

    private void Attack(TrackEntry trackEntry, Event e)
    {
        if (e.Data.Name != "Attack_Hit")
        {
            return;
        }

        // 애니메이터에 의해 공격 애니메이션 중 발생하는 함수
        if (_inRangeTarget == null || !_inRangeTarget.gameObject.activeSelf)
        {
            return;
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position + _colliderOffset,
            _status.Range, Defines.MonsterLayer);

        foreach (Collider2D col in hitColliders)
        {
            if (col.gameObject == _inRangeTarget.gameObject)
            {
                this.AttackAction(new HashSet<MonsterBehaviour>{_inRangeTarget});
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

        if (_anyTarget != null && _anyTarget.gameObject.activeSelf)
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
            _anyTarget = targets.OrderBy(md => md.dist).Select(md => md.target).FirstOrDefault()?.GetComponent<MonsterBehaviour>();
        }
        else
        {
            _anyTarget = null;
        }
    }

    private void UpdateRadiusMonster()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position + _colliderOffset,
            _status.Range, Defines.MonsterLayer);

        var inRadiusTargets = new List<(MonsterBehaviour target, float dist)>();

        foreach (Collider2D col in hitColliders)
        {
            if (col.gameObject.TryGetComponent(out MonsterBehaviour monsterBehaviour))
            {
                inRadiusTargets.Add((monsterBehaviour, (transform.position - col.transform.position).sqrMagnitude));
            }
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


    #region IAttackAction

    public abstract void AttackAction(HashSet<MonsterBehaviour> targets);

    public abstract  void SkillAction(HashSet<MonsterBehaviour> targets);

    #endregion
}

