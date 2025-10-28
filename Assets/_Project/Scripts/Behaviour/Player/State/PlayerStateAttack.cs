using System.Collections;
using System.Linq;
using _Project.Scripts.Defines;
using UnityEngine;

public class PlayerStateAttack : PlayerStateBase
{
    private Coroutine _coroutine;

    public PlayerStateAttack(StateMachine<PlayerState> stateMachine, PlayerBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        if (_context.SkeletonAnimation.AnimationState.GetCurrent(0)?.Animation.Name != "Idle")
        {
            _context.SkeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
        }
        _coroutine = _context.StartCoroutine(AttackCoroutine());
    }

    public override void Execute()
    {
        _context.UpdateMoveAnimation();
        _context.Rotation();
        UpdateState();
    }

    public override void Exit()
    {
        // exit current animation
        _context.StopCoroutine(_coroutine);
    }

    private void UpdateState()
    {
        if (_context.InRadiusMonsters.Count == 0)
        {
            _context.StateMachine.ChangeState(PlayerState.Idle);
        }
        else if (_context.IsSkillActive)
        {
            _context.StateMachine.ChangeState(PlayerState.Skill);
        }
    }


    private IEnumerator AttackCoroutine()
    {
        while (true)
        {
            _context.SkeletonAnimation.AnimationState.SetAnimation(1, "Attack", false);
            yield return CoroutineManager.WaitForSeconds(_context.WaitTime);
        }
    }
}
