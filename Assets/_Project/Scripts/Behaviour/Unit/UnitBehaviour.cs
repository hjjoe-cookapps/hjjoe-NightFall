using System;
using System.Collections;
using _Project.Scripts.Defines;
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
    public int AttackWaitTime;  // 공격 대기시간
    public int AttackRange;
    public int AttackTargetCount;
}

public class UnitBehaviour : MonoBehaviour
{
    #region variable

    [SerializeField]
    private HPModule _hpModule;

    [SerializeField]
    private UnityStatus _status;

    private Quaternion _initialRotation;

    private GameObject _anyTarget; // 몬스터 아무거나
    private GameObject _inRangeTarget; //

    private bool _isScanMonster;
    private Coroutine _scanMonsterCoroutine;

    private readonly StateMachine<UnitState> _stateMachine = new();

    #endregion

    #region property

    public HPModule HPModule => _hpModule;

    public UnityStatus Status => _status;
    public GameObject AnyTarget => _anyTarget;
    public  GameObject InRangeTarget => _inRangeTarget;
    public bool IsScanMonster => _isScanMonster;

    public StateMachine<UnitState> StateMachine => _stateMachine;

    #endregion

    #region event

    private void Awake()
    {
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
        _initialRotation = transform.rotation;
        _stateMachine.ChangeState(UnitState.Idle);
    }

    private void Update()
    {
        _stateMachine.Execute();
    }

    #endregion

    public void StartGame()
    {
        _isScanMonster = true;
        _scanMonsterCoroutine = StartCoroutine(ScanMonsterCoroutine());
    }

    public void Rotation()
    {
        Quaternion rotation = Quaternion.identity;

        switch (_stateMachine.CurStateType)
        {
            case UnitState.Idle:
                break;
            case UnitState.Chase:
                //rotation = (_agent.destination.x < transform.position.x) ? Defines.Monster.LeftRotation : Defines.Monster.RightRotation;
                break;
            case UnitState.Attack:
                if (_inRangeTarget != null)
                {
                    rotation = (_inRangeTarget.transform.position.x < transform.position.x) ? Defines.Monster.LeftRotation : Defines.Monster.RightRotation;
                }
                else
                {
                    return;
                }
                break;
            case UnitState.Dead:
                return;
        }

        transform.rotation = _initialRotation * rotation;
    }

    private IEnumerator ScanMonsterCoroutine()
    {
        while (true)
        {
            UpdateRadiusMonster();
            yield return CoroutineManager.WaitForSeconds(0.2f);
        }
    }

    private void UpdateRadiusMonster()
    {

    }
}

