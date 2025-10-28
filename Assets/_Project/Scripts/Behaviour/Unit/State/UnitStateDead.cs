using _Project.Scripts.Defines;

public class UnitStateDead : UnitStateBase
{
    public UnitStateDead(StateMachine<UnitState> stateMachine, UnitBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        //_context.Agent.ResetPath();
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
