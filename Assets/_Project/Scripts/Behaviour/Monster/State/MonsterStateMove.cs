using _Project.Scripts.Defines;
using UnityEngine;

public class MonsterStateMove : MonsterStateBase
{
    public MonsterStateMove(StateMachine<MonsterState> stateMachine, MonsterBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        if (_context.SkeletonAnimation.AnimationState.GetCurrent(0)?.Animation.Name != "Move")
        {
            _context.SkeletonAnimation.AnimationState.SetAnimation(0, "Move", true);
        }
        _context.Rotation();
    }

    public override void Execute()
    {
        //Debug.Log(_context.Agent.destination);
        //_context.Agent.SetDestination(_context.MainTarget.transform.position);
        //_context.Rotation();
        //Debug.Log(_context.Agent.destination);
        UpdateState();
        _context.Move();
    }

    public override void Exit()
    {
    }

    private void UpdateState()
    {
        if (_context.InRangeTarget != null)
        {
            _context.StateMachine.ChangeState(MonsterState.Attack);
        }
        else if (_context.ChaseTarget != null)
        {
            _context.StateMachine.ChangeState(MonsterState.Chase);
        }
    }
}
