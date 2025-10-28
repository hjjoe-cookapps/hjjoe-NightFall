using System;
using _Project.Scripts.Defines;

public abstract class UnitStateBase : State<UnitState>
{
    protected readonly UnitBehaviour _context;

    protected UnitStateBase(StateMachine<UnitState> stateMachine, UnitBehaviour context) : base(stateMachine)
    {
        _context = context;
    }
}

