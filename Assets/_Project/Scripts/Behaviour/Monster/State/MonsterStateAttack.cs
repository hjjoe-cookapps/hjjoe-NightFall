using System.Collections;
using _Project.Scripts.Defines;
using UnityEngine;

public class MonsterStateAttack : MonsterStateBase
{
    private Coroutine _coroutine;

    public MonsterStateAttack(StateMachine<MonsterState> stateMachine, MonsterBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.Agent.ResetPath();
        _coroutine = _context.StartCoroutine(AttackCoroutine());
    }

    public override void Execute()
    {
        UpdateState();
    }

    public override void Exit()
    {
        _context.StopCoroutine(_coroutine);
        _coroutine = null;
        _context.Animator.ResetTrigger("Attack");
    }

    private void UpdateState()
    {
        if (_context.InRangeTarget == null)
        {
            AnimatorStateInfo stateInfo = _context.Animator.GetCurrentAnimatorStateInfo(0);

            if (!stateInfo.IsName("Attack"))
            {
                _context.StateMachine.ChangeState(MonsterState.Walk);
            }
        }
    }

    private IEnumerator AttackCoroutine()
    {
        while (true)
        {
            _context.Animator.SetTrigger("Attack");
            _context.Rotation();
            yield return CoroutineManager.WaitForSeconds(_context.Status.Cooltime);
        }
    }
}
