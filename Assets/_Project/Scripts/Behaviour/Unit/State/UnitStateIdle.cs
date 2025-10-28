using _Project.Scripts.Defines;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;

public class UnitStateIdle : UnitStateBase
{
    public UnitStateIdle(StateMachine<UnitState> stateMachine, UnitBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.Agent.ResetPath();
        _context.ExternCharacterScript.SetState(CharacterState.Idle);
    }

    public override void Execute()
    {
        UpdateState();
    }


    private void UpdateState()
    {
        if (_context.InRangeTarget != null)
        {
            _context.StateMachine.ChangeState(UnitState.Attack);
        }
        else if (_context.AnyTarget != null)
        {
            _context.StateMachine.ChangeState(UnitState.Chase);
        }

    }
}

