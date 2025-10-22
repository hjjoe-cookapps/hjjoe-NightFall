using System.Collections;
using _Project.Scripts.Defines;
using UnityEngine;

// 수정 필요
public class MonsterStateChase : MonsterStateBase
{
    private Coroutine _chaseCoroutine;

    public MonsterStateChase(StateMachine<MonsterState> stateMachine, MonsterBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.Animator.SetInteger("State", MonsterBehaviour.MoveAnimIndex);
        _chaseCoroutine = _context.StartCoroutine(ChaseCoroutine());
    }

    public override void Execute()
    {
        CheckTargetInRadius();
        UpdateState();
    }

    private void CheckTargetInRadius()
    {
        if (_context.ChaseTarget)
        {
            Vector3 distance = _context.ChaseTarget.transform.position - _context.transform.position;
            distance.y = 0;

            if (distance.magnitude <= _context.Status.Range)
            {
                _context.InRangeTarget = _context.ChaseTarget;
            }
        }
    }

    public override void Exit()
    {
        _context.Animator.SetInteger("State", MonsterBehaviour.IdleAnimIndex);
        _context.StopCoroutine(_chaseCoroutine);
    }

    private void UpdateState()
    {
        if (_context.InRangeTarget != null)
        {
            _context.StateMachine.ChangeState(MonsterState.Attack);
        }
        else if (_context.ChaseTarget == null)
        {
            _context.StateMachine.ChangeState(MonsterState.Walk);
        }
    }

    private IEnumerator ChaseCoroutine()
    {
        while (true)
        {
            if (_context.ChaseTarget != null)
            {
                _context.Agent.SetDestination(_context.ChaseTarget.transform.position);
            }
            yield return CoroutineManager.WaitForSeconds(0.2f);
        }
    }
}
