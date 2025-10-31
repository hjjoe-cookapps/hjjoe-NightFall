using _Project.Scripts.Defines;
using Spine;
using UnityEngine;

public class UnitStateAttack : UnitStateBase
{
    public UnitStateAttack(StateMachine<UnitState> stateMachine, UnitBehaviour context) : base(stateMachine, context)
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
        Attack();
        UpdateState();
    }

    public override void Exit()
    {
    }

    private void UpdateState()
    {
        if (_context.InRangeTarget == null)
        {
            TrackEntry trackEntry = _context.SkeletonAnimation.AnimationState.GetCurrent(0);
            if (trackEntry.Animation.Name != "Attack")
            {
                _context.StateMachine.ChangeState(UnitState.Chase);
            }
        }
    }

    private void Attack()
    {
        if (_context.IsAttackAble)
        {
            _context.IsAttackAble = false;
            TrackEntry trackEntry = _context.SkeletonAnimation.AnimationState.SetAnimation(0, "Attack", false);
            trackEntry.Complete += delegate
            {
                _context.SkeletonAnimation.AnimationState.AddAnimation(0, "Idle", true, 0);
            };
            _context.Rotation();
        }
    }
}
