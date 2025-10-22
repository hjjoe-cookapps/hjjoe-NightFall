using _Project.Scripts.Defines;

public abstract class MonsterStateBase : State<MonsterState>
{
    protected readonly MonsterBehaviour _context;

    protected MonsterStateBase(StateMachine<MonsterState> stateMachine, MonsterBehaviour context) : base(stateMachine)
    {
        _context = context;
    }
}
