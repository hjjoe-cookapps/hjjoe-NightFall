using _Project.Scripts.Defines;
using UnityEngine;

public class UnitStateIdle : UnitStateBase
{
    public UnitStateIdle(StateMachine<UnitState> stateMachine, UnitBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        if (_context.SkeletonAnimation.AnimationState.GetCurrent(0)?.Animation.Name != "Idle")
        {
            _context.SkeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
        }

        _context.Rigidbody.linearVelocity = Vector2.zero;
    }

    public override void Execute()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        if (_context.InRangeTarget != null)
        {
            _context.StateMachine.ChangeState(UnitState.Attack);
        }
        else if (_context.AnyTarget != null)
        {
            _context.StateMachine.ChangeState(UnitState.Chase);
        }
    }
}

