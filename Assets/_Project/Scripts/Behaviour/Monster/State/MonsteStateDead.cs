using _Project.Scripts.Defines;
using Spine;
using UnityEngine;

public class MonsterStateDead : MonsterStateBase
{
    public MonsterStateDead(StateMachine<MonsterState> stateMachine, MonsterBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.RigidBody.linearVelocity = Vector2.zero;
        _context.SkeletonAnimation.AnimationState.SetAnimation(0, "Death", false);
    }

    public override void Execute()
    {
        // 사망처리는 MonsterBehaviour -> OnAnimationComplete함수 확인
    }

    public override void Exit()
    {
    }
}
