using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Defines;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[Serializable]
public struct CharacterStatus
{
    public string Name;

    public int HP;
    public int MoveSpeed;
    public int RegenerationTime;

    public AttackType AttackType;
    public bool IsAttackSplash;
    public int AttackDamage;
    public int AttackWaitTime;  // 공격 대기시간
    public int AttackRange;
    public int AttackTargetCount;

    public bool IsSkillSplash;
    public int SkillDamage;
    public int SkillWaitTime;   // 스킬 시전중 공격 대기시간
    public int SkillRange;
    public int SkillSpeed;    // 스킬 애니메이션에 대해서 배속으로 제어할 것
    public int SkillTargetCount;
    public int SkillTime;   // skill 시전 시간
    public int SkillCooltime;// skill 쿨타임

    public string CharacterPrefab;
    public string AttackPrefab;
    public string SkillPrefab;
};

public class PlayerBehaviour : MonoBehaviour, IAttackAction
{
    #region variable

    [SerializeField]
    private Character _externCharacterScript;
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private AnimationEvents _animationEvents;
    [SerializeField]
    private HPModule _hpModule;

    [SerializeField]
    private CharacterStatus _status; // Todo:remove

    private bool isSkillActive = false;

    private Vector3 _moveInput;
    private Quaternion _initialRotation;

    private List<MonsterBehaviour> _inRadiusMonsters = new();
    private readonly StateMachine<PlayerState> _stateMachine = new();

    private Coroutine _scanRadiusMonsterCoroutine;

    #endregion

    #region property
    public Character ExternCharacterScript => _externCharacterScript;
    public HPModule HPModule => _hpModule;
    public CharacterStatus CharacterStatus => _status;
    public bool IsSkillActive => isSkillActive;

    public bool IsSplash => isSkillActive ? _status.IsSkillSplash : _status.IsAttackSplash;
    public int Damage => isSkillActive ? _status.SkillDamage : _status.AttackDamage;
    public int WaitTime => isSkillActive ? _status.SkillWaitTime : _status.AttackWaitTime;
    public int Range => isSkillActive ? _status.SkillRange : _status.AttackRange;
    public int TargetCount => isSkillActive ? _status.SkillTargetCount : _status.AttackTargetCount;
    public List<MonsterBehaviour> InRadiusMonsters => _inRadiusMonsters;
    public StateMachine<PlayerState> StateMachine => _stateMachine;

    #endregion

    #region event
    private void Awake()
    {
        _externCharacterScript = _externCharacterScript == null ? GetComponent<Character>() : _externCharacterScript;
        _rigidbody = _rigidbody == null ? GetComponent<Rigidbody>() : _rigidbody;
        _animationEvents = _animationEvents == null ? GetComponentInChildren<AnimationEvents>() : _animationEvents;
        _hpModule = _hpModule == null ? GetComponent<HPModule>() : _hpModule;

        _stateMachine.RegisterState<PlayerStateIdle>(PlayerState.Idle, this);
        _stateMachine.RegisterState<PlayerStateAttack>(PlayerState.Attack, this);
        _stateMachine.RegisterState<PlayerStateSkill>(PlayerState.Skill, this);
    }

    private void Start()
    {
        // get data code here
        // _status = ***
        _hpModule.Init(_status.HP);

        _initialRotation = transform.rotation;
        _stateMachine.ChangeState(PlayerState.Idle);
        _scanRadiusMonsterCoroutine = StartCoroutine(ScanRadiusMonsters());

        //TODO :: erase
        _animationEvents.OnCustomEvent -= Attack;
        _animationEvents.OnCustomEvent += Attack;
    }

    private void FixedUpdate()
    {
        FixedMove();
        Rotation();
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

    private void FixedMove()
    {
        if (_moveInput.sqrMagnitude < 0.01f)
        {
            _rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 targetVelocity = _moveInput * _status.MoveSpeed;
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

    private void Rotation()
    {
        Quaternion rotation = Quaternion.identity;
        if (_stateMachine.CurStateType != PlayerState.Idle)
        {
            if (_inRadiusMonsters.Count > 0 && _inRadiusMonsters[0])
            {
                rotation = _externCharacterScript.transform.position.x < _inRadiusMonsters[0].transform.position.x
                    ? Defines.Player.RightRotation :  Defines.Player.LeftRotation;
            }

        }
        else if (_moveInput.x != 0f) // PlayerState == Idle
        {
            rotation = _moveInput.x > 0f ? Defines.Player.RightRotation : Defines.Player.LeftRotation;
        }

        transform.rotation = _initialRotation * rotation;
    }

    private IEnumerator UpdateSkillTime()
    {
        // UI와 시간 동기화 필요
        yield return CoroutineManager.WaitForSeconds(_status.SkillTime);
        isSkillActive = false;
        Debug.Log("Skill End");
    }

    private void UpdateRadiusMonsters()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Range, Defines.MonsterLayer);
        var inRadiusMonsters = new List<(MonsterBehaviour monster, float dist)>();

        foreach (Collider collider in hitColliders)
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

    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (input.sqrMagnitude > 0.01f) // sqrMagnitude는 magnitude보다 성능이 좋음
        {
            _moveInput = new Vector3(input.x, 0f, input.y);

        }
        else
        {
            _moveInput = Vector3.zero;
            _externCharacterScript.SetState(CharacterState.Idle);
        }
    }

    public void OnSkill(InputValue value)
    {
        if (value.isPressed)
        {
            if (!isSkillActive)
            {
                isSkillActive = true;
                StartCoroutine(UpdateSkillTime());
                Debug.Log("Skill start");
            }
        }
    }

    public void Attack(string str)
    {
        HashSet<MonsterBehaviour> targets = _inRadiusMonsters.Take(TargetCount).ToHashSet();
        if (IsSplash)
        {
            HashSet<MonsterBehaviour> newTargets = new HashSet<MonsterBehaviour>(targets);
            foreach (MonsterBehaviour monster in targets)
            {
                Collider[] hitColliders = Physics.OverlapSphere(monster.transform.position, Defines.HitRange, Defines.MonsterLayer);

                foreach (Collider col in hitColliders)
                {
                    if (col.gameObject.TryGetComponent(out MonsterBehaviour monsterBehaviour))
                    {
                        newTargets.Add(monsterBehaviour);
                    }
                }
            }

            targets = newTargets;
        }

        if (isSkillActive)
        {
            SkillAction(targets);
        }
        else
        {
            AttackAction(targets);
        }
    }

    #region IAttackAction
    public void AttackAction(HashSet<MonsterBehaviour> targets)
    {
        // TODO : 구현 옮기기
        foreach (MonsterBehaviour monster in targets)
        {
            monster.HPModule.TakeDamage(Damage, gameObject);

            ParticleEffectBehaviour effect = ResourceManager.Instance.Instantiate("Effect/Slash_Straight_11_BW", monster.BodyTransform).GetComponent<ParticleEffectBehaviour>();
            if (transform.rotation.y == 0)
            {
                effect.SetDirection(Direction.Right);
            }
            else
            {
                effect.SetDirection(Direction.Left);
            }
        }
    }

    public void SkillAction(HashSet<MonsterBehaviour> targets)
    {
        foreach (MonsterBehaviour monster in targets)
        {
            monster.HPModule.TakeDamage(Damage, gameObject);

            ParticleEffectBehaviour effect = ResourceManager.Instance.Instantiate("Effect/Slash_Circle_04_BW", monster.BodyTransform).GetComponent<ParticleEffectBehaviour>();
            if (transform.rotation.y == 0)
            {
                effect.SetDirection(Direction.Right);
            }
            else
            {
                effect.SetDirection(Direction.Left);
            }
        }

    }
    #endregion
}
