using _Project.Scripts.Defines;

public class PlayerStateMove : PlayerStateBase
{
    public PlayerStateMove(StateMachine<PlayerState> stateMachine, PlayerBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.SkeletonAnimation.AnimationState.SetAnimation(0, "Move", true);
    }

    public override void Execute()
    {
        _context.Rotation();
        UpdateState();
    }

    public override void Exit()
    {

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
        else if (_context.MoveInput.sqrMagnitude == 0)
        {
            _context.StateMachine.ChangeState(PlayerState.Idle);
        }
    }


}
