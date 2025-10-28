using _Project.Scripts.Defines;
using UnityEngine;

public class PlayerStateIdle : PlayerStateBase
{
    public PlayerStateIdle(StateMachine<PlayerState> stateMachine, PlayerBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.SkeletonAnimation.AnimationState.SetAnimation(0, "Idle", true);
    }
    public override void Execute()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        if (_context.InRadiusMonsters.Count > 0)
        {
            if (_context.IsSkillActive)
            {
                _context.StateMachine.ChangeState(PlayerState.Skill);
            }
            else
            {
                _context.StateMachine.ChangeState(PlayerState.Attack);
            }
        }
        else if (_context.MoveInput.sqrMagnitude > 0)
        {
            _context.StateMachine.ChangeState(PlayerState.Move);
        }
    }
}
