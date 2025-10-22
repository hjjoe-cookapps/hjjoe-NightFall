using _Project.Scripts.Defines;

public abstract class PlayerStateBase : State<PlayerState>
{
    protected readonly PlayerBehaviour _context;

    protected PlayerStateBase(StateMachine<PlayerState> stateMachine, PlayerBehaviour context) : base(stateMachine)
    {
        _context = context;
    }
}

