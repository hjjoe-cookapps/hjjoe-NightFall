using _Project.Scripts.Defines;

public class MonsterStateWalk : MonsterStateBase
{
    public MonsterStateWalk(StateMachine<MonsterState> stateMachine, MonsterBehaviour context) : base(stateMachine, context)
    {
    }

    public override void Enter()
    {
        _context.Animator.SetInteger("State", MonsterBehaviour.MoveAnimIndex);
        _context.Agent.SetDestination(_context.MainTarget.transform.position);
        _context.Rotation();
    }

    public override void Execute()
    {
        UpdateState();
    }

    public override void Exit()
    {
        _context.Animator.SetInteger("State", MonsterBehaviour.IdleAnimIndex);
    }

    private void UpdateState()
    {
        if (_context.InRangeTarget != null)
        {
            _context.StateMachine.ChangeState(MonsterState.Attack);
        }
        else if (_context.ChaseTarget != null)
        {
            _context.StateMachine.ChangeState(MonsterState.Chase);
        }
    }
}
