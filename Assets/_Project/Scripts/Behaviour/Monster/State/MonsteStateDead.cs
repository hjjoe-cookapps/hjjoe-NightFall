using _Project.Scripts.Defines;
using UnityEngine;

public class MonsterStateDead : MonsterStateBase
{
    public MonsterStateDead(StateMachine<MonsterState> stateMachine, MonsterBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.Agent.ResetPath();
        _context.Animator.ResetTrigger("Attack");
        _context.Animator.SetBool("Action", false);
        _context.Animator.Play("Idle");
        _context.Animator.SetInteger("State", MonsterBehaviour.DeathAnimIndex);

    }

    public override void Execute()
    {
        var state = _context.Animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Death") && state.normalizedTime >= 1.0f)
        {
            ResourceManager.Instance.Destroy(_context.gameObject);
        }
        // 애니메이션 종료시 사망 처리
    }

    public override void Exit()
    {
    }

}
