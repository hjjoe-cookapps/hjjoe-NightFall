using _Project.Scripts.Defines;
using UnityEngine;

public class UnitStateDead : UnitStateBase
{
    public UnitStateDead(StateMachine<UnitState> stateMachine, UnitBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.Rigidbody.linearVelocity = Vector2.zero;
        _context.SkeletonAnimation.AnimationState.SetAnimation(0, "Death", false);

    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        // 사망처리는 MonsterBehaviour -> OnAnimationComplete함수 확인
    }

    private void UpdateState()
    {

    }
}
