using _Project.Scripts.Defines;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;

public class UnitStateDead : UnitStateBase
{
    public UnitStateDead(StateMachine<UnitState> stateMachine, UnitBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.Agent.ResetPath();

        _context.ExternCharacterScript.SetState(CharacterState.DeathF);
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
