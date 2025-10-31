using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Defines;
using CookApps.Inspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Event = Spine.Event;

public class PlayerBehaviour : MonoBehaviour, IAttackAction<MonsterBehaviour>
{
    #region variable

    [SerializeField]
    private Rigidbody2D _rigidbody;
    [SerializeField]
    private SkeletonAnimation _skeletonAnimation;
    [SerializeField]
    private HPModule _hpModule;
    [Required]
    [SerializeField]
    private Slider _slider;

    private Vector3 _colliderOffset;
    private Vector2 _moveInput;

    [SerializeField]
    private CharacterStatus _status; // Todo:remove

    private bool _isSkillActive;
    private bool _isAttackAble;
    private Coroutine _checkAttackTimeCoroutine;

    private List<MonsterBehaviour> _inRadiusMonsters = new();
    private Coroutine _scanRadiusMonsterCoroutine;

    private readonly StateMachine<PlayerState> _stateMachine = new();

    #endregion

    #region property
    public SkeletonAnimation SkeletonAnimation => _skeletonAnimation;
    public HPModule HPModule => _hpModule;
    public CharacterStatus CharacterStatus => _status;
    public bool IsSkillActive => _isSkillActive;
    public bool IsAttackAble
    {
        get => _isAttackAble;
        set => _isAttackAble = value;
    }

    public Vector2 MoveInput  => _moveInput;
    public bool IsSplash => _isSkillActive ? _status.IsSkillSplash : _status.IsAttackSplash;
    public float Damage => _isSkillActive ? _status.SkillDamage : _status.AttackDamage;
    public float WaitTime => _isSkillActive ? _status.SkillWaitTime : _status.AttackWaitTime;
    public float Range => _isSkillActive ? _status.SkillRange : _status.AttackRange;
    public int TargetCount => _isSkillActive ? _status.SkillTargetCount : _status.AttackTargetCount;
    public List<MonsterBehaviour> InRadiusMonsters => _inRadiusMonsters;
    public StateMachine<PlayerState> StateMachine => _stateMachine;

    #endregion

    #region event
    private void Awake()
    {
        _rigidbody = _rigidbody == null ? GetComponent<Rigidbody2D>() : _rigidbody;
        _skeletonAnimation = _skeletonAnimation == null ? GetComponentInChildren<SkeletonAnimation>() : _skeletonAnimation;
        _hpModule = _hpModule == null ? GetComponent<HPModule>() : _hpModule;

        _stateMachine.RegisterState<PlayerStateIdle>(PlayerState.Idle, this);
        _stateMachine.RegisterState<PlayerStateMove>(PlayerState.Move, this);
        _stateMachine.RegisterState<PlayerStateAttack>(PlayerState.Attack, this);
        _stateMachine.RegisterState<PlayerStateSkill>(PlayerState.Skill, this);
    }

    private void Start()
    {
        _colliderOffset = GetComponent<Collider2D>().offset;

        // get data code here
        // _status = ***
        _hpModule.Init(_status.HP);

        _hpModule.OnDamageEvent -= UpdateHpUI;
        _hpModule.OnDamageEvent += UpdateHpUI;


        _skeletonAnimation.AnimationState.Event -= Attack;
        _skeletonAnimation.AnimationState.Event += Attack;

        _skeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
        _skeletonAnimation.AnimationState.Complete += OnAnimationComplete;

        _checkAttackTimeCoroutine = StartCoroutine(CheckAttackTimeCoroutine());
        _scanRadiusMonsterCoroutine = StartCoroutine(ScanRadiusMonsters());

        _stateMachine.ChangeState(PlayerState.Idle);
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

    #endregion


    public void Rotation()
    {
        float scaleX = _skeletonAnimation.Skeleton.ScaleX;

        if (_stateMachine.CurStateType != PlayerState.Move)
        {
            if (_inRadiusMonsters.Count > 0 && _inRadiusMonsters[0])
            {
                scaleX = transform.position.x < _inRadiusMonsters[0].transform.position.x ? 1 : -1;
            }
        }
        else if (_moveInput.x != 0f) // PlayerState == Move
        {
            scaleX = _moveInput.x > 0f ? 1 : -1;
        }

        if (scaleX != _skeletonAnimation.Skeleton.ScaleX)
        {
            _skeletonAnimation.Skeleton.ScaleX = scaleX;
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (input.sqrMagnitude > 0.01f) // sqrMagnitude는 magnitude보다 성능이 좋음
        {
            _moveInput = input.normalized;
        }
        else
        {
            _moveInput = Vector2.zero;
        }
    }

    public void OnSkill()
    {
        if (!_isSkillActive)
        {
            _isSkillActive = true;
            StartCoroutine(UpdateSkillTime());

            _isAttackAble = true;
            StopCoroutine(_checkAttackTimeCoroutine);
            _checkAttackTimeCoroutine = StartCoroutine(CheckAttackTimeCoroutine());
            Debug.Log("Skill start");
        }
    }

    public void UpdateMoveAnimation()
    {
        string name = _skeletonAnimation.AnimationState.GetCurrent(0).Animation.Name;

        if (_moveInput.sqrMagnitude > 0.1f)
        {
            if (name != "Move")
            {
                _skeletonAnimation.AnimationState.SetAnimation(0, "Move", true);
            }
        }
        else
        {
            if (name != "Idle")
            {
                _skeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
            }
        }
    }

    private void FixedMove()
    {
        if (_moveInput.sqrMagnitude < 0.01f)
        {
            _rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        _rigidbody.linearVelocity = _moveInput * _status.MoveSpeed;
    }

    private IEnumerator UpdateSkillTime()
    {
        // UI와 시간 동기화 필요
        yield return CoroutineManager.WaitForSeconds(_status.SkillCooltime);
        _isSkillActive = false;
        _isAttackAble = true;
        StopCoroutine(_checkAttackTimeCoroutine);
        _checkAttackTimeCoroutine = StartCoroutine(CheckAttackTimeCoroutine());
        Debug.Log("Skill End");
    }

    private void UpdateRadiusMonsters()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position + _colliderOffset, Range, Defines.MonsterLayer);
        var inRadiusMonsters = new List<(MonsterBehaviour monster, float dist)>();

        foreach (Collider2D collider in hitColliders)
        {
            if (collider.gameObject.TryGetComponent(out MonsterBehaviour monsterBehaviour))
            {
                inRadiusMonsters.Add(
                    (monsterBehaviour, (transform.position - collider.transform.position).sqrMagnitude));
            }
        }

        // list를 order순으로 정렬해서 저장
        _inRadiusMonsters = inRadiusMonsters.OrderBy(md => md.dist).Select(md => md.monster).ToList();
    }

    private IEnumerator ScanRadiusMonsters()
    {
        while (true)
        {
            UpdateRadiusMonsters();
            yield return CoroutineManager.WaitForSeconds(0.2f);
        }
    }

    private IEnumerator CheckAttackTimeCoroutine()
    {
        while (true)
        {
            if (!_isAttackAble)
            {
                yield return CoroutineManager.WaitForSeconds(WaitTime);
                _isAttackAble = true;
            }

            yield return null;
        }
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

    private void Attack(TrackEntry trackEntry, Event e)
    {
        if (e.Data.Name != "Attack_Hit")
        {
            return;
        }

        HashSet<MonsterBehaviour> targets = _inRadiusMonsters.Take(TargetCount).ToHashSet();
        if (IsSplash)
        {
            HashSet<MonsterBehaviour> newTargets = new HashSet<MonsterBehaviour>(targets);
            foreach (MonsterBehaviour monster in targets)
            {
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(monster.transform.position + _colliderOffset, Defines.HitRange, Defines.MonsterLayer);

                foreach (Collider2D col in hitColliders)
                {
                    if (col.gameObject.TryGetComponent(out MonsterBehaviour monsterBehaviour))
                    {
                        newTargets.Add(monsterBehaviour);
                    }
                }
            }

            targets = newTargets;
        }

        if (_isSkillActive)
        {
            SkillAction(targets);
        }
        else
        {
            AttackAction(targets);
        }
    }

    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry.TrackIndex == 1 &&
            (trackEntry.Animation.Name == "Attack" || trackEntry.Animation.Name == "Skill"))
        {
            _skeletonAnimation.AnimationState.SetEmptyAnimation(1, 0.4f);
        }
    }

    #region IAttackAction
    public void AttackAction(HashSet<MonsterBehaviour> targets)
    {
        // TODO : 구현 옮기기
        foreach (MonsterBehaviour monster in targets)
        {
            monster.HpModule.TakeDamage(Damage, gameObject);

            //ParticleEffectBehaviour effect = ResourceManager.Instance.Instantiate("Effect/Slash_Straight_11_BW", monster.CenterTransform).GetComponent<ParticleEffectBehaviour>();
            //if (transform.rotation.y == 0)
            //{
            //    effect.SetDirection(Direction.Right);
            //}
            //else
            //{
            //    effect.SetDirection(Direction.Left);
            //}
        }
    }

    public void SkillAction(HashSet<MonsterBehaviour> targets)
    {
        foreach (MonsterBehaviour monster in targets)
        {
            monster.HpModule.TakeDamage(Damage, gameObject);

            //ParticleEffectBehaviour effect = ResourceManager.Instance.Instantiate("Effect/Slash_Circle_04_BW", monster.CenterTransform).GetComponent<ParticleEffectBehaviour>();
            //if (transform.rotation.y == 0)
            //{
            //    effect.SetDirection(Direction.Right);
            //}
            //else
            //{
            //    effect.SetDirection(Direction.Left);
            //}
        }

    }
    #endregion
}
