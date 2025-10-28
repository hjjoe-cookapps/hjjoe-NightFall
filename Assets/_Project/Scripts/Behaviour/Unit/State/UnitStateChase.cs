using _Project.Scripts.Defines;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;

public class UnitStateChase : UnitStateBase
{
    public UnitStateChase(StateMachine<UnitState> stateMachine, UnitBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.Agent.SetDestination(_context.AnyTarget.transform.position);
        _context.Rotation();
        _context.ExternCharacterScript.SetState(CharacterState.Walk);
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
