using _Project.Scripts.Defines;

public class UnitStateChase : UnitStateBase
{
    public UnitStateChase(StateMachine<UnitState> stateMachine, UnitBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        //_context.Agent.SetDestination(_context.AnyTarget.transform.position);
        _context.Rotation();
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }

    private void UpdateState()
    {

    }
}
