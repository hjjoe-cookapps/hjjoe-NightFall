using _Project.Scripts.Defines;

public class UnitStateChase : UnitStateBase
{
    public UnitStateChase(StateMachine<UnitState> stateMachine, UnitBehaviour context) : base(stateMachine, context)
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
        _context.Move();
        UpdateState();
    }

    public override void Exit()
    {
    }

    private void UpdateState()
    {
        if (_context.InRangeTarget != null)
        {
            _context.StateMachine.ChangeState(UnitState.Attack);

        }
        else if (_context.AnyTarget == null)
        {
            _context.StateMachine.ChangeState(UnitState.Idle);
        }
    }
}
