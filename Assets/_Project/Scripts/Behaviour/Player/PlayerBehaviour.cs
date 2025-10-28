using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Defines;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using Event = Spine.Event;

[Serializable]
public struct CharacterStatus
{
    public string Name;

    public int HP;
    public float MoveSpeed;
    public int RegenerationTime;

    public AttackType AttackType;
    public bool IsAttackSplash;
    public int AttackDamage;
    public int AttackWaitTime;  // 공격 대기시간
    public float AttackRange;
    public int AttackTargetCount;

    public bool IsSkillSplash;
    public int SkillDamage;
    public int SkillWaitTime;   // 스킬 시전중 공격 대기시간
    public float SkillRange;
    public int SkillSpeed;    // 스킬 애니메이션에 대해서 배속으로 제어할 것
    public int SkillTargetCount;
    public int SkillTime;   // skill 시전 시간
    public int SkillCooltime;// skill 쿨타임

};

public class PlayerBehaviour : MonoBehaviour, IAttackAction
{
    #region variable

    [SerializeField]
    private Rigidbody2D _rigidbody;
    [SerializeField]
    private SkeletonAnimation _skeletonAnimation;
    [SerializeField]
    private HPModule _hpModule;

    [SerializeField]
    private CharacterStatus _status; // Todo:remove

    private bool _isSkillActive = false;

    private Vector2 _moveInput;

    private List<MonsterBehaviour> _inRadiusMonsters = new();
    private readonly StateMachine<PlayerState> _stateMachine = new();

    private Coroutine _scanRadiusMonsterCoroutine;

    #endregion

    #region property
    public SkeletonAnimation SkeletonAnimation => _skeletonAnimation;
    public HPModule HPModule => _hpModule;
    public CharacterStatus CharacterStatus => _status;
    public bool IsSkillActive => _isSkillActive;

    public Vector2 MoveInput  => _moveInput;
    public bool IsSplash => _isSkillActive ? _status.IsSkillSplash : _status.IsAttackSplash;
    public int Damage => _isSkillActive ? _status.SkillDamage : _status.AttackDamage;
    public int WaitTime => _isSkillActive ? _status.SkillWaitTime : _status.AttackWaitTime;
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
        // get data code here
        // _status = ***
        _hpModule.Init(_status.HP);

        _skeletonAnimation.AnimationState.Event -= Attack;
        _skeletonAnimation.AnimationState.Event += Attack;

        _skeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
        _skeletonAnimation.AnimationState.Complete += OnAnimationComplete;

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

    public void OnSkill(InputValue value)
    {
        if (value.isPressed)
        {
            if (!_isSkillActive)
            {
                _isSkillActive = true;
                StartCoroutine(UpdateSkillTime());
                Debug.Log("Skill start");
            }
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
        yield return CoroutineManager.WaitForSeconds(_status.SkillTime);
        _isSkillActive = false;
        Debug.Log("Skill End");
    }

    private void UpdateRadiusMonsters()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, Range, Defines.MonsterLayer);
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
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(monster.transform.position, Defines.HitRange, Defines.MonsterLayer);

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
