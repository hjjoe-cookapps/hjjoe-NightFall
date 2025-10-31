using _Project.Scripts.Defines;
using Spine;
using UnityEngine;

public class MonsterStateDead : MonsterStateBase
{
    private readonly Collider2D _collider;

    public MonsterStateDead(StateMachine<MonsterState> stateMachine, MonsterBehaviour context) : base(stateMachine, context)
    {
        _collider = context.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        _context.Rigidbody.linearVelocity = Vector2.zero;
        _context.SkeletonAnimation.AnimationState.SetAnimation(0, "Death", false);
        _collider.enabled = false;

    }

    public override void Execute()
    {
        // 사망처리는 MonsterBehaviour -> OnAnimationComplete함수 확인
    }

    public override void Exit()
    {
        _collider.enabled = true;
    }
}
