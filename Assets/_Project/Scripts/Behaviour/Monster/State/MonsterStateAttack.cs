using System.Collections;
using _Project.Scripts.Defines;
using Spine;
using UnityEngine;

public class MonsterStateAttack : MonsterStateBase
{
    private Coroutine _coroutine;

    public MonsterStateAttack(StateMachine<MonsterState> stateMachine, MonsterBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.RigidBody.linearVelocity = Vector2.zero;
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
    }

    private void UpdateState()
    {
        if (_context.InRangeTarget == null)
        {
            TrackEntry trackEntry = _context.SkeletonAnimation.AnimationState.GetCurrent(0);
            if (trackEntry.Animation.Name != "Attack")
            {
                _context.StateMachine.ChangeState(MonsterState.Move);
            }
        }
    }

    private IEnumerator AttackCoroutine()
    {
        while (true)
        {
            _context.SkeletonAnimation.AnimationState.SetAnimation(0, "Attack", false);
            _context.SkeletonAnimation.AnimationState.AddAnimation(0, "Idle", false, 0);
            _context.Rotation();
            yield return CoroutineManager.WaitForSeconds(_context.Status.Cooltime);
        }
    }
}
